using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

[System.Serializable]
public enum MySelectedBuildType
{
    Floor,
    Celling,
    Wall,
    Door,
    Window,
    RepairTools
}

public class MyBuildManager : MonoBehaviour
{
    [Header("===Player===")]
    public GameObject _player;

    [Header("===Block Parent===")]
    [SerializeField] Transform _parentTransform;

    [Header("===Type===")]
    [HideInInspector] private MySelectedBuildType _mySelectBuildType;

    #region prefab
    [SerializeField] public List<List<GameObject>> _bundleBulingPrefab;

    [Header("===Build Object===")]
    [SerializeField] private List<GameObject> _floorList;
    [SerializeField] private List<GameObject> _cellingList;
    [SerializeField] private List<GameObject> _wallList;
    [SerializeField] private List<GameObject> _doorList;
    [SerializeField] private List<GameObject> _windowList;
    #endregion

    #region LayerMask
    [Header("===LayerMask===")]
    [SerializeField] LayerMask _nowTempLayer;               // �׷��� ���� ���̾�
    private LayerMask _buildFinishedLayer;                  // �� ���� ������ layermask   
    private int _buildFinishedint;                          // �� ���� ������ layer int
    private int _dontRaycastInt;                            // temp ��ġ �߿� temp block�� �浹���� ����
    private List<Tuple<LayerMask, int>> _tempUnderBlockLayer;   // �ӽ� ���� ���̾�
                                                                         // 0. Temp floor ���̾�
                                                                         // 1. Temp celling ���̾�
                                                                         // 2. Temp wall ���̾�
    [HideInInspector] public LayerMask _tempWholeLayer;     //  temp floor , celling, wall ���̾� �� ��ģ
    #endregion

    #region condition
    private int _buildTypeIdx;                              // ���� Ÿ������
    private int _buildDetailIdx;                            // �� Ÿ�Ծ� ���° ������Ʈ ����

    [Header("===Temp Object Setting===")]
    [SerializeField] bool _isTempValidPosition;             // �ӽ� ������Ʈ�� ������ �� �ִ���
    [SerializeField] bool _isEnoughResource;                // ��ġ�ϱ⿡ ��ᰡ �������?
    [SerializeField] bool _isntColliderOther;               // temp wall �� �ٸ� ������Ʈ�� �浹�� ��������? ( false�� �浹 )
    #endregion

    #region temp object Trs
    [HideInInspector] GameObject _TempObjectBuilding;        // �ӽ� ������Ʈ
    [HideInInspector] List<Transform> _tempUnderParentTrs;   // �ӽ� ������Ʈ ���� �� �θ� trs
                                                            // 0. �ӽ� ������Ʈ model �θ�
                                                            // 1. `` tempFloor �θ�
                                                            // 2. `` celling �θ�
                                                            // 3. `` tempWall �θ�
                                                            // 4. (wall �϶���) `` ladder �θ� 
    [HideInInspector] Transform _otherConnectorTr;       // �浹�� �ٸ� ������Ʈ 
    #endregion

    #region Material
    [Header("===Material===")]
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;
    [HideInInspector] Material _oriMaterial;
    [HideInInspector] Material _nowBuildMaterial;
    #endregion

    // ������Ƽ
    public int BuildFinishedLayer { get => _buildFinishedint; }
    public bool IsntColliderOther { get => _isntColliderOther; set { _isntColliderOther = value; } }

    // =============================================
    private void Awake()
    {
        SaveManager.Instance.GameDataSave += () => SaveManager.Instance.F_SaveBuilding(_parentTransform.transform);

        F_InitLayer();              // ���̾� �ʱ�ȭ
        F_InitBundleBlock();        // ���� prefab �� list�ϳ��� �ʱ�ȭ

        _isTempValidPosition = true;
        _isEnoughResource    = false;
        _isntColliderOther   = true;  // �ٸ� ������Ʈ�� �浹�Ǿ��ִ°�?

        // ## TODO ������
        SaveManager.Instance.F_LoadBuilding(_parentTransform);
    }

    #region 1. ���̾� �ʱ�ȭ 2. ������ list

    private void F_InitLayer()
    {
        _dontRaycastInt         = LayerMask.NameToLayer("DontRaycastSphere");
        _buildFinishedint       = LayerMask.NameToLayer("BuildFinishedBlock");
        _buildFinishedLayer     = LayerMask.GetMask("BuildFinishedBlock");

        _tempUnderBlockLayer = new List<Tuple<LayerMask, int>>
        {
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempFloorLayer") , 17 ),        // temp floor ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempCellingLayer") , 16 ),      // temp celling ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempWallLayer") , 15 ),         // temp wall ���̾�
            
        };

        _tempWholeLayer = _tempUnderBlockLayer[0].Item1 | _tempUnderBlockLayer[1].Item1 | _tempUnderBlockLayer[2].Item1;

    }

    private void F_InitBundleBlock()
    {
        // �� block ������ List�� �ϳ��� List�� ����
        _bundleBulingPrefab = new List<List<GameObject>>
        {
            _floorList,
            _cellingList,
            _wallList,
            _doorList,
            _windowList
        };
    }
    #endregion

    public void F_GetbuildType(int v_type = 0, int v_detail = 1)
    {
        // 0. Ui�� �߸��� idx�� �Ѿ���� �� 
        if (v_type < 0 && v_detail < 0)
            return;

        // 1. index �ʱ�ȭ
        _buildTypeIdx = v_type;
        _buildDetailIdx = v_detail;

        // 2. �ӽ� ������Ʈ Ȯ��
        if (_TempObjectBuilding != null)
            Destroy(_TempObjectBuilding);
        _TempObjectBuilding = null;

        // 3. building check �ʱ�ȭ
        BuildMaster.Instance.mybuildCheck.F_BuildingStart();

        // 4. ���� ���� 
        StopAllCoroutines();
        StartCoroutine(F_TempBuild());
    }

    IEnumerator F_TempBuild()
    {
        // 0. index�� �ش��ϴ� ���� ������Ʈ return
        GameObject _currBuild = F_GetCurBuild(_buildTypeIdx, _buildDetailIdx);
        // 0.1. �� ���� Ÿ�Կ� ���� �˻��� layer ���ϱ�
        F_TempRaySetting(_mySelectBuildType);
        // 0.2. type�� ���� progress on off ui ����
        F_OnOffProgressUI();

        while (true)
        {
            // �������� type�� �ı����� �� ��
            if (_mySelectBuildType == MySelectedBuildType.RepairTools)
            {
                F_RepairAndDestroyTool();
            }
            // ��������, build type �� �� 
            else
            {
                F_OtherBuildBlockBuild(_currBuild);
            }

            // update ȿ�� 
            yield return null;
        }
    }


    #region ray , snap ����
    private void F_OtherBuildBlockBuild(GameObject v_build) 
    {
        // 1. index�� �ش��ϴ� ���ӿ�����Ʈ ����
        F_CreateTempPrefab(v_build);

        // 2. �ش� �����̶� ���� Layer�� raycast
        F_Raycast(_nowTempLayer);

        // 3. temp ������Ʈ�� �ݶ��̴� �˻�
        F_CheckCollision();

        // 4. ��ġ
        if (Input.GetMouseButtonDown(0))
            F_FinishBuild();
    }


    private void F_OnOffProgressUI() 
    {
        // 0. repair type �� destroy���̸� progressUI ���� 
        if (_mySelectBuildType == MySelectedBuildType.RepairTools && _buildDetailIdx == 1)
            BuildMaster.Instance.housingUiManager.F_OnOFfBuildingProgressUi(false);
        // 1. �ƴϸ� �ѱ�
        else
            BuildMaster.Instance.housingUiManager.F_OnOFfBuildingProgressUi(true);

    }

    private void F_Raycast(LayerMask v_layer)
    {
        // �Ѿ�� Layer�� ���� raycast
        RaycastHit _hit;

        if (Physics.Raycast(_player.transform.position, _player.transform.forward * 10, out _hit, 5f, v_layer)) // Ÿ�� : LayerMask
        {
            _TempObjectBuilding.transform.position = _hit.point;
        }
    }

    private void F_CheckCollision()
    {
        Collider[] _coll = Physics.OverlapSphere(_TempObjectBuilding.transform.position, 1f, _nowTempLayer);    // Ÿ�� : LayerMask

        if (_coll.Length > 0)
            F_Snap(_coll);
        else
            _isTempValidPosition = false;

    }

    private void F_Snap(Collider[] v_coll)
    {
        _otherConnectorTr = null;

        for (int i = 0; i < v_coll.Length; i++)
        {

            if (v_coll[i].GetComponent<MyConnector>()._canConnect == true)
            {
                _otherConnectorTr = v_coll[i].transform;
                break;
            }
        }

        // ��ġ ������ ��ġ�� ������
        if (_otherConnectorTr == null)
        {
            F_ChangeMesh(_tempUnderParentTrs[0], false);
            _isTempValidPosition = false;
            return;
        }

        // Ÿ���� wall �϶� ȸ�� 
        if (_mySelectBuildType == MySelectedBuildType.Wall || _mySelectBuildType == MySelectedBuildType.Window
            || _mySelectBuildType == MySelectedBuildType.Door ) 
        {
            // �� temp ���� ȸ�� += ������ Ŀ������ ȸ��
            Quaternion qu = _TempObjectBuilding.transform.rotation;
            qu.eulerAngles = new Vector3(qu.eulerAngles.x, _otherConnectorTr.eulerAngles.y , qu.eulerAngles.z);
            _TempObjectBuilding.transform.rotation = qu;
        }

        // ��ġ����
        _TempObjectBuilding.transform.position
             = _otherConnectorTr.position;

        // mesh �ѱ�
        F_ChangeMesh(_tempUnderParentTrs[0], true);
        // ��ġ���� 
        _isTempValidPosition = true;
    }

    public GameObject F_GetCurBuild( int v_type , int v_detail) 
    {
        switch (v_type)
        {
            case 0:
                _mySelectBuildType = MySelectedBuildType.Floor;
                return _floorList[v_detail];
            case 1:
                _mySelectBuildType = MySelectedBuildType.Celling; 
                return _cellingList[v_detail];
            case 2:
                _mySelectBuildType = MySelectedBuildType.Wall;
                return _wallList[v_detail];
            case 3:
                _mySelectBuildType = MySelectedBuildType.Door;
                return _doorList[v_detail];
            case 4:
                _mySelectBuildType = MySelectedBuildType.Window;
                return _windowList[v_detail];
            default:
                _mySelectBuildType = MySelectedBuildType.RepairTools;
                return null;
        }
    }

    private void F_TempRaySetting(MySelectedBuildType v_type)
    {
        switch (v_type)
        {
            case MySelectedBuildType.Floor:
                _nowTempLayer = _tempUnderBlockLayer[0].Item1;          // floor ���̾�
                break;
            case MySelectedBuildType.Celling:
                _nowTempLayer = _tempUnderBlockLayer[1].Item1;          // celling ���̾�
                break;
            case MySelectedBuildType.Wall:                              // window, door, window�� ���� wall ���̾� ��� 
            case MySelectedBuildType.Door:
            case MySelectedBuildType.Window:
                _nowTempLayer = _tempUnderBlockLayer[2].Item1;          // wall ���̾�
                break;
            case MySelectedBuildType.RepairTools:                       // repair �� �� �� 
                _nowTempLayer = _buildFinishedLayer;                    // �� ���� ������ layer
                break;
        }

    }


    private void F_CreateTempPrefab( GameObject v_temp) 
    {
        // ��������
        // temp������Ʈ�� null�̵Ǹ� �ٷ� ������!
        if (_TempObjectBuilding == null)
        {
            // 1. ����
            _TempObjectBuilding = Instantiate( v_temp , new Vector3(100f,100f,100f) , Quaternion.identity );

            // 2. �θ� trs �ʱ�ȭ
            _tempUnderParentTrs = new List<Transform>
            {
                _TempObjectBuilding.transform.GetChild(0),      // model parent
                _TempObjectBuilding.transform.GetChild(1),      // floor parent
                _TempObjectBuilding.transform.GetChild(2),      // celling parent
                _TempObjectBuilding.transform.GetChild(3),      // wall parent
            };

            // 2. �� Parent ���� ������Ʈ�� ���̾� �ٲٱ�
            for (int i = 1; i < _tempUnderParentTrs.Count; i++) 
            {
                F_ChangeLayer(_tempUnderParentTrs[i], _dontRaycastInt);        
            }

            // 2-1. moel�� is Trigger �ѱ� 
            F_OnCollision(_tempUnderParentTrs[0], true);

            // 3. ���� material ����
            _oriMaterial = _tempUnderParentTrs[0].GetChild(0).GetComponent<MeshRenderer>().material;

            // 4. mybuilding check�� ���۽���
            F_BuldingInitCheckBuild();

            // 5. modeld�� Material �ٲٱ�
            F_ChangeMaterial(_tempUnderParentTrs[0], _nowBuildMaterial);
        }
    }

    private void F_BuldingInitCheckBuild() 
    {
        // 1. �ʱ�ȭ
        BuildMaster.Instance.mybuildCheck.F_BuildingStart();

        // 2. ��ᰡ �������? ����ϸ� true,  �ƴϸ� false
        _isEnoughResource = BuildMaster.Instance.mybuildCheck.F_WholeSourseIsEnough();

        // 3. �����е��� ���� material ��ȭ
        if (_isEnoughResource)
            _nowBuildMaterial = _greenMaterial;
        else
            _nowBuildMaterial = _redMaterial;
    }

    #endregion

    #region ���� & �ı�����

    // ���� & �ı����� ���� 
    private void F_RepairAndDestroyTool() 
    {
        // 0. ��Ŭ�� ���� ��
        if (Input.GetMouseButtonDown(0)) 
        {
            // 1. ray ���� finished ������ ������
            RaycastHit _hit;
            if (Physics.Raycast(_player.transform.position, _player.transform.forward * 10, out _hit, 5f, _nowTempLayer))   // Ÿ�� : LayerMask
            {
                // 1. myBlock �������� ( �浹�� model�� �θ���, ~block ��ũ��Ʈ )
                MyBuildingBlock my = _hit.collider.gameObject.transform.parent.GetComponent<MyBuildingBlock>();

                // 2. repair ����
                if (_buildDetailIdx == 0)
                    F_RepairTool(my);
                // 3. destroy ����
                else
                    F_DestroyTool(my);
            }
        }

    }

    private void F_DestroyTool(MyBuildingBlock v_mb )
    {
        // 3-1. ray�� ������ block ��ũ��Ʈ�� Ŀ���� update,  _canConnect �� true�� 
        v_mb.F_BlockCollisionConnector(true);

        // 3-2. destory
        Destroy(v_mb.gameObject);

    }

    private void F_RepairTool( MyBuildingBlock v_mb ) 
    {
        // 1. ��ᰡ ����ϸ�?
        if (BuildMaster.Instance.mybuildCheck.F_WholeSourseIsEnough())
        {
            // 1. max ���� ������ , 1 ����
            if (v_mb.MyBlockMaxHp > v_mb.MyBlockHp)
            {
                // 1-1. �κ��丮 ������Ʈ (��� �Ҹ�)
                BuildMaster.Instance.mybuildCheck.F_UpdateInvenToBuilding();

                // 1-2. ������� , ui ������Ʈ 
                BuildMaster.Instance.mybuildCheck.F_BuildingStart();

                // 1-2. 1����
                v_mb.MyBlockHp += 1;

            }
            else
                return;
        }
        else
            return;
    }
    #endregion

    #region building ���� �� (��ġ)

    private void F_FinishBuild() 
    {
        // 1. �κ� �� ��ᰡ ����ϸ� , �ùٸ� ��ġ�� ������, �ٸ� ������Ʈ�� �浹�� ���°� �ƴϸ� 
        if (_isEnoughResource == true &&  _isTempValidPosition == true && _isntColliderOther == true )
        {
            // 2. ����
            F_BuildTemp();

            // 0. �÷��̾� �ִϸ��̼� ���� 
            PlayerManager.Instance.PlayerController.F_CreateMotion();

            // 3. �κ��丮 ������Ʈ
            BuildMaster.Instance.mybuildCheck.F_UpdateInvenToBuilding();
        }
        else
            return;
    }
    
    private void F_BuildTemp() 
    { 
        if( _TempObjectBuilding != null) 
        { 
            // 0. ����
            GameObject _nowbuild = Instantiate(F_GetCurBuild(_buildTypeIdx, _buildDetailIdx), _TempObjectBuilding.transform.position , _TempObjectBuilding.transform.rotation , _parentTransform);

            // 1. destory
            Destroy( _TempObjectBuilding);
            _TempObjectBuilding = null;

            // 3. model�� material ����
            F_ChangeMaterial(_nowbuild.transform.GetChild(0) , _oriMaterial);

            // 4. model�� layer (buildFinished��) ����
            F_ChangeLayer( _nowbuild.transform.GetChild(0) , _buildFinishedint , true );

            // 4-1. model�� �ݶ��̴��� is trigger üũ ����
            if (_mySelectBuildType == MySelectedBuildType.Door)
                F_OnCollision(_nowbuild.transform.GetChild(0), true);
            else
                F_OnCollision(_nowbuild.transform.GetChild(0), false);

            // 4-2. model�� MyModelBlock�� ������ false�� ��ȯ
            F_ChagneMyModelBlock(_nowbuild.transform.GetChild(0) , true);

            // 5. �� ������Ʈ�� �´� temp ���̾�� ��ȯ
            for (int i = 1; i < _nowbuild.transform.childCount - 1; i++) 
            {
                F_ChangeLayer( _nowbuild.transform.GetChild(i) , _tempUnderBlockLayer[i-1].Item2 );   
            }

            // 6. ���� ���θ��� block Connector ������Ʈ
            F_ConeectorUpdate(_nowbuild.transform);

            // 8. ���� ���� ���� block�� MyBuildingBlock �߰�
            if (_nowbuild.GetComponent<MyBuildingBlock>() == null)
            {
                // null �̸� �߰��ϱ� 
                _nowbuild.AddComponent<MyBuildingBlock>();
            }

            // 9. nowBuild�� �浹�� Ŀ���͵� ������Ʈ
            MyBuildingBlock _nowBuildBlock = _nowbuild.GetComponent<MyBuildingBlock>();
            _nowBuildBlock.F_BlockCollisionConnector( false );

            // 8-1. block�� �ʵ� �ʱ�ȭ 
            _nowBuildBlock.F_SetBlockFeild(_buildTypeIdx, _buildDetailIdx % 10, 
                BuildMaster.Instance.mybuildCheck._myblock.BlockHp,
                BuildMaster.Instance.mybuildCheck._myblock.BlockMaxHp);

        }
    }

    // Ŀ���� ������Ʈ 
    public void F_ConeectorUpdate( Transform v_pa ) 
    {
        for (int i = 1; i <= 3; i ++) 
        {
            foreach (MyConnector mc in v_pa.transform.GetChild(i).GetComponentsInChildren<MyConnector>())
            {
                mc.F_UpdateConnector(); 
            }
        }
    }

    #endregion

    #region saveManager 

    // ������ ��ġ������ �����ϴ� model������ MyModelBlock�� ����
    public void F_ModelComplte() 
    {
        // ������Ʈ ����, model ���� ������Ʈ�� MyModelBlock�� ��ũ��Ʈ�� ����
        for (int i = 0; i < _parentTransform.childCount; i++) 
        {
            Transform _model = _parentTransform.GetChild(i);

            foreach (MyModelBlock mb in _model.GetComponentsInChildren<MyModelBlock>())
                mb.isModelBuild = true;
        }
    }

    // ������ ������ ���� ��, �ʱ⿡ ���� 
    public void F_FirstInitBaseFloor() 
    {
        _buildTypeIdx = 0;
        _buildDetailIdx = 0;

        Vector3 _buildVec = Vector3.zero;
        for (int i = 0; i < 3; i++) 
        {
            for (int j = 0; j < 3; j++) 
            {
                _buildVec = new Vector3( j * 5 , 0 , i * -5);
                GameObject _nowbuild = Instantiate( F_GetCurBuild( _buildTypeIdx , _buildDetailIdx ), _buildVec, Quaternion.identity , _parentTransform);
            }
        }
    }

    // ���̺� �ҷ��� ��, MyBuldingBlock�� �ʵ� ä���ֱ�
    public void F_CreateBlockFromSave(int _t , int _d , Vector3 _trs , Vector3 _ro , int _h , int _maxhp) 
    {
        // Ÿ�� �ε���, �������ε���, ��ġ, ȸ��, hp , �ִ� hp

        // ���� ,  �θ����� 
        GameObject _nowbuild = Instantiate(F_GetCurBuild(_t, _d), _trs, Quaternion.identity, _parentTransform);
        // �� ȸ�� ����
        Quaternion _qu = Quaternion.Euler(_ro);
        _nowbuild.transform.rotation = _qu;

        // myBuildingBlock�� ��ũ��Ʈ�� ������������
        if( _nowbuild.GetComponent<MyBuildingBlock>() == null)
            _nowbuild.AddComponent<MyBuildingBlock>();
        
        // 3-5. block�� �ʵ�
        MyBuildingBlock _tmpBlock = _nowbuild.GetComponent<MyBuildingBlock>();
        // �ʵ� ����
        _tmpBlock.F_SetBlockFeild( _t, _d, _h , _maxhp );

    }

    // ���� ������ ���鼭, ���� ������Ʈ 
    public void F_UpdateWholeBlock() 
    {
        for (int i = 0; i < _parentTransform.childCount; i++) 
        {
            MyBuildingBlock _myblo = _parentTransform.GetChild(i).GetComponent<MyBuildingBlock>();
            _myblo.F_BlockCollisionConnector( false );
        }
    }

    #endregion

    #region chagneEct
    
    private void F_ChagneMyModelBlock( Transform v_parnet, bool v_Flag)  
    {
        // model ���� MyModelBlock�� �����ؼ� bool ��ȯ 
        foreach (MyModelBlock my in v_parnet.GetComponentsInChildren<MyModelBlock>())
        {
            my.isModelBuild = true;
        }
    }

    private void F_OnCollision( Transform v_trs , bool v_flag) 
    {
        v_trs.GetComponent<Collider>().isTrigger = v_flag;
    }

    private void F_ChangeMaterial( Transform v_pa , Material material ) 
    {
        foreach ( MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>()) 
        {
            msr.material = material;
        }
    }

    private void F_ChangeMesh( Transform v_pa , bool v_flag) 
    {
        foreach (MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>())
        {
            msr.enabled = v_flag;
        }
    }

    // ���̾� ����
    // �θ� ���̾� ���� �� v_flag�� true
    private void F_ChangeLayer( Transform v_pa , int v_layer , bool v_flag = false )    // ���ӿ�����Ʈ�� ���̾ �ٲ� ���� int ������
    {
        if (v_flag)
            v_pa.gameObject.layer = v_layer;

        else 
        {
            for (int i = 0; i < v_pa.childCount; i++)
            {
                v_pa.GetChild(i).gameObject.layer = v_layer;
            }
        }
    }
    #endregion

    #region Player Controller
    
    // �Ǽ� ���� ������ �� �ʱ�ȭ �Լ� 
    public void F_InitBuildngMode() 
    {
        // 0. ���� �����ϰ� �ִ� building �ڷ�ƾ ���� 
        StopAllCoroutines();

        // 1. buildingProgressUi ����
        BuildMaster.Instance.housingUiManager._buildingProgressUi.gameObject.SetActive(false);

        // 2. Ȥ�ó� �������� housing ui�� ���� 
        //HousingUiManager.Instance._buildingCanvas.gameObject.SetActive(false);

        // 3. preview ������Ʈ ����
        if (_TempObjectBuilding != null)
            Destroy(_TempObjectBuilding);
        _TempObjectBuilding = null;

    }



    #endregion

    #region MyModelBlock
    public void F_IsntCollChagneMaterail( int v_num ) 
    { 
        switch (v_num) 
        {
            // �ʷϻ����� model ��ȭ
            case 0:
                F_ChangeMaterial(_tempUnderParentTrs[0] , _greenMaterial );
                break;
            // ���������� model ��ȭ
            case 1:
                F_ChangeMaterial(_tempUnderParentTrs[0], _redMaterial );
                break;
        }
    }

    #endregion
}