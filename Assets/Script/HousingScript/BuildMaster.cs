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
    // Housing Block과 , Conenctor의 부모 
    public Transform _parentTransform;

    [Header("===Script===")]
    public MyBuildManager myBuildManger;                    // build하는 동작 관리 ( snapbuild & repairDestroy )
    public MyBuildCheck mybuildCheck;                       // build시 재료가 충분한지 & build Process Ui 
    public HousingDataManager housingDataManager;           // csv 데이터 불러오기 , 초기화 
    public HousingUiManager housingUiManager;               // 우클릭시 ui 관리 
    public Housing_SnapBuild housingSnapBuild;
    public Housing_RepairDestroy housingRepairDestroy;

    // 블럭 sprite 
    [Header("===Sprite===")]
    [SerializeField] public List<Sprite> _blockSprite;

    // 현재 짓고 있는 블럭의 데이터 
    [SerializeField] private HousingBlock _currBlockData;

    [Header("===Idx===")]
    public int _buildTypeIdx;                              // 무슨 타입인지
    public int _buildDetailIdx;                            // 그 타입안 몇번째 오브젝트 인지

    // Housing SnapBuild과 Housing repairDestroy에서 공통으로 사용중인 Layer
    [Header("===Layer===")]
    public LayerMask _buildFinishedLayer;                           // 다 지은 블럭의 layermask   
    public int _buildFinishedint;                                   // 다 지은 블럭의 layer int
    public List<Tuple<LayerMask, int>> _connectorLayer;             // 커넥터 레이어 
    [HideInInspector] public LayerMask _ConnectorWholelayer;        // 모든 커넥터 레이어 다 합친
    private int _placedItemLayerInt;                                // 설치 완료한 오브젝트 layer int
               
    // 프로퍼티 
    public HousingBlock currBlockData { get => _currBlockData;  }
    public int placedItemLayerInt { get => _placedItemLayerInt;  }

    protected override void InitManager()
    {
        // 0. 레이어 초기화
        F_InitLayer();

        // 1. savemanager 델리게이트에 저장 
        SaveManager.Instance.GameDataSave += () => SaveManager.Instance.F_SaveBuilding(_parentTransform.transform);
        // 1. Connector 저장
        BuildMaster.Instance.housingDataManager.F_InitConnectorInfo();
        // 1. savemanager에서 불러오기 
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
    // Collider의 Trigger OnOff
    public void F_ColliderTriggerOnOff(Transform v_trs, bool v_flag)
    {
        // trs : 블럭 하위 colliderGroup의 자식들 collider on / off
        for (int i = 0; i < v_trs.childCount; i++) 
        {
            v_trs.GetChild(i).GetComponent<Collider>().isTrigger = v_flag;
        }
    }

    // 부모 밑 material 변환
    public void F_ChangeMaterial(Transform v_pa, Material material)
    {
        foreach (MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>())
        {
            msr.material = material;
        }
    }

    // 레이어 초기화 
    private void F_InitLayer()
    {
        // 설치완료한 오브젝트 레이어
        _placedItemLayerInt = LayerMask.NameToLayer("PlacedItemLayer");

        // 설치완료한 블럭 레이어
        _buildFinishedint   = LayerMask.NameToLayer("BuildFinishedBlock");
        _buildFinishedLayer = LayerMask.GetMask("BuildFinishedBlock");

        // 커넥터 레이어 
        _connectorLayer = new List<Tuple<LayerMask, int>>
        {
            new Tuple <LayerMask , int>( LayerMask.GetMask("FloorConnectorLayer") , LayerMask.NameToLayer("FloorConnectorLayer") ),         // temp floor 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("CellingConnectorLayer") , LayerMask.NameToLayer("CellingConnectorLayer") ),     // temp celling 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("WallConnectorLayer") , LayerMask.NameToLayer("WallConnectorLayer") ),           // temp wall 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("WallConnectorLayer") , LayerMask.NameToLayer("WallConnectorLayer") ),           // temp wall 레이어 ( destory 도구 위해서 )
            
        };

        // 커넥터 다 합친 레이어 
        _ConnectorWholelayer = _connectorLayer[0].Item1 | _connectorLayer[1].Item1 | _connectorLayer[2].Item1;

    }

    // MyModelBlock 스크립트 삭제 
    public void F_DestoryMyModelBlockUnderParent(Transform v_parent)    // parent : block 밑 model 
    {
        for (int i = 0; i < v_parent.childCount; i++) 
        {
            // 없으면 패스 
            if (v_parent.GetChild(i).GetComponent<MyModelBlock>() == null)
                continue;

            // 삭제
            Destroy( v_parent.GetChild(i).GetComponent<MyModelBlock>() );
        }
        
    }



    #endregion

    #region SaveManager

    // 데이터 파일이 없을 때, 초기에 생성 
    public void F_FirstInitBaseFloor()
    {
        int _tempTypeIdx = 0;
        int _tempDetailIdx = 0;
        Vector3 _buildVec = Vector3.zero;

        // 1. floor 생성 
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                _buildVec = new Vector3(j * 5, 0, i * -5);

                // 1. 기본블럭 생성 
                GameObject _nowbuild = Instantiate(myBuildManger.F_GetCurBuild(_tempTypeIdx, _tempDetailIdx), _buildVec, Quaternion.identity, _parentTransform);
            }
        }

        // 2. 커넥터 업데이트 
        // 2-1. 초기 N개 블럭에 대한 커넥터 업데이트 ( parentTransform의 childCount로 하면 계속늘어나서 무한루프 )
        for (int i = 0; i < 9; i++)
        {
            // 2-1-2. 커넥터 create 
            BuildMaster.Instance.housingRepairDestroy.F_CreateConnector((SelectedBuildType)0, _parentTransform.GetChild(i));
        }

    }

    // 세이브 불러온 후, MyBuldingBlock에 필드 채워넣기
    public void F_CreateBlockFromSave(int _t, int _d, Vector3 _trs, Vector3 _ro, int _h, int _maxhp)
    {
        // 타입 인덱스, 디테일인덱스, 위치, 회전, hp , 최대 hp

        // 1. 생성 ,  부모지정 
        GameObject _nowbuild = default ;

        // 인덱스가 음수이면 ? -> Connector 
        if (_t < 0)
        {
            _nowbuild = Instantiate(housingRepairDestroy._connectorObject, _trs, Quaternion.identity, _parentTransform);
            _nowbuild.layer = _connectorLayer[(_t * - 1) - 1].Item2;
        }
        else
        {
            // blcok 기본레이어 : BuildFInishedLayer
            _nowbuild = Instantiate(myBuildManger.F_GetCurBuild(_t, _d), _trs, Quaternion.identity, _parentTransform);

            // 1-1. 새로 지은 오브젝트 밑 myBuildelBlock 삭제 
            F_DestoryMyModelBlockUnderParent( _nowbuild.transform.GetChild(0) );

        }

        // 2. 내 회전 조정
        Quaternion _qu = Quaternion.Euler(_ro);
        _nowbuild.transform.rotation = _qu;

        // 4.myBuildingBlock에 스크립트가 없을수도있음
        if (_nowbuild.GetComponent<MyBuildingBlock>() == null)
            _nowbuild.AddComponent<MyBuildingBlock>();

        // 4-1. block의 필드
        MyBuildingBlock _tmpBlock = _nowbuild.GetComponent<MyBuildingBlock>();

        // 4-2. 필드 세팅
        int _hp = _h;
        if (_trs == Vector3.zero)       // 0,0,0 위치에서 hp는  100 
            _hp = 9999;
        _tmpBlock.F_SetBlockFeild(_t, _d, _hp, _hp);

    }
    #endregion

}
