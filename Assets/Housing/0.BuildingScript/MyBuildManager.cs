using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

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

public class MyBuildManager : Singleton<MyBuildManager>
{
    [Header("Player")]
    public GameObject _player;

    [Header("CheckBulidBlock")]
    [SerializeField] MyBuildCheck _mybuildCheck;
    [SerializeField] Transform _parentTransform;

    [Header("BundleBuildingPrepab")]
    [SerializeField] public List<List<GameObject>> _bundleBulingPrefab;

    [Header("Build Object")]
    [SerializeField] private List<GameObject> _floorList;
    [SerializeField] private List<GameObject> _cellingList;
    [SerializeField] private List<GameObject> _wallList;
    [SerializeField] private List<GameObject> _doorList;
    [SerializeField] private List<GameObject> _windowList;

    [Header("LayerMask")]
    [SerializeField] LayerMask _nowTempLayer;       // 그래서 현재 레이어
    [SerializeField] LayerMask _buildFinishedLayer; // 다 지은 블럭의 layermask
    [SerializeField] int _buildFinishedint;         // 다 지은 블럭의 layer int
    [SerializeField] int _dontRaycastInt;         // temp 설치 중에 temp block의 충돌감지 방지
    [SerializeField] List<Tuple<LayerMask, int>> _tempUnderBlockLayer;   // 임시 블럭 레이어
                                                                         // 0. Temp floor 레이어
                                                                         // 1. Temp celling 레이어
                                                                         // 2. Temp wall 레이어
    [SerializeField] public LayerMask _tempWholeLayer;     //  temp floor , celling, wall 레이어 다 합친

    [Header("Type")]
    [SerializeField] private MySelectedBuildType _mySelectBuildType;

    [Header("Temp Object Setting")]
    [SerializeField] int _buildTypeIdx;     // 무슨 타입인지
    [SerializeField] int _buildDetailIdx;   // 그 타입안 몇번째 오브젝트 인지
    [SerializeField] bool _isTempValidPosition  = false;                // 임시 오브젝트가 지어질 수 있는지
    [SerializeField] bool _isEnoughResource     = false;                // 설치하기에 재료가 충분한지?
    [SerializeField] bool _isntColliderOther;                           // temp wall 이 다른 오브젝트랑 충돌한 상태인지? ( false면 충돌 )

    [Header("Tesmp Object Building")]
    [SerializeField] GameObject _TempObjectBuilding;        // 임시 오브젝트
    [SerializeField] List<Transform> _tempUnderParentTrs;   // 임시 오브젝트 밑의 각 부모 trs
                                                            // 0. 임시 오브젝트 model 부모
                                                            // 1. `` tempFloor 부모
                                                            // 2. `` celling 부모
                                                            // 3. `` tempWall 부모
                                                            // 4. (wall 일때만) `` ladder 부모 
    [SerializeField] Transform _otherConnectorTr;       // 충돌한 다른 오브젝트 

    [Header("Ori Material")]
    [SerializeField] Material _oriMaterial;
    [SerializeField] Material _nowBuildMaterial;
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;

    // 프로퍼티
    public int BuildFinishedLayer { get => _buildFinishedint; }
    public bool IsntColliderOther { get => _isntColliderOther; set { _isntColliderOther = value; } }


    // 싱글톤 ( awake 역할 )
    protected override void InitManager()
    {
        F_InitLayer();              // 레이어 초기화
        F_InitBundleBlock();        // 블럭 prefab 을 list하나로 초기화

        _isntColliderOther = true;  // 다른 오브젝트와 충돌되어있는가?

        // ## TODO 저장기능
        SaveManager.Instance.F_LoadBuilding(_parentTransform);
    }

    #region 1. 레이어 초기화 2. 프리팹 list

    private void F_InitLayer()
    {
        _dontRaycastInt = LayerMask.NameToLayer("DontRaycastSphere");
        _buildFinishedint = LayerMask.NameToLayer("BuildFinishedBlock");

        _buildFinishedLayer = LayerMask.GetMask("BuildFinishedBlock");

        _tempUnderBlockLayer = new List<Tuple<LayerMask, int>>
        {
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempFloorLayer") , 17 ),        // temp floor 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempCellingLayer") , 16 ),      // temp celling 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempWallLayer") , 15 ),         // temp wall 레이어
            
        };

        _tempWholeLayer = _tempUnderBlockLayer[0].Item1 | _tempUnderBlockLayer[1].Item1 | _tempUnderBlockLayer[2].Item1;


    }

    private void F_InitBundleBlock()
    {
        // 각 block 프리팹 List를 하나의 List로 묶기
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

    private void Update()
    {
        Debug.DrawRay(_player.transform.position, _player.transform.forward * 10f, Color.red);

        // ##TODO 저장기능
        // L 누르면 building 저장
        if (Input.GetKeyDown(KeyCode.L))
            SaveManager.Instance.F_SaveBuilding(_parentTransform.transform);
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
        if (_TempObjectBuilding != null)
            Destroy(_TempObjectBuilding);
        _TempObjectBuilding = null;

        // 초반에 1회 실행
        _mybuildCheck.F_BuildingStart();

        // 3. 동작 시작 
        StopAllCoroutines();
        StartCoroutine(F_TempBuild());
    }

    IEnumerator F_TempBuild()
    {
        // 0. index에 해당하는 게임 오브젝트 return
        GameObject _currBuild = F_GetCurBuild(_buildTypeIdx, _buildDetailIdx);
        // 0.1. 내 블럭 타입에 따라 검사할 layer 정하기
        F_TempRaySetting(_mySelectBuildType);

        while (true)
        {
            // 수리,파괴 type 일 때
            if (_mySelectBuildType == MySelectedBuildType.RepairTools)
            {
                F_RepairAndFix();
            }
            // build type 일 때 
            else
            {
                F_OtherBuildBlockBuild(_currBuild);
            }

            // update 효과 
            yield return null;
        }
    }

    private void F_OtherBuildBlockBuild(GameObject v_build) 
    {
        // 1. index에 해당하는 게임오브젝트 생성
        F_CreateTempPrefab(v_build);

        // 2. 해당 블럭이랑 같은 Layer만 raycast
        F_Raycast(_nowTempLayer);

        // 3. temp 오브젝트의 콜라이더 검사
        F_CheckCollision();

        // 4. 설치
        if (Input.GetMouseButtonDown(0))
            F_FinishBuild();
    }

    private void F_RepairAndFix() 
    {
        // 0. 우클릭 했을 때
        if (Input.GetMouseButtonDown(0)) 
        {
            // 1. ray 쏴서 finished 블럭이 잡히면
            RaycastHit _hit;
            if (Physics.Raycast(_player.transform.position, _player.transform.forward * 10, out _hit, 5f, _nowTempLayer))   // 타입 : LayerMask
            {
                // 1. myBlock 가져오기 ( 충돌한 model의 부모의 block 스크립트 )
                MyBuildingBlock my = _hit.collider.gameObject.transform.parent.GetComponent<MyBuildingBlock>();
                // 2. 수리도구
                if (_buildDetailIdx == 0)
                {
                    // 2-2. 블럭의 hp를 max hp로 
                    // #TODO
                    my.MyBlockHp = 33;

                }
                // 3. 파괴도구 
                else if (_buildDetailIdx == 1)
                {
                    // 3-1. ray된 블럭의 block 스크립트의 커넥터 update,  _canConnect 를 true로 
                    my.F_BlockCollisionConnector( true );

                    // 3-2. destory
                    Destroy(_hit.transform.gameObject);
                }
            }
        }

    }

    #region ray , snap 동작

    private void F_Raycast(LayerMask v_layer)
    {
        // 넘어온 Layer에 따라 raycast
        RaycastHit _hit;

        if (Physics.Raycast(_player.transform.position, _player.transform.forward * 10, out _hit, 5f, v_layer)) // 타입 : LayerMask
        {
            _TempObjectBuilding.transform.position = _hit.point;
        }
    }

    private void F_CheckCollision()
    {
        Collider[] _coll = Physics.OverlapSphere(_TempObjectBuilding.transform.position, 1f, _nowTempLayer);    // 타입 : LayerMask

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

        // 설치 가능한 위치가 없으면
        if (_otherConnectorTr == null)
        {
            F_ChangeMesh(_tempUnderParentTrs[0], false);
            _isTempValidPosition = false;
            return;
        }

        // 타입이 wall 일땐 회전 
        if (_mySelectBuildType == MySelectedBuildType.Wall || _mySelectBuildType == MySelectedBuildType.Window
            || _mySelectBuildType == MySelectedBuildType.Door ) 
        {
            // 내 temp 블럭 회전 += 접촉한 커넥터의 회전
            Quaternion qu = _TempObjectBuilding.transform.rotation;
            qu.eulerAngles = new Vector3(qu.eulerAngles.x, _otherConnectorTr.eulerAngles.y , qu.eulerAngles.z);
            _TempObjectBuilding.transform.rotation = qu;
        }

        // 위치수정
        _TempObjectBuilding.transform.position
             = _otherConnectorTr.position;

        // mesh 켜기
        F_ChangeMesh(_tempUnderParentTrs[0], true);
        // 설치가능 
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
                _nowTempLayer = _tempUnderBlockLayer[0].Item1;          // floor 레이어
                break;
            case MySelectedBuildType.Celling:
                _nowTempLayer = _tempUnderBlockLayer[1].Item1;          // celling 레이어
                break;
            case MySelectedBuildType.Wall:                              // window, door, window는 같은 wall 레이어 사용 
            case MySelectedBuildType.Door:
            case MySelectedBuildType.Window:
                _nowTempLayer = _tempUnderBlockLayer[2].Item1;          // wall 레이어
                break;
            case MySelectedBuildType.RepairTools:
                _nowTempLayer = _buildFinishedLayer;                    // 다 지은 블럭의 layer
                break;
        }

    }


    private void F_CreateTempPrefab( GameObject v_temp) 
    {
        // 실행조건
        // temp오브젝트가 null이되면 바로 생성됨!
        if (_TempObjectBuilding == null)
        {
            // 1. 생성
            _TempObjectBuilding = Instantiate(v_temp);

            // 2. 부모 trs 초기화
            _tempUnderParentTrs = new List<Transform>
            {
                _TempObjectBuilding.transform.GetChild(0),      // model parent
                _TempObjectBuilding.transform.GetChild(1),      // floor parent
                _TempObjectBuilding.transform.GetChild(2),      // celling parent
                _TempObjectBuilding.transform.GetChild(3),      // wall parent
            };

            // 2. 각 Parent 밑의 오브젝트의 레이어 바꾸기
            for (int i = 1; i < _tempUnderParentTrs.Count; i++) 
            {
                F_ChangeLayer(_tempUnderParentTrs[i], _dontRaycastInt);        
            }

            // 2-1. moel의 is Trigger 켜기 
            F_OnCollision(_tempUnderParentTrs[0], true);

            // 3. 원래 material 저장
            _oriMaterial = _tempUnderParentTrs[0].GetChild(0).GetComponent<MeshRenderer>().material;

            // 4. mybuilding check의 동작실행
            F_BuldingInitCheckBuild();

            // 5. modeld의 Material 바꾸기
            F_ChangeMaterial(_tempUnderParentTrs[0], _nowBuildMaterial);
        }
    }

    private void F_BuldingInitCheckBuild() 
    {
        // 1. 초기화
        _mybuildCheck.F_BuildingStart();

        // 2. 재료가 충분한지? 충분하면 true,  아니면 false
        _isEnoughResource = _mybuildCheck.F_WholeSourseIsEnough();

        // 3. 재료충분도에 따른 material 변화
        if (_isEnoughResource)
            _nowBuildMaterial = _greenMaterial;
        else
            _nowBuildMaterial = _redMaterial;
    }

    #endregion

    #region building 동작 끝 (설치)

    private void F_FinishBuild() 
    {
        // 1. 인벤 내 재료가 충분하면 , 올바른 위치에 있으면, 다른 오브젝트랑 충돌한 상태가 아니면 
        if (_isEnoughResource == true &&  _isTempValidPosition == true && _isntColliderOther == true )
        {
            // 2. 짓기
            F_BuildTemp();

            // 0. 플레이어 애니메이션 실행 
            PlayerManager.Instance.PlayerController.F_CreateMotion();

            // 3. 인벤토리 업데이트
            _mybuildCheck.F_UpdateInvenToBuilding();
        }
        else
            return;
    }
    
    private void F_BuildTemp() 
    { 
        if( _TempObjectBuilding != null) 
        { 
            // 0. 생성
            GameObject _nowbuild = Instantiate(F_GetCurBuild(_buildTypeIdx, _buildDetailIdx), _TempObjectBuilding.transform.position , _TempObjectBuilding.transform.rotation , _parentTransform);

            // 1. destory
            Destroy( _TempObjectBuilding);
            _TempObjectBuilding = null;

            // 3. model의 material 변경
            F_ChangeMaterial(_nowbuild.transform.GetChild(0) , _oriMaterial);

            // 4. model의 layer (buildFinished로) 변경
            F_ChangeLayer( _nowbuild.transform.GetChild(0) , _buildFinishedint , true );

            // 4-1. model의 콜라이더를 is trigger 체크 해제
            F_OnCollision(_nowbuild.transform.GetChild(0) , false);

            // 4-2. model의 MyModelBlock에 접근해 false로 변환
            F_ChagneMyModelBlock(_nowbuild.transform.GetChild(0) , true);

            // 5. 각 오브젝트에 맞는 temp 레이어로 변환
            for (int i = 1; i < _nowbuild.transform.childCount - 1; i++) 
            {
                F_ChangeLayer( _nowbuild.transform.GetChild(i) , _tempUnderBlockLayer[i-1].Item2 );   
            }

            // 6. 현재 새로만든 block Connector 업데이트
            F_ConeectorUpdate(_nowbuild.transform);

            // 8. 현재 새로 만든 block에 MyBuildingBlock 추가
            if (_nowbuild.GetComponent<MyBuildingBlock>() == null)
            {
                // null 이면 추가하기 
                _nowbuild.AddComponent<MyBuildingBlock>();
            }

            // 9. nowBuild와 충돌한 커넥터들 업데이트
            MyBuildingBlock _nowBuildBlock = _nowbuild.GetComponent<MyBuildingBlock>();
            _nowBuildBlock.F_BlockCollisionConnector( false );

            // 8-1. block에 필드 초기화 
            _nowBuildBlock.F_SetBlockFeild(_buildTypeIdx, _buildDetailIdx % 10, _mybuildCheck._myblock.BlockHp);

        }
    }

    // 커넥터 업데이트 
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

    public void F_CreateBlockFromSave(int _t , int _d , Vector3 _trs , Vector3 _ro ,int _h) 
    {
        // 타입 인덱스, 디테일인덱스, 위치, 회전, hp

        // 생성 ,  부모지정 
        GameObject _nowbuild = Instantiate(F_GetCurBuild(_t, _d), _trs, Quaternion.identity, _parentTransform);
        // 내 회전 조정
        Quaternion _qu = Quaternion.Euler(_ro);
        _nowbuild.transform.rotation = _qu;

        // myBuildingBlock에 스크립트가 없을수도있음
        if( _nowbuild.GetComponent<MyBuildingBlock>() == null)
            _nowbuild.AddComponent<MyBuildingBlock>();
        
        // 3-5. block의 필드
        MyBuildingBlock _tmpBlock = _nowbuild.GetComponent<MyBuildingBlock>();
        // 필드 세팅
        _tmpBlock.F_SetBlockFeild(_t, _d, _h);

    }

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
        // model 밑의 MyModelBlock에 접근해서 bool 변환 
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

    // 레이어 변경
    // 부모 레이어 변경 시 v_flag를 true
    private void F_ChangeLayer( Transform v_pa , int v_layer , bool v_flag = false )    // 게임오브젝트의 레이어를 바꿀 때는 int 형으로
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
    
    // 건설 도구 내렸을 때 초기화 함수 
    public void F_InitBuildngMode() 
    {
        // 0. 현재 실행하고 있는 building 코루틴 종료 
        StopAllCoroutines();
    
        // 1. buildingProgressUi 끄기
        HousingUiManager.Instance._buildingProgressUi.gameObject.SetActive(false);

        // 2. 혹시나 켜져있을 housing ui도 끄기 
        //HousingUiManager.Instance._buildingCanvas.gameObject.SetActive(false);

        // 3. preview 오브젝트 끄기
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
            // 초록색으로 model 변화
            case 0:
                F_ChangeMaterial(_tempUnderParentTrs[0] , _greenMaterial );
                break;
            // 빨간색으로 model 변화
            case 1:
                F_ChangeMaterial(_tempUnderParentTrs[0], _redMaterial );
                break;
        }
    }

    #endregion
}
