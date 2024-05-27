using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Progress;

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

public class MyBuildManager : MonoBehaviour
{
    public GameObject v_testobject;
    
    [Header("===Player===")]
    public GameObject _player;

    [Header("===Block Parent===")]
    [SerializeField] Transform _parentTransform;

    [Header("===Type===")]
    [HideInInspector] private SelectedBuildType _SelectBuildType;
    [HideInInspector] private Connector[] _connectorArr;
    [SerializeField] private Connector _currConnector;
    public GameObject _connectorObject;

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
    [SerializeField] LayerMask _currTempLayer;              // 현재 (temp 블럭이 감지할) 레이어 
    private LayerMask _buildFinishedLayer;                  // 다 지은 블럭의 layermask   
    private int _buildFinishedint;                          // 다 지은 블럭의 layer int
    private List<Tuple<LayerMask, int>> _connectorLayer;    // 커넥터 레이어 
                                                                // 0. Temp floor 레이어
                                                                // 1. Temp celling 레이어
                                                                // 2. Temp wall 레이어
    [HideInInspector] public LayerMask _tempWholeLayer;     //  temp floor , celling, wall 레이어 다 합친
    #endregion

    #region condition
    private int _buildTypeIdx;                              // 무슨 타입인지
    private int _buildDetailIdx;                            // 그 타입안 몇번째 오브젝트 인지

    [Header("===Temp Object Setting===")]
    [SerializeField] bool _isTempValidPosition;             // 임시 오브젝트가 지어질 수 있는지
    [SerializeField] bool _isEnoughResource;                // 설치하기에 재료가 충분한지?
    [SerializeField] bool _isntColliderOther;               // temp wall 이 다른 오브젝트랑 충돌한 상태인지? ( false면 충돌 )
    #endregion

    #region temp object Trs
    [HideInInspector] GameObject _tempObjectBuilding;        // 임시 오브젝트
    [HideInInspector] Transform _modelTransform;             // 모델 오브젝트 
    [HideInInspector] Transform _otherConnectorTr;           // 충돌한 다른 오브젝트 
    #endregion

    #region Material
    [Header("===Material===")]
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;
    [HideInInspector] Material _oriMaterial;
    [HideInInspector] Material _nowBuildMaterial;
    #endregion

    #region 블럭 destroy
    private List<Tuple<ConnectorType, Vector3>> _detectConnectorOnDestroyBlock;            // destory시 이 위치에서 감지한 Connector
    private List<Tuple<ConnectorType, Transform>> _detectBuildFinishedBlockOnConnector;      // 위에서 감지한 커넥터의 위치에서 buildFInished 감지 

    #endregion

    // 프로퍼티
    public int BuildFinishedLayer { get => _buildFinishedint; }
    public bool IsntColliderOther { get => _isntColliderOther; set { _isntColliderOther = value; } }

    // =============================================
    private void Awake()
    {
        // 0. savemanager 델리게이트에 저장 
        SaveManager.Instance.GameDataSave += () => SaveManager.Instance.F_SaveBuilding(_parentTransform.transform);

        // 1. 초기화
        _connectorArr = new Connector[System.Enum.GetValues(typeof(ConnectorType)).Length];       // 커넥터 타입만큼 배열 생성
        _detectConnectorOnDestroyBlock = new List<Tuple<ConnectorType, Vector3>>();
        _detectBuildFinishedBlockOnConnector = new List<Tuple<ConnectorType, Transform>>();

        F_InitLayer();              // 레이어 초기화                        
        _bundleBulingPrefab = new List<List<GameObject>> // 각 block 프리팹 List를 하나의 List로 묶기
        {
            _floorList,
            _cellingList,
            _wallList,
            _doorList,
            _windowList
        };

        // 2. 상태 초기화
        _isTempValidPosition = true;
        _isEnoughResource = false;
        _isntColliderOther = true;  // 다른 오브젝트와 충돌되어있는가?

        // 
        BuildMaster.Instance.housingDataManager.F_InitConnectorInfo();

        // 3. SavaManager에서 불러오기 
        SaveManager.Instance.F_LoadBuilding(_parentTransform);
    }

    public void F_SetConnArr(Connector con1, Connector con2, Connector con3, Connector con4)
    {
        _connectorArr[0] = con1;
        _connectorArr[1] = con2;
        _connectorArr[2] = con3;
        _connectorArr[3] = con4;
    }

    private void F_InitLayer()
    {
        _buildFinishedint = LayerMask.NameToLayer("BuildFinishedBlock");
        _buildFinishedLayer = LayerMask.GetMask("BuildFinishedBlock");

        _connectorLayer = new List<Tuple<LayerMask, int>>
        {
            new Tuple <LayerMask , int>( LayerMask.GetMask("FloorConnectorLayer") , LayerMask.NameToLayer("FloorConnectorLayer") ),         // temp floor 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("CellingConnectorLayer") , LayerMask.NameToLayer("CellingConnectorLayer") ),     // temp celling 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("WallConnectorLayer") , LayerMask.NameToLayer("WallConnectorLayer") ),           // temp wall 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("WallConnectorLayer") , LayerMask.NameToLayer("WallConnectorLayer") ),           // temp wall 레이어 ( destory 도구 위해서 )
            
        };

        _tempWholeLayer = _connectorLayer[0].Item1 | _connectorLayer[1].Item1 | _connectorLayer[2].Item1;

    }


    public void F_GetbuildType(int v_type = 0, int v_detail = 1)
    {
        // 0. Ui상 잘못된 idx가 넘어왔을 때 
        if (v_type < 0 && v_detail < 0)
            return;

        // 1. index 초기화
        _buildTypeIdx = v_type;
        _buildDetailIdx = v_detail;

        // 2. 임시 오브젝트 확인
        if (_tempObjectBuilding != null)
            Destroy(_tempObjectBuilding);
        _tempObjectBuilding = null;

        // 3. building check 초기화
        BuildMaster.Instance.mybuildCheck.F_BuildingStart();

        // 4. 동작 시작 
        StopAllCoroutines();
        StartCoroutine(F_TempBuild());
    }

    IEnumerator F_TempBuild()
    {
        // 0. index에 해당하는 게임 오브젝트 return
        GameObject _currBuild = F_GetCurBuild(_buildTypeIdx, _buildDetailIdx);
        // 0.1. 내 블럭 타입에 따라 검사할 layer , Conneector 정하기 
        F_SettingCurrLayer(_SelectBuildType);

        // 0.2. type에 따라 progress on off ui 설정
        F_OnOffProgressUI();

        while (true)
        {
            // 수리도구 type의 파괴도구 일 때
            if (_SelectBuildType == SelectedBuildType.RepairTools)
            {
                F_RepairAndDestroyTool();
            }
            // 수리도구, build type 일 때 
            else
            {
                F_OtherBuildBlockBuild(_currBuild);
            }

            // update 효과 
            yield return null;
        }
    }


    #region ray , snap 동작
    private void F_OtherBuildBlockBuild(GameObject v_build)
    {
        // 1. index에 해당하는 게임오브젝트 생성
        F_CreateTempPrefab(v_build);

        // 2. 해당 블럭이랑 같은 Layer만 raycast
        F_Raycast(_currTempLayer);

        // 3. temp 오브젝트의 콜라이더 검사
        F_CheckCollision(_currTempLayer);

        // 4. 설치
        if (Input.GetMouseButtonDown(0))
            F_FinishBuild();
    }


    private void F_OnOffProgressUI()
    {
        // 0. repair type 의 destroy툴이면 progressUI 끄기 
        if (_SelectBuildType == SelectedBuildType.RepairTools && _buildDetailIdx == 1)
            BuildMaster.Instance.housingUiManager.F_OnOFfBuildingProgressUi(false);
        // 1. 아니면 켜기
        else
            BuildMaster.Instance.housingUiManager.F_OnOFfBuildingProgressUi(true);

    }

    private void F_Raycast(LayerMask v_layer)
    {
        // 1. 넘어온 Layer'만' rayCast
        RaycastHit _hit;

        // ##TODO 여기서 playermanager의 trasform을 가져오는게 낫나? 근데 그러면 매프레임 가져와야함 
        // 2. raycast 되면 -> 임시 오브젝트를 그 위치로 옮기기 
        if (Physics.Raycast(_player.transform.position, _player.transform.forward * 10, out _hit, 5f, v_layer)) // 타입 : LayerMask
        {
            _tempObjectBuilding.transform.position = _hit.point;
        }
    }

    private void F_CheckCollision(LayerMask v_layer)
    {
        // 1. 콜라이더 검사 
        Collider[] _coll = Physics.OverlapSphere(_tempObjectBuilding.transform.position, 1f, v_layer);    // 타입 : LayerMask

        // 2. 검사되면 -> 오브젝트 Snap
        if (_coll.Length > 0)
            F_Snap(_coll);
        else
            _isTempValidPosition = false;

    }

    private void F_Snap(Collider[] v_coll)
    {
        // 0. 다른 커넥터? -> 배열에 처음으로 들어온 collider
        _otherConnectorTr = v_coll[0].transform;

        // 1. 타입이 wall 일때는 회전 
        if (_SelectBuildType == SelectedBuildType.Wall || _SelectBuildType == SelectedBuildType.Window
            || _SelectBuildType == SelectedBuildType.Door)
        {
            // 내 temp 블럭 회전 += 접촉한 커넥터의 회전
            Quaternion qu = _tempObjectBuilding.transform.rotation;
            qu.eulerAngles = new Vector3(qu.eulerAngles.x, _otherConnectorTr.eulerAngles.y, qu.eulerAngles.z);
            _tempObjectBuilding.transform.rotation = qu;
        }

        // 2. Snap!! 
        _tempObjectBuilding.transform.position
             = _otherConnectorTr.position;

        // 3. mesh 켜기
        F_MeshOnOff(_modelTransform, true);

        // 4. 설치가능 
        _isTempValidPosition = true;
    }

    public GameObject F_GetCurBuild(int v_type, int v_detail)
    {
        switch (v_type)
        {
            case 0:
                _SelectBuildType = SelectedBuildType.Floor;
                return _floorList[v_detail];
            case 1:
                _SelectBuildType = SelectedBuildType.Celling;
                return _cellingList[v_detail];
            case 2:
                _SelectBuildType = SelectedBuildType.Wall;
                return _wallList[v_detail];
            case 3:
                _SelectBuildType = SelectedBuildType.Door;
                return _doorList[v_detail];
            case 4:
                _SelectBuildType = SelectedBuildType.Window;
                return _windowList[v_detail];
            default:
                _SelectBuildType = SelectedBuildType.RepairTools;
                return null;
        }
    }

    private void F_SettingCurrLayer(SelectedBuildType v_type)
    {
        switch (v_type)
        {
            case SelectedBuildType.Floor:
                _currTempLayer = _connectorLayer[0].Item1;                          // floor 레이어
                break;
            case SelectedBuildType.Celling:
                _currTempLayer = _connectorLayer[1].Item1;                          // celling 레이어
                break;
            case SelectedBuildType.Wall:                                                 // window, door, window는 같은 wall 레이어 사용 
            case SelectedBuildType.Door:
            case SelectedBuildType.Window:
                _currTempLayer = _connectorLayer[2].Item1;                          // wall 레이어
                break;
            case SelectedBuildType.RepairTools:                                          // repair 툴 일 때 
                _currTempLayer = _buildFinishedLayer;                                    // buildFinished
                break;
        }
    }

    public void F_SettingConnectorType(SelectedBuildType v_type , Transform v_otherConnector)
    {
        switch (v_type)
        {
            case SelectedBuildType.Floor:
                _currConnector = _connectorArr[0];      // floor 커넥터  
                break;
            case SelectedBuildType.Celling:
                _currConnector = _connectorArr[1];     // celling 커넥터 
                break;
            case SelectedBuildType.Wall:                                                 // window, door, window는 같은 wall 레이어 사용 
            case SelectedBuildType.Door:
            case SelectedBuildType.Window:
                if(v_otherConnector.rotation.y != 0)  // 회전값이 있으면?
                    _currConnector = _connectorArr[3];                                      // 회전 0 wall 커넥터
                else
                    _currConnector = _connectorArr[2];                                      // 회전 x wall 커넥터
            
                break;
            case SelectedBuildType.RepairTools:                                             // repair 툴 일 때 
                break;
        }
    }


    private void F_CreateTempPrefab(GameObject v_temp)
    {
        // 실행조건
        // temp오브젝트가 null이되면 바로 생성됨!
        if (_tempObjectBuilding == null)
        {
            // 1. 생성 & 100f,100f,100f는 임시위치 
            _tempObjectBuilding = Instantiate(v_temp, new Vector3(100f, 100f, 100f), Quaternion.identity);

            // 2. model Transform 
            _modelTransform = _tempObjectBuilding.transform.GetChild(0);

            // 2-1. moel의 is Trigger 켜기 
            F_ColliderTriggerOnOff(_modelTransform, true);

            // 3. 원래 material 저장
            _oriMaterial = _modelTransform.GetChild(0).GetComponent<MeshRenderer>().material;

            // 4. mybuilding check의 동작실행
            F_BuldingInitCheckBuild();

            // 5. modeld의 Material 바꾸기
            F_ChangeMaterial(_modelTransform, _nowBuildMaterial);
        }
    }

    private void F_BuldingInitCheckBuild()
    {
        // 1. 초기화
        BuildMaster.Instance.mybuildCheck.F_BuildingStart();

        // 2. 재료가 충분한지? 충분하면 true,  아니면 false
        _isEnoughResource = BuildMaster.Instance.mybuildCheck.F_WholeSourseIsEnough();

        // 3. 재료충분도에 따른 material 변화
        if (_isEnoughResource)
            _nowBuildMaterial = _greenMaterial;
        else
            _nowBuildMaterial = _redMaterial;
    }

    #endregion

    #region 수리 & 파괴도구

    // 수리 & 파괴도구 동작 
    private void F_RepairAndDestroyTool()
    {
        // 0. 우클릭 했을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 1. ray 쏴서 finished 블럭이 잡히면
            RaycastHit _hit;
            if (Physics.Raycast(_player.transform.position, _player.transform.forward * 10, out _hit, 5f, _currTempLayer))   // 타입 : LayerMask
            {
                // 1. myBlock 가져오기 ( 충돌한 buildFinished 오브젝트의 부모의, mybuildingBlock 스크립트 )
                MyBuildingBlock my = _hit.collider.gameObject.transform.parent.GetComponent<MyBuildingBlock>();

                // 2. repair 도구
                if (_buildDetailIdx == 0)
                    F_RepairTool(my);
                // 3. destroy 도구
                else
                    F_DestroyTool(my);
            }
        }

    }

    private void F_DestroyTool(MyBuildingBlock v_mb)
    {
        // 1. 커넥터 업데이트 ( 삭제 )
        F_DestroyConnetor((SelectedBuildType)v_mb.MyBlockTypeIdx, v_mb.gameObject.transform);

        // 2. 오브젝트 파괴 사운드 재생
        SoundManager.Instance.F_PlaySFX(SFXClip.DESTORY);
    }

    private void F_RepairTool(MyBuildingBlock v_mb)
    {
        // 1. 재료가 충분하면?
        if (BuildMaster.Instance.mybuildCheck.F_WholeSourseIsEnough())
        {
            // 1. max 보다 작으면 , 1 증가
            if (v_mb.MyBlockMaxHp > v_mb.MyBlockHp)
            {
                // 1-1. 인벤토리 업데이트 (재료 소모)
                BuildMaster.Instance.mybuildCheck.F_UpdateInvenToBuilding();

                // 1-2. 정보담기 , ui 업데이트 
                BuildMaster.Instance.mybuildCheck.F_BuildingStart();

                // 1-2. 1증가
                v_mb.MyBlockHp += 1;

                // 2. 오브젝트 수리 사운드 재생 ( 임시로 파괴 사운드와 동일 )
                SoundManager.Instance.F_PlaySFX(SFXClip.DESTORY);
            }
            else
                return;
        }
        else
            return;
    }
    #endregion

    #region building 동작 끝 (설치)

    private void F_FinishBuild()
    {
        // 1. 인벤 내 재료가 충분하면 , 올바른 위치에 있으면, 다른 오브젝트랑 충돌한 상태가 아니면 
        if (_isEnoughResource == true && _isTempValidPosition == true && _isntColliderOther == true)
        {
            // 2. 짓기
            F_BuildTemp();

            // 0. 플레이어 애니메이션 실행 
            PlayerManager.Instance.PlayerController.F_CreateMotion();

            // 3. 인벤토리 업데이트
            BuildMaster.Instance.mybuildCheck.F_UpdateInvenToBuilding();

            // 4. 사운드 재생
            SoundManager.Instance.F_PlaySFX(SFXClip.INSTALL);
        }
        else
            return;
    }

    private void F_BuildTemp()
    {
        if (_tempObjectBuilding != null)
        {
            // 0. 생성
            GameObject _nowbuild = Instantiate(F_GetCurBuild(_buildTypeIdx, _buildDetailIdx), _tempObjectBuilding.transform.position, _tempObjectBuilding.transform.rotation, _parentTransform);

            // 1. destory
            Destroy(_tempObjectBuilding);
            _tempObjectBuilding = null;

            // 3. model의 material & Layer(buildFinished) 변겅
            F_ChangeMaterial(_nowbuild.transform.GetChild(0), _oriMaterial);
            F_ChangeLayer(_nowbuild.transform.GetChild(0), _buildFinishedint);

            // 4-1. model의 콜라이더를 is trigger 체크 해제 ( Door은 Model 콜라이더가 trigger이여야함 )
            if (_SelectBuildType == SelectedBuildType.Door)
                F_ColliderTriggerOnOff(_nowbuild.transform.GetChild(0), true);
            else
                F_ColliderTriggerOnOff(_nowbuild.transform.GetChild(0), false);

            // 4-2. MyModelblock은 설치 후 삭제 ( 다 지어지고나서는 쓸모가 x )
            // Destroy(_nowbuild.transform.GetChild(0).gameObject.GetComponent<MyModelBlock>());
            
            // 5. MyBuildingBlock 추가
            if (_nowbuild.GetComponent<MyBuildingBlock>() == null)
                _nowbuild.AddComponent<MyBuildingBlock>();

            // 5-1. block에 필드 초기화 
            MyBuildingBlock _nowBuildBlock = _nowbuild.GetComponent<MyBuildingBlock>();
            _nowBuildBlock.F_SetBlockFeild(_buildTypeIdx, _buildDetailIdx % 10,
                BuildMaster.Instance.currBlockData.BlockHp,
                BuildMaster.Instance.currBlockData.BlockMaxHp);

            // 1. 커넥터 지정 
            F_SettingConnectorType(_SelectBuildType , _otherConnectorTr);

            // 0. 커넥터 추가
            F_CreateConnector(_nowBuildBlock.transform);

            // 0. 그 자리에 원래있던 커넥터 destory
            Destroy(_otherConnectorTr.gameObject);
        }
    }


    #endregion


    #region Connector Create

    // 타입에 따라 회전 설정 
    public Quaternion F_SettingTypeToRatation(ConnectorType v_type)
    {
        Quaternion _rot = Quaternion.identity;
        switch (v_type)
        {
            case ConnectorType.FloorConnector:
                _rot.eulerAngles = new Vector3(90f, 0, 0);
                return _rot;
            case ConnectorType.CellingConnector:
                _rot.eulerAngles = new Vector3(90f, 0, 0);
                return _rot;
            case ConnectorType.BasicWallConnector:
                _rot.eulerAngles = new Vector3(0, 0, 0);
                return _rot;
            case ConnectorType.RotatedWallConnector:
                _rot.eulerAngles = new Vector3(0, 90f, 0);
                return _rot;
            default:
                return _rot;
        }

    }

    // Type에 따라 레이어 설정 
    private LayerMask F_returnLayerType(ConnectorType v_type, Vector3 v_genePosi)
    {
        if (v_type == ConnectorType.CellingConnector && v_genePosi.y == 0)
            return _connectorLayer[(int)ConnectorType.FloorConnector].Item1;
        else
            return _connectorLayer[(int)v_type].Item1;
    }

    // 해당 위치에서 Lay
    private Collider[] F_DetectOBject(Vector3 v_posi, LayerMask v_layer)
    {
        // 해당 위치에서 layermask 을 검사해서 return
        Collider[] _coll = Physics.OverlapSphere(v_posi, 1f, v_layer);

        return _coll;
    }

    // 커넥터 생성 
    private GameObject F_InstaceConnector(Vector3 v_genePosi, ConnectorType v_type)
    {
        GameObject _connectorInstance = Instantiate(_connectorObject, v_genePosi, Quaternion.identity);
        _connectorInstance.transform.parent = _parentTransform;     // 부모설정 

        // 2. dir에 따라 회전값 조정, 적용
        _connectorInstance.transform.rotation = F_SettingTypeToRatation(v_type);

        // 3. myBuildingBlock 추가
        _connectorInstance.GetComponent<MyBuildingBlock>().F_SetBlockFeild( ((int)v_type + 1) * - 1 , -1, -100, -100);      // floor : -1 , celling : -2 , basic wall : -3 , rotated wall : -4 

        // 4. 레이어 , 타입이 celling일 때, celling의 y가 0 이면? -> floor레이어야함
        if(v_type == ConnectorType.CellingConnector && v_genePosi.y == 0  )
            _connectorInstance.layer = _connectorLayer[(int)ConnectorType.FloorConnector].Item2;
        else
            _connectorInstance.layer = _connectorLayer[(int)v_type].Item2;

        return _connectorInstance;
    }

    private void F_CreateConnector(Transform v_stanardTrs) // 기준이 되는 블럭의 trs 
    {
        // 현재 connector안의 List 에 접근 
        for (int i = 0; i < _currConnector.connectorList.Count; i++)
        {
            ConnectorType _conType = _currConnector.connectorList[i].Item1;
            Vector3 _posi = _currConnector.connectorList[i].Item2 + v_stanardTrs.position;

            // Layermask + buildFinished LayerMask 
            Collider[] coll = F_DetectOBject(_posi , F_returnLayerType( _conType, _posi) | _buildFinishedLayer);

            // 2. 검사해서 잡히면? 설치 x
            if (coll.Length > 0)
                continue;

            // 3. 검사해서 안 잡히면? -> 설치0
            else
                F_InstaceConnector(_posi , _conType );
            
        }
    }

    public void F_DestroyConnetor(SelectedBuildType v_buildType, Transform v_stanardTrs)
    {
        _detectConnectorOnDestroyBlock.Clear();
        _detectBuildFinishedBlockOnConnector.Clear();

        // 0. 커넥터 타입 지정하기 
        F_SettingConnectorType(v_buildType, v_stanardTrs);

        // 1. 삭제 블럭 기준으로 >  모든 커넥터 삭제하기 
        for (int i = 0; i < _currConnector.connectorList.Count; i++) 
        {
            ConnectorType _conType = _currConnector.connectorList[i].Item1;
            Vector3 _posi = _currConnector.connectorList[i].Item2 + v_stanardTrs.position;
            
            Collider[] coll = F_DetectOBject(_posi, _tempWholeLayer );      // 커넥터 전체레이어 

            // 1. 검사해서 잡히면? > 삭제 
            if (coll.Length > 0)
            {
                // 1. 리스트에 넣기 
                _detectConnectorOnDestroyBlock.Add(new Tuple<ConnectorType, Vector3 >(_conType , _posi));

                // 2. 삭제 
                Destroy(coll[0].gameObject);
            }
        }

        // 2. Destory 에 관한 커넥터 업데이트 
        StartCoroutine(F_UpdateConnector(_detectConnectorOnDestroyBlock , _detectBuildFinishedBlockOnConnector, v_stanardTrs ));
    }

    IEnumerator F_UpdateConnector(List<Tuple<ConnectorType, Vector3>> v_temp , List<Tuple<ConnectorType, Transform>> v_list , Transform v_Standard)
    {
        // 2. temp list에 담긴 위치에서 다시 커넥터 검사 => buildFinished된 블럭을 감지 
        yield return StartCoroutine(F_DetectConnector(v_temp));

        // 3. 2번에서 감지된 블럭에서 다시 커넥터 생성 
        yield return StartCoroutine(F_testConnectorDetect(v_list));

        // ##TODO 
        // buildFinished 블럭을 맨 나중에 지워서 2,3번에서 buildFinished블럭 감지하면 감지가됨, 그래서 커넥터 이상하게 생김 
        // 타입을 Vector3로 가지고 있고, 본인 Trasnfrom을 먼저 지워야겠음 

        // 4. 넘어온 v_standard trasnfrom을 커넥터 다 생성하고 나서 지우기 
        yield return StartCoroutine(F_DestoryStandardObject(v_Standard));
    }

    IEnumerator F_DetectConnector(List<Tuple<ConnectorType, Vector3>> v_temp)
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < v_temp.Count; i++)
        {
            ConnectorType _listCon = v_temp[i].Item1;
            Vector3 _listPosi = v_temp[i].Item2;

            // 해당 커넥터위치에서 다시 Connector구하기
            _currConnector = _connectorArr[ (int)_listCon ];

            // _conn을기준으로 buildFinsished 블럭 찾기 
            for (int cnt = 0; cnt < _currConnector.connectorList.Count; cnt++)
            {
                ConnectorType _conconType = _currConnector.connectorList[cnt].Item1;
                Vector3 _posiposi = _currConnector.connectorList[cnt].Item2 + _listPosi;
                
                Collider[] coll = F_DetectOBject(_posiposi, _buildFinishedLayer);      // buildFinished 

                if (coll.Length > 0)
                {
                    _detectBuildFinishedBlockOnConnector.Add(new Tuple<ConnectorType, Transform>(_conconType, coll[0].transform ));
                }
                
            }
            
        }
    }

    IEnumerator F_testConnectorDetect(List<Tuple<ConnectorType, Transform>> v_list ) 
    {
        yield return new WaitForSeconds(0.1f);

        for(int i = 0; i < v_list.Count; i++) 
        {
            // 1. 현재 커넥터 타입 정하기
           _currConnector = _connectorArr[(int)v_list[i].Item1];

            // 2. 커넥터 생성
            F_CreateConnector(v_list[i].Item2);

            yield return new WaitForSeconds(0.1f);

        }
    }

    IEnumerator F_DestoryStandardObject(Transform v_standardObj) 
    {
        yield return new WaitForSeconds(0.1f);

        Debug.Log("오브젝트 삭제 ");
        Destroy(v_standardObj.gameObject);
    }

    #endregion

    #region saveManager 

    // 데이터 파일이 없을 때, 초기에 생성 
    public void F_FirstInitBaseFloor() 
    {
        _buildTypeIdx = 0;
        _buildDetailIdx = 0;
        Vector3 _buildVec = Vector3.zero;

        // 1. floor 생성 
        for (int i = -1; i <= 1; i++) 
        {
            for (int j = -1; j <= 1; j++) 
            {
                _buildVec = new Vector3( j * 5 , 0 , i * -5);

                // 1. 기본블럭 생성 
                GameObject _nowbuild = Instantiate( F_GetCurBuild( _buildTypeIdx , _buildDetailIdx ), _buildVec, Quaternion.identity , _parentTransform);
            }
        }

        // 2. 커넥터 업데이트 
        // 2-0. floor 커넥터로 지정 
        _currConnector = _connectorArr[(int)ConnectorType.FloorConnector];

        // 2-1. 초기 N개 블럭에 대한 커넥터 업데이트 ( parentTransform의 childCount로 하면 계속늘어나서 무한루프 )
        for (int i = 0; i < 9; i++)
        {
            // 2-1-2. 커넥터 create 
            F_CreateConnector(_parentTransform.GetChild(i));
        }

    }

    // 세이브 불러온 후, MyBuldingBlock에 필드 채워넣기
    public void F_CreateBlockFromSave(int _t , int _d , Vector3 _trs , Vector3 _ro , int _h , int _maxhp) 
    {
        // 타입 인덱스, 디테일인덱스, 위치, 회전, hp , 최대 hp

        // 1. 생성 ,  부모지정 
        GameObject _nowbuild;

        // 인덱스가 음수이면 ? -> Connector 
        if (_t < 0)
        { 
            _nowbuild = Instantiate(_connectorObject , _trs, Quaternion.identity , _parentTransform);
            _nowbuild.layer = _connectorLayer[ (_t * -1) - 1 ].Item2;
        }
        else
            _nowbuild = Instantiate(F_GetCurBuild(_t, _d), _trs, Quaternion.identity, _parentTransform);

        // 2. 내 회전 조정
        Quaternion _qu = Quaternion.Euler(_ro);
        _nowbuild.transform.rotation = _qu;

        // myBuildingBlock에 스크립트가 없을수도있음
        if( _nowbuild.GetComponent<MyBuildingBlock>() == null)
            _nowbuild.AddComponent<MyBuildingBlock>();
        
        // 3-5. block의 필드
        MyBuildingBlock _tmpBlock = _nowbuild.GetComponent<MyBuildingBlock>();
        // 필드 세팅
        _tmpBlock.F_SetBlockFeild( _t, _d, _h , _maxhp );

    }


    #endregion

    #region chagneEct


    // Collider의 Trigger OnOff
    private void F_ColliderTriggerOnOff( Transform v_trs , bool v_flag) 
    {
        v_trs.GetComponent<Collider>().isTrigger = v_flag;
    }

    // Material change
    private void F_ChangeMaterial( Transform v_pa , Material material ) 
    {
        foreach ( MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>()) 
        {
            msr.material = material;
        }
    }

    // Mesh OnOff
    private void F_MeshOnOff( Transform v_pa , bool v_flag) 
    {
        foreach (MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>())
        {
            msr.enabled = v_flag;
        }
    }

    // 레이어 변경
    private void F_ChangeLayer( Transform v_pa , int v_layer  )    // 게임오브젝트의 레이어를 바꿀 때는 int 형으로
    {
        for (int i = 0; i < v_pa.childCount; i++)
        {
            v_pa.GetChild(i).gameObject.layer = v_layer;
        }
        
    }
    #endregion

    #region Player Controller
    
    // 건설 도구 내렸을 때 초기화 함수 
    public void F_InitBuildngMode() 
    {
        // 0. 현재 실행하고 있는 building 코루틴 종료 
        StopAllCoroutines();

        // 1. buildingProgressUi 끄기
        BuildMaster.Instance.housingUiManager._buildingProgressUi.gameObject.SetActive(false);

        // 2. 혹시나 켜져있을 housing ui도 끄기 
        BuildMaster.Instance.housingUiManager._buildingCanvas.gameObject.SetActive(false);

        // 3. preview 오브젝트 끄기
        if (_tempObjectBuilding != null)
            Destroy(_tempObjectBuilding);
        _tempObjectBuilding = null;

    }

    #endregion

    #region MyModelBlock
    public void F_IsntCollChagneMaterail( int v_num ) 
    { 
        switch (v_num) 
        {
            // 초록색으로 model 변화
            case 0:
                F_ChangeMaterial(_modelTransform, _greenMaterial );
                break;
            // 빨간색으로 model 변화
            case 1:
                F_ChangeMaterial(_modelTransform, _redMaterial );
                break;
        }
    }

    #endregion
}

