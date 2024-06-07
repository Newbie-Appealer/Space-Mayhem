using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MyBuildManager : MonoBehaviour
{
    [Header("===Block Transform===")]
    [HideInInspector] public GameObject _tempObject;        // 임시 오브젝트

    [HideInInspector] public Transform _modelTransform;             // 모델 오브젝트 
    [HideInInspector] public Transform _colliderGroupTrasform;      // 콜라이더 부모 오브젝트 

    [Header("===Type===")]
    [HideInInspector] private SelectedBuildType _SelectBuildType;

    #region prefab
    [SerializeField] public List<List<GameObject>> _bundleBulingPrefab;

    [Header("===Build Object===")]
    [SerializeField] private List<GameObject> _floorList;
    [SerializeField] private List<GameObject> _cellingList;
    [SerializeField] private List<GameObject> _wallList;
    [SerializeField] private List<GameObject> _doorList;
    [SerializeField] private List<GameObject> _windowList;
    #endregion

    [Header("===LayerMask===")]
    [SerializeField] LayerMask _currTempLayer;              // 현재 (temp 블럭이 감지할) 레이어 

    #region Material
    [Header("===Material===")]
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;
    [HideInInspector] public Material _oriMaterial;
    [HideInInspector] Material _nowBuildMaterial;
    #endregion

    public bool _isEnoughResource;                          // 설치하기에 재료가 충분한지
    private int _housingManager_Typeidx;                    // myBuildManager에서 사용할 type 인덱스
    private int _housingManager_Detailidx;                  // myBuildingManager에서 사용할 detail 

    // =============================================
    private void Awake()
    {
        // 1. 초기화                      
        _bundleBulingPrefab = new List<List<GameObject>> // 각 block 프리팹 List를 하나의 List로 묶기
        {
            _floorList,
            _cellingList,
            _wallList,
            _doorList,
            _windowList
        };

        // 1. 재료 상태 초기화 -> buildManager에서 관리 
        _isEnoughResource = false;

    }

    public void F_GetbuildType(int v_type = 0, int v_detail = 1)
    {
        // 0. Ui상 잘못된 idx가 넘어왔을 때 
        if (v_type < 0 && v_detail < 0)
            return;

        // 1. buildMater의 index 넣기 
        BuildMaster.Instance.F_SetHousnigIdx(v_type , v_detail);

        // 1-1. 이 스크립트에서 사용할 idx 가져오기 
        _housingManager_Typeidx = BuildMaster.Instance._buildTypeIdx;
        _housingManager_Detailidx = BuildMaster.Instance._buildDetailIdx;

        // 2. 임시 오브젝트 확인
        if (_tempObject != null)
            Destroy(_tempObject);
        _tempObject = null;

        // 3. building check 초기화
        BuildMaster.Instance.mybuildCheck.F_BuildingStart();

        // 4. 동작 시작 
        StopAllCoroutines();
        StartCoroutine(F_TempBuild());
    }

    IEnumerator F_TempBuild()
    {
        // 0. index에 해당하는 게임 오브젝트 return
        GameObject _currBuild = F_GetCurBuild(_housingManager_Typeidx, _housingManager_Detailidx);
        // 0.1. 내 블럭 타입에 따라 검사할 layer , Conneector 정하기 
        F_SettingCurrLayer(_SelectBuildType);

        // 0.2. type에 따라 progress on off ui 설정
        F_OnOffProgressUI();

        while (true)
        {
            // 수리도구 type의 파괴도구 일 때
            if (_SelectBuildType == SelectedBuildType.RepairTools)
            {
                BuildMaster.Instance.housingRepairDestroy.F_RepairAndDestroyTool( _currTempLayer );
            }
            // 수리도구, build type 일 때 
            else
            {
                // 1. index에 해당하는 게임오브젝트 생성 , tempObjectbuilding 오브젝트 생성 
                F_CreateTempPrefab(_currBuild);

                // 2. 생성한 오브젝트를 snap , tempObjectBuilding 오브젝트 넘기기 
                BuildMaster.Instance.housingSnapBuild.F_OtherBuildBlockBuild( _SelectBuildType ,_tempObject , _currTempLayer );
            }

            // update 효과 
            yield return null;
        }
    }

    private void F_OnOffProgressUI()
    {
        // 0. repair type 의 destroy툴이면 progressUI 끄기 
        if (_SelectBuildType == SelectedBuildType.RepairTools && _housingManager_Detailidx == 1)
            BuildMaster.Instance.housingUiManager.F_OnOFfBuildingProgressUi(false);
        // 1. 아니면 켜기
        else
            BuildMaster.Instance.housingUiManager.F_OnOFfBuildingProgressUi(true);

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
                _currTempLayer = BuildMaster.Instance._connectorLayer[0].Item1;                          // floor 레이어
                break;
            case SelectedBuildType.Celling:
                _currTempLayer = BuildMaster.Instance._connectorLayer[1].Item1;                          // celling 레이어
                break;
            case SelectedBuildType.Wall:                                                                 // window, door, window는 같은 wall 레이어 사용 
            case SelectedBuildType.Door:
            case SelectedBuildType.Window:
                _currTempLayer = BuildMaster.Instance._connectorLayer[2].Item1;                          // wall 레이어
                break;
            case SelectedBuildType.RepairTools:                                                          // repair 툴 일 때 
                _currTempLayer = BuildMaster.Instance._buildFinishedLayer;                               // buildFinished
                break;
        }
    }

    private void F_CreateTempPrefab(GameObject v_temp)
    {
        // 실행조건
        // temp오브젝트가 null이되면 바로 생성됨!
        if (_tempObject == null)
        {
            // 1. 생성 & 100f,100f,100f는 임시위치 
            _tempObject = Instantiate(v_temp, new Vector3(100f, 100f, 100f), Quaternion.identity);

            // 2. model Transform & collider group Transform 
            _modelTransform         = _tempObject.transform.GetChild(0);
            _colliderGroupTrasform  = _tempObject.transform.GetChild(1);

            // 2-1. 오브젝트 하위 collider 오브젝트의 하위 콜라이더를 trigger 
            BuildMaster.Instance.F_ColliderTriggerOnOff(_colliderGroupTrasform, true);

            // 3. 원래 material 저장
            _oriMaterial = _modelTransform.GetChild(0).GetComponent<MeshRenderer>().material;

            // 4. mybuilding check의 동작실행
            F_BuldingInitCheckBuild();

            // 5. modeld의 Material 바꾸기
            BuildMaster.Instance.F_ChangeMaterial(_modelTransform, _nowBuildMaterial);
        }
    }

    public void F_BuldingInitCheckBuild()
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


    #region Player Controller
    
    // 건설 도구 내렸을 때 초기화 함수 
    public void F_InitBuildngMode() 
    {
        // 0. 현재 실행하고 있는 building 코루틴 종료 
        StopAllCoroutines();

        // 1. buildingProgressUi 끄기
        BuildMaster.Instance.housingUiManager._buildingProgressUi.gameObject.SetActive(false);

        // 2. 혹시나 켜져있을 housing ui도 끄기 
        BuildMaster.Instance.housingUiManager._buildingBlockSelectUi.gameObject.SetActive(false);

        // 3. preview 오브젝트 끄기
        if (_tempObject != null)
            Destroy(_tempObject);
        _tempObject = null;

    }

    #endregion

    #region MyModelBlock / 아직사용 x 
    
    public void F_IsntCollChagneMaterail( int v_num ) 
    {
        // model을 기준으로 Material을 바꿈 ( 예외 : save 파일 불러올 때 , modelTrs가 null 인데 함수호출됨 (인스턴스화 -> 콜라이더 실행 -> 삭제)
        if (_modelTransform == null)
            return;

        switch (v_num) 
        {
            // 초록색으로 model 변화
            case 0:
                BuildMaster.Instance.F_ChangeMaterial(_modelTransform, _greenMaterial );
                break;
            // 빨간색으로 model 변화
            case 1:
                BuildMaster.Instance.F_ChangeMaterial(_modelTransform, _redMaterial );
                break;
        }
    }
    
    #endregion
}
