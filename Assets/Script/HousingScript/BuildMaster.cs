using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SelectedBuildType
{
    Floor,
    Celling,
    Wall,
    Door,
    Window,
    RepairTools
}

[System.Serializable]
public enum ConnectorType
{
    FloorConnector,
    CellingConnector,
    BasicWallConnector,
    RotatedWallConnector
}

[System.Serializable]
public enum ConnectorGroup
{
    FloorConnectorGroup,
    CellingConnectorGroup,
    BasicWallConnectorGroup,
    None
}

public class BuildMaster : Singleton<BuildMaster>
{

    [Header("===Transform===")]
    public GameObject _playerCamera;
    // Housing Block�� , Conenctor�� �θ� 
    public Transform _parentTransform;

    [Header("===Script===")]
    public MyBuildManager myBuildManger;                    // build�ϴ� ���� ���� ( snapbuild & repairDestroy )
    public MyBuildCheck mybuildCheck;                       // build�� ��ᰡ ������� & build Process Ui 
    public HousingDataManager housingDataManager;           // csv ������ �ҷ����� , �ʱ�ȭ 
    public HousingUiManager housingUiManager;               // ��Ŭ���� ui ���� 
    public Housing_SnapBuild housingSnapBuild;
    public Housing_RepairDestroy housingRepairDestroy;

    // �� sprite 
    [Header("===Sprite===")]
    [SerializeField] public List<Sprite> _blockSprite;

    // ���� ���� �ִ� ���� ������ 
    [SerializeField] private HousingBlock _currBlockData;

    [Header("===Idx===")]
    public int _buildTypeIdx;                              // ���� Ÿ������
    public int _buildDetailIdx;                            // �� Ÿ�Ծ� ���° ������Ʈ ����

    // Housing SnapBuild�� Housing repairDestroy���� �������� ������� Layer
    [Header("===Layer===")]
    public LayerMask _buildFinishedLayer;                           // �� ���� ���� layermask   
    public int _buildFinishedint;                                   // �� ���� ���� layer int
    public List<Tuple<LayerMask, int>> _connectorLayer;             // Ŀ���� ���̾� 
    [HideInInspector] public LayerMask _ConnectorWholelayer;        // ��� Ŀ���� ���̾� �� ��ģ
    private int _placedItemLayerInt;                                // ��ġ �Ϸ��� ������Ʈ layer int
               
    // ������Ƽ 
    public HousingBlock currBlockData { get => _currBlockData;  }
    public int placedItemLayerInt { get => _placedItemLayerInt;  }

    protected override void InitManager()
    {
        // 0. ���̾� �ʱ�ȭ
        F_InitLayer();

        // 1. savemanager ��������Ʈ�� ���� 
        SaveManager.Instance.GameDataSave += () => SaveManager.Instance.F_SaveBuilding(_parentTransform.transform);
        // 1. Connector ����
        BuildMaster.Instance.housingDataManager.F_InitConnectorInfo();
        // 1. savemanager���� �ҷ����� 
        SaveManager.Instance.F_LoadBuilding(_parentTransform);
    }

    public void F_SetBlockData( HousingBlock v_block) 
    {
        this._currBlockData = v_block;
    }
    public void F_SetHousnigIdx(int v_type , int v_detail) 
    {
        this._buildTypeIdx = v_type;
        this._buildDetailIdx = v_detail;
    }

    #region mybuildManager, housing SnapBuild , housing repairDestory
    // Collider�� Trigger OnOff
    public void F_ColliderTriggerOnOff(Transform v_trs, bool v_flag)
    {
        // trs : �� ���� colliderGroup�� �ڽĵ� collider on / off
        for (int i = 0; i < v_trs.childCount; i++) 
        {
            v_trs.GetChild(i).GetComponent<Collider>().isTrigger = v_flag;
        }
    }

    // �θ� �� material ��ȯ
    public void F_ChangeMaterial(Transform v_pa, Material material)
    {
        foreach (MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>())
        {
            msr.material = material;
        }
    }

    // ���̾� �ʱ�ȭ 
    private void F_InitLayer()
    {
        // ��ġ�Ϸ��� ������Ʈ ���̾�
        _placedItemLayerInt = LayerMask.NameToLayer("PlacedItemLayer");

        // ��ġ�Ϸ��� �� ���̾�
        _buildFinishedint   = LayerMask.NameToLayer("BuildFinishedBlock");
        _buildFinishedLayer = LayerMask.GetMask("BuildFinishedBlock");

        // Ŀ���� ���̾� 
        _connectorLayer = new List<Tuple<LayerMask, int>>
        {
            new Tuple <LayerMask , int>( LayerMask.GetMask("FloorConnectorLayer") , LayerMask.NameToLayer("FloorConnectorLayer") ),         // temp floor ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("CellingConnectorLayer") , LayerMask.NameToLayer("CellingConnectorLayer") ),     // temp celling ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("WallConnectorLayer") , LayerMask.NameToLayer("WallConnectorLayer") ),           // temp wall ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("WallConnectorLayer") , LayerMask.NameToLayer("WallConnectorLayer") ),           // temp wall ���̾� ( destory ���� ���ؼ� )
            
        };

        // Ŀ���� �� ��ģ ���̾� 
        _ConnectorWholelayer = _connectorLayer[0].Item1 | _connectorLayer[1].Item1 | _connectorLayer[2].Item1;

    }

    // MyModelBlock ��ũ��Ʈ ���� 
    public void F_DestoryMyModelBlockUnderParent(Transform v_parent)    // parent : block �� model 
    {
        for (int i = 0; i < v_parent.childCount; i++) 
        {
            // ������ �н� 
            if (v_parent.GetChild(i).GetComponent<MyModelBlock>() == null)
                continue;

            // ����
            Destroy( v_parent.GetChild(i).GetComponent<MyModelBlock>() );
        }
        
    }



    #endregion

    #region SaveManager

    // ������ ������ ���� ��, �ʱ⿡ ���� 
    public void F_FirstInitBaseFloor()
    {
        int _tempTypeIdx = 0;
        int _tempDetailIdx = 0;
        Vector3 _buildVec = Vector3.zero;

        // 1. floor ���� 
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                _buildVec = new Vector3(j * 5, 0, i * -5);

                // 1. �⺻�� ���� 
                GameObject _nowbuild = Instantiate(myBuildManger.F_GetCurBuild(_tempTypeIdx, _tempDetailIdx), _buildVec, Quaternion.identity, _parentTransform);
            }
        }

        // 2. Ŀ���� ������Ʈ 
        // 2-1. �ʱ� N�� ���� ���� Ŀ���� ������Ʈ ( parentTransform�� childCount�� �ϸ� ��Ӵþ�� ���ѷ��� )
        for (int i = 0; i < 9; i++)
        {
            // 2-1-2. Ŀ���� create 
            BuildMaster.Instance.housingRepairDestroy.F_CreateConnector((SelectedBuildType)0, _parentTransform.GetChild(i));
        }

    }

    // ���̺� �ҷ��� ��, MyBuldingBlock�� �ʵ� ä���ֱ�
    public void F_CreateBlockFromSave(int _t, int _d, Vector3 _trs, Vector3 _ro, int _h, int _maxhp)
    {
        // Ÿ�� �ε���, �������ε���, ��ġ, ȸ��, hp , �ִ� hp

        // 1. ���� ,  �θ����� 
        GameObject _nowbuild = default ;

        // �ε����� �����̸� ? -> Connector 
        if (_t < 0)
        {
            _nowbuild = Instantiate(housingRepairDestroy._connectorObject, _trs, Quaternion.identity, _parentTransform);
            _nowbuild.layer = _connectorLayer[(_t * - 1) - 1].Item2;
        }
        else
        {
            // blcok �⺻���̾� : BuildFInishedLayer
            _nowbuild = Instantiate(myBuildManger.F_GetCurBuild(_t, _d), _trs, Quaternion.identity, _parentTransform);

            // 1-1. ���� ���� ������Ʈ �� myBuildelBlock ���� 
            F_DestoryMyModelBlockUnderParent( _nowbuild.transform.GetChild(0) );

        }

        // 2. �� ȸ�� ����
        Quaternion _qu = Quaternion.Euler(_ro);
        _nowbuild.transform.rotation = _qu;

        // 4.myBuildingBlock�� ��ũ��Ʈ�� ������������
        if (_nowbuild.GetComponent<MyBuildingBlock>() == null)
            _nowbuild.AddComponent<MyBuildingBlock>();

        // 4-1. block�� �ʵ�
        MyBuildingBlock _tmpBlock = _nowbuild.GetComponent<MyBuildingBlock>();

        // 4-2. �ʵ� ����
        int _hp = _h;
        if (_trs == Vector3.zero)       // 0,0,0 ��ġ���� hp��  100 
            _hp = 9999;
        _tmpBlock.F_SetBlockFeild(_t, _d, _hp, _hp);

    }
    #endregion

}
