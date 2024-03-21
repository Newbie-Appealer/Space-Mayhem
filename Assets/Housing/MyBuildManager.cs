using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Timeline;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;


[System.Serializable]
public enum MySelectedBuildType
{
    defaultFloor,
    floor,
    celling,
    wall
}

public class MyBuildManager : MonoBehaviour
{
    public static MyBuildManager instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Player")]
    public GameObject _player;

    [Header("Build Object")]
    [SerializeField] private List<GameObject> _floorList;
    [SerializeField] private List<GameObject> _cellingList;
    [SerializeField] private List<GameObject> _wallList;

    [Header("LayerMask")]
    [SerializeField] LayerMask _nowTempLayer;       // 그래서 현재 레이어
    [SerializeField] LayerMask _tempFloorLayer;     // 임시 floor 레이어
    [SerializeField] int _floorLayer;               // temp floor의 Layer (int)
    [SerializeField] LayerMask _tempWallLayer;      // 임시 wall 레이어  
    [SerializeField] int _wallLayer;                // temp wall의 Layer (int)
    [SerializeField] public LayerMask _finishedLayer;      // 다 지은 블럭의 레이어
    [SerializeField] int _buildFinished;            // 다 지은 블럭의 layer (int)
                                                    // 
    [SerializeField] int _dontRaycastLayer;         // temp 설치 중에 temp wall&floor의 충돌감지
    [SerializeField] int _buildingBlocklayer;       // 현재 짓고 있는 블럭의 layer (플레이어 / 다른블럭과 충돌 x)

    [Header( "Type")]
    [SerializeField] private MySelectedBuildType _mySelectBuildType;
   
    [Header("Temp Object Setting")]
    [SerializeField] int _buildTypeIdx;     // 무슨 타입인지
    [SerializeField] int _buildDetailIdx;   // 그 타입안 몇번째 오브젝트 인지`

    [Header("Tesmp Object Building")]
    [SerializeField] GameObject _TempObjectBuilding;    // 임시 오브젝트
    [SerializeField] Transform _TempModelparent;        // 임시 오브젝트 model 부모
    [SerializeField] Transform _TempFloorParent;        // 임시 오브젝트 밑에 tempFloor 부모
    [SerializeField] Transform _TempWallParent;         // 임시 오브젝트 밑에 tempWall 부모
    [SerializeField] Transform _otehrConnectorTr;       // 충돌한 다른 오브젝트 

    [Header("Ori Material")]
    [SerializeField] Material _validBuildMaterial;
    [SerializeField] Material _oriMaterial;

    [SerializeField] bool _isTempValidPosition = false;             // 임시 오브젝트가 지어질 수 있는지

    private void Start()
    {
        _floorLayer = LayerMask.NameToLayer("TempFloorLayer");
        _wallLayer = LayerMask.NameToLayer("TempWallLayer");

        _dontRaycastLayer = LayerMask.NameToLayer("DontRaycastSphere");
        _buildingBlocklayer = LayerMask.NameToLayer("BuildingBlock");
        _buildFinished = LayerMask.NameToLayer("BuildFInishedBlock");
    }

    private void Update()
    {
        Debug.DrawRay(_player.transform.position, _player.transform.forward * 10f , Color.red);
    }

    public void F_GetbuildType( int v_type , int v_detail) 
    {
        _buildTypeIdx = v_type;
        _buildDetailIdx = v_detail;

        if (_TempObjectBuilding != null)
            Destroy(_TempObjectBuilding);
        _TempObjectBuilding = null;

        StopAllCoroutines();
        StartCoroutine(F_TempBuild());
    }

    IEnumerator F_TempBuild() 
    {
        // 게임 오브젝트 return
        GameObject _currBuild = F_GetCurBuild();
        F_TempRaySetting( _mySelectBuildType );         // 내 블럭 타입에 따라 검사할 layer 정하기

        while (true) 
        {
            // 받아온거 생성
            F_CreateTempPrefab(_currBuild);

            // layer따라 레이캐스트
            F_Raycast(_nowTempLayer);

            // 콜라이더 검사
            F_CheckCollision();

            // 설치
            if (Input.GetMouseButtonDown(0))
                F_BuildTemp();

            yield return null;      // update 효과   
        }
    }

    private void F_TempRaySetting( MySelectedBuildType v_type) 
    {
        switch ( v_type ) 
        {
            case MySelectedBuildType.floor:
                _nowTempLayer = _tempFloorLayer;
                break;
            case MySelectedBuildType.wall:
                _nowTempLayer = _tempWallLayer;
                break;
        }

    }

    private void F_Raycast( LayerMask v_layer ) 
    {
        // 넘어온 Layer에 따라 raycast
        RaycastHit _hit;

        if (Physics.Raycast( _player.transform.position , _player.transform.forward * 10 , out _hit , 5f , v_layer)) 
        {
            _TempObjectBuilding.transform.position = _hit.point;
        }
    }

    private void F_CheckCollision() 
    {
        Collider[] _coll = Physics.OverlapSphere( _TempObjectBuilding.transform.position , 1f , _nowTempLayer );

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
            return;

        _TempObjectBuilding.transform.position
             = _otehrConnectorTr.position;

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
            default:
                return null;
        }
    }

    private void F_CreateTempPrefab( GameObject v_temp) 
    {
        // 실행조건
        // temp오브젝트가 null이되면 바로 생성됨!
        if (_TempObjectBuilding == null)
        {
            _TempObjectBuilding = Instantiate(v_temp);

            _TempObjectBuilding.name = "임시 오브젝트";

            _TempModelparent = _TempObjectBuilding.transform.GetChild(0);     // temp model
            _TempFloorParent = _TempObjectBuilding.transform.GetChild(1);     // temp floor
            _TempWallParent = _TempObjectBuilding.transform.GetChild (2);     // temp wall

            // Parent 밑의 오브젝트의 레이어 바꾸기
            F_ChangeChlidLayer( _TempFloorParent , _dontRaycastLayer );        // Tempfloor을 ray 안되게
            F_ChangeChlidLayer( _TempWallParent, _dontRaycastLayer);           // TempWall을 ray 안되게

            F_ChangeChlidLayer( _TempModelparent , _buildingBlocklayer );       // model의 layerl 변경 , 다른것과 충돌 안되게

            //원래 material 저장
            _oriMaterial = _TempModelparent.GetChild(0).GetComponent<MeshRenderer>().material;

            // Material 바꾸기
            F_ChangeMaterial(_TempModelparent , _validBuildMaterial );
        }
    }

    private void F_BuildTemp() 
    { 
        if( _TempObjectBuilding != null && _isTempValidPosition == true) 
        { 
            // 생성
            GameObject _nowbuild = Instantiate(F_GetCurBuild(), _TempObjectBuilding.transform.position , _TempObjectBuilding.transform.rotation );

            _nowbuild.gameObject.name = "새로생성한 오브젝트";

            DestroyImmediate( _TempObjectBuilding);
            _TempObjectBuilding = null;

            // material 변경
            F_ChangeMaterial(_nowbuild.transform.GetChild(0) , _oriMaterial);

            // _TempWall & _TempFloor 의 레이어 변경 필요
            F_ChangeChlidLayer( _nowbuild.transform.GetChild(0) , _buildFinished);      // buildFinished로 변경
            F_ChangeChlidLayer( _nowbuild.transform.GetChild(1) , _floorLayer);         // temp floor로 변경
            F_ChangeChlidLayer( _nowbuild.transform.GetChild(2) , _wallLayer );         // temp walll로 변경

            // 내 temp , wall 오브젝트 충돌 처리 후 업데이트
            foreach ( MyConnector mc in _nowbuild.transform.GetChild(1).GetComponentsInChildren<MyConnector>()) 
            {
                Debug.Log( mc.gameObject.name);
                mc.F_UpdateConnector();
            }
            foreach(MyConnector mc in _nowbuild.transform.GetChild(2).GetComponentsInChildren<MyConnector>()) 
            {
                mc.F_UpdateConnector();
            }

            // 상태 connector를 update
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

    private void F_ChangeChlidLayer( Transform v_pa , int v_layer , bool v_flag = false ) 
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

}
