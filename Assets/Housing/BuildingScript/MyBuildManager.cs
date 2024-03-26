using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

[System.Serializable]
public enum MySelectedBuildType
{
    floor,
    celling,
    wall,
    door,
    window,
    ladder,
    repair
}

public class MyBuildManager : MonoBehaviour
{
    public static MyBuildManager instance;

    [Header("Player")]
    public GameObject _player;

    [Header("Build Object")]
    [SerializeField] private List<GameObject> _floorList;
    [SerializeField] private List<GameObject> _cellingList;
    [SerializeField] private List<GameObject> _wallList;
    [SerializeField] private List<GameObject> _doorList;
    [SerializeField] private List<GameObject> _windowList;
    [SerializeField] private List<GameObject> _ladderList;
    [SerializeField] private List<GameObject> _repairList;

    [Header("LayerMask")]
    [SerializeField] LayerMask _nowTempLayer;       // 그래서 현재 레이어
    [SerializeField] int _buildingBlocklayer;       // 현재 짓고 있는 블럭의 layer (플레이어 / 다른블럭과 충돌 x)
    [SerializeField] int _buildFinishedLayer;       // 다 지은 블럭의 layer 
    [SerializeField] int _dontRaycastLayer;         // temp 설치 중에 temp block의 충돌감지 방지
    [SerializeField] List< Tuple<LayerMask , int > > _tempUnderBlockLayer;   // 임시 블럭 레이어
        // 0. Temp floor 레이어
        // 1. Temp celling 레이어
        // 2. Temp wall 레이어
        // 3. Temp Ladder 레이어

    [Header( "Type")]
    [SerializeField] private MySelectedBuildType _mySelectBuildType;
   
    [Header("Temp Object Setting")]
    [SerializeField] int _buildTypeIdx;     // 무슨 타입인지
    [SerializeField] int _buildDetailIdx;   // 그 타입안 몇번째 오브젝트 인지
    [SerializeField] bool _isTempValidPosition = false;             // 임시 오브젝트가 지어질 수 있는지

    [Header("Tesmp Object Building")]
    [SerializeField] GameObject _TempObjectBuilding;        // 임시 오브젝트
    [SerializeField] List<Transform> _tempUnderParentTrs;   // 임시 오브젝트 밑의 각 부모 trs
        // 0. 임시 오브젝트 model 부모
        // 1. `` tempFloor 부모
        // 2. `` celling 부모
        // 3. `` tempWall 부모
        // 4. (wall 일때만) `` ladder 부모 
    [SerializeField] Transform _otehrConnectorTr;       // 충돌한 다른 오브젝트 

    [Header("Ori Material")]
    [SerializeField] Material _validBuildMaterial;
    [SerializeField] Material _oriMaterial;

    // 프로퍼티
    public int BuildFinishedLayer { get => _buildFinishedLayer; }


    private void Awake()
    {
        instance = this;
        F_InitLayer();
    }


    private void F_InitLayer() 
    {
        _dontRaycastLayer = LayerMask.NameToLayer("DontRaycastSphere");
        _buildingBlocklayer = LayerMask.NameToLayer("BuildingBlock");
        _buildFinishedLayer = LayerMask.NameToLayer("BuildFinishedBlock");

        _tempUnderBlockLayer = new List<Tuple<LayerMask, int>>
        {
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempFloorLayer") , 17 ),        // temp floor 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempCellingLayer") , 16 ),      // temp celling 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempWallLayer") , 15 ),         // temp wall 레이어
            
        };

    }

    private void Update()
    {
        Debug.DrawRay(_player.transform.position + new Vector3(0, 1f, 0) , _player.transform.forward * 10f , Color.red);
    }

    public void F_GetbuildType( int v_type = 0 , int v_detail = 1) 
    {
        // 1. index 초기화
        _buildTypeIdx = v_type;
        _buildDetailIdx = v_detail;

        // 2. 임시 오브젝트 확인
        if (_TempObjectBuilding != null)
            Destroy(_TempObjectBuilding);
        _TempObjectBuilding = null;

        // 3. 동작 시작 
        StopAllCoroutines();
        StartCoroutine(F_TempBuild());
        

    }

    IEnumerator F_TempBuild() 
    {
        // 0. index에 해당하는 게임 오브젝트 return
        GameObject _currBuild = F_GetCurBuild();
        // 0.1. 내 블럭 타입에 따라 검사할 layer 정하기
        F_TempRaySetting( _mySelectBuildType );         

        while (true) 
        {
            // 1. index에 해당하는 게임오브젝트 생성
            F_CreateTempPrefab(_currBuild);

            // 2. 해당 블럭이랑 같은 Layer만 raycast
            F_Raycast(_nowTempLayer);

            // 3. temp 오브젝트의 콜라이더 검사
            F_CheckCollision();

            // 4. 설치
            if (Input.GetMouseButtonDown(0))
                F_BuildTemp();

            // update 효과 
            yield return null;     
        }
    }


    private void F_Raycast( LayerMask v_layer ) 
    {
        // 넘어온 Layer에 따라 raycast
        RaycastHit _hit;

        if (Physics.Raycast( _player.transform.position + new Vector3(0,1f,0) , _player.transform.forward * 10 , out _hit , 5f , v_layer)) // 타입 : LayerMask
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

    private void F_Snap(Collider[] v_coll ) 
    {
        _otehrConnectorTr = null; 

        for (int i = 0; i < v_coll.Length; i++)
        {

            if (v_coll[i].GetComponent<MyConnector>()._canConnect == true)
            {
                _otehrConnectorTr = v_coll[i].transform;
                break;
            }
        }

        // 설치 가능한 위치가 없으면
        if (_otehrConnectorTr == null)
        {
            F_ChangeMesh(_tempUnderParentTrs[0] , false);
            _isTempValidPosition = false;
            return;
        }

        _TempObjectBuilding.transform.position
             = _otehrConnectorTr.position;

        F_ChangeMesh(_tempUnderParentTrs[0], true);
        _isTempValidPosition = true;
    }

    private GameObject F_GetCurBuild() 
    {
        switch (_buildTypeIdx)
        {
            case 0:
                _mySelectBuildType = MySelectedBuildType.floor;
                return _floorList[_buildDetailIdx];
            case 1:
                _mySelectBuildType = MySelectedBuildType.celling; 
                return _cellingList[_buildDetailIdx];
            case 2:
                _mySelectBuildType = MySelectedBuildType.wall;
                return _wallList[_buildDetailIdx];
            case 3:
                _mySelectBuildType = MySelectedBuildType.door;
                return _doorList[_buildDetailIdx];
            case 4:
                _mySelectBuildType = MySelectedBuildType.window;
                return _windowList[_buildDetailIdx];
            case 5:
                _mySelectBuildType = MySelectedBuildType.ladder;
                return _ladderList[_buildDetailIdx];
            case 6:
                _mySelectBuildType = MySelectedBuildType.repair;
                return _repairList[_buildDetailIdx];
            default:
                return null;
        }
    }

    private void F_TempRaySetting(MySelectedBuildType v_type)
    {
        switch (v_type)
        {
            case MySelectedBuildType.floor:
                _nowTempLayer = _tempUnderBlockLayer[0].Item1;          // floor 레이어
                break;
            case MySelectedBuildType.celling:
                _nowTempLayer = _tempUnderBlockLayer[1].Item1;          // celling 레이어
                break;
            case MySelectedBuildType.wall:                              // window, door, window는 같은 wall 레이어 사용 
            case MySelectedBuildType.door:
            case MySelectedBuildType.window:
                _nowTempLayer = _tempUnderBlockLayer[2].Item1;          // wall 레이어
                break;
        }

    }


    private void F_CreateTempPrefab( GameObject v_temp) 
    {
        // 실행조건
        // temp오브젝트가 null이되면 바로 생성됨!
        if (_TempObjectBuilding == null)
        {
            _TempObjectBuilding = Instantiate(v_temp);

            _tempUnderParentTrs = new List<Transform>
            {
                _TempObjectBuilding.transform.GetChild(0),      // model parent
                _TempObjectBuilding.transform.GetChild(1),      // floor parent
                _TempObjectBuilding.transform.GetChild(2),      // celling parent
                _TempObjectBuilding.transform.GetChild(3),      // wall parent
            };

            // 각 Parent 밑의 오브젝트의 레이어 바꾸기
            for (int i = 1; i < _tempUnderParentTrs.Count; i++) 
            {
                F_ChangeLayer(_tempUnderParentTrs[i], _dontRaycastLayer);        
            }  

            F_ChangeLayer(_tempUnderParentTrs[0], _buildingBlocklayer , true);       // model의 layerl 변경 , 다른것과 충돌 안되게

            //원래 material 저장
            _oriMaterial = _tempUnderParentTrs[0].GetChild(0).GetComponent<MeshRenderer>().material;

            // Material 바꾸기
            F_ChangeMaterial( _tempUnderParentTrs[0], _validBuildMaterial );
        }
    }

    private void F_BuildTemp() 
    { 
        if( _TempObjectBuilding != null && _isTempValidPosition == true) 
        { 
            // 생성
            GameObject _nowbuild = Instantiate(F_GetCurBuild(), _TempObjectBuilding.transform.position , _TempObjectBuilding.transform.rotation );

            Destroy( _TempObjectBuilding);
            _TempObjectBuilding = null;

            // material 변경
            F_ChangeMaterial(_nowbuild.transform.GetChild(0) , _oriMaterial);

            // buildFinished로 변경
            F_ChangeLayer( _nowbuild.transform.GetChild(0) , _buildFinishedLayer);

            // 각 오브젝트에 맞는 temp 레이어로 변환
            for (int i = 1; i < _nowbuild.transform.childCount - 1; i++) 
            {
                F_ChangeLayer( _nowbuild.transform.GetChild(i) , _tempUnderBlockLayer[i-1].Item2 );   
            }

            // 내 temp 오브젝트 충돌 처리 후 Connector 업데이트
            // floor
            foreach (MyConnector mc in _nowbuild.transform.GetChild(1).GetComponentsInChildren<MyConnector>())
            {
                mc.F_UpdateConnector();
            }
            // celling
            foreach (MyConnector mc in _nowbuild.transform.GetChild(2).GetComponentsInChildren<MyConnector>())
            {
                mc.F_UpdateConnector();
            }
            // wall
            foreach (MyConnector mc in _nowbuild.transform.GetChild(3).GetComponentsInChildren<MyConnector>())
            {
                mc.F_UpdateConnector();
            }

            // 현재 나와 충돌한 connector를 update
            _otehrConnectorTr.gameObject.GetComponent<MyConnector>().F_UpdateConnector();
        }
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

    // 건설 도구 내렷을 때 초기화 함수
    

}
