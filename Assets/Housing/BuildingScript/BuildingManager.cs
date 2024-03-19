using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public enum SelectBuildType 
{
    defaultFloor,
    floor,
    celling,
    wall
}


public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    [Header("Player")]
    public GameObject _player;

    [Header("Build Setting")]
    [SerializeField] public LayerMask _connectorLayer;  // connector이 있는 레이어로 설정
    [SerializeField] int _buildBlockLayer;              // buildBlock 레이어

    [Header("building object")]
    [SerializeField] GameObject[] _floorObject;                      // 바닥 오브젝트
    [SerializeField] GameObject[] _cellingObject;                    // 천장 오브젝트
    [SerializeField] GameObject[] _wallObject;                       // 벽 오브젝트

    [Header("now Select Object")]
    [SerializeField]
    SelectBuildType _nowSelectType;                 // 현재 선택 된 임시 오브젝트의 타입 저장
    [SerializeField]
    ConnectorType _myTempConnector;                 // 현재 선택된 커넥터 타입
    [SerializeField]
    GameObject _nowSelectTempObject;                // 현재 선택 된 임시 오브젝트
    [SerializeField]
    Transform _modelParent;                         // 임시 오브젝트의 model 
    [SerializeField]
    bool _canTempObjectSnap = false;                // 임시 오브젝트가 snap 가능한지?
    [SerializeField]
    public string _nowSelectBlockName;              // 현재 선택된 블럭의 이름 (임시블럭의 이름)

    [Header(" Material ")]
    [SerializeField] Material _tempMaterial;                         // 임시 오브젝트의 material
    [SerializeField] Material _oriMaterial;                          // 원래 material

    [Header("현재 블럭 설치 idx")]
    [SerializeField] private int _blockTypeIdx = -1;
    [SerializeField] private int _blockBuilIdx = -1;

    //[SerializeField] bool _startBuilding = true;    // building 시작 조건 ( )
    [SerializeField] bool _endBuilding = true;      // building 끝 조건
    [SerializeField] bool _blockisMove;             // 블럭이 snap 될 조건?

    private void Start()
    {
        instance = this;        // 싱글톤

        // layer num 찾기
        _buildBlockLayer    = LayerMask.NameToLayer("BuildingBlock");

    }

    private void Update()
    {
        // 플레이어 앞으로 ray 쏘기
        Debug.DrawRay(_player.transform.position, _player.transform.forward * 10, Color.red);
    }

    // HousingUiManager에서 마우스 우클릭을 떼면 실행
    public void F_BuildingTypeNBlockiIdx(int v_typeIdx, int v_builgIdx)
    {
        // 0. 인덱스 설정
        _blockTypeIdx = v_typeIdx;
        _blockBuilIdx = v_builgIdx;

        // 1. 조건 설정
        _endBuilding = true;

        // 2. 스냅 조건 설정
        _canTempObjectSnap = false;

        // 3. 현재 오브젝트 있으면 destroy
        if (_nowSelectTempObject != null)
            Destroy( _nowSelectTempObject );
        _nowSelectTempObject = null;

        // 4. 새로운 블럭을 설치 할 때 ( 전에 설치 되고있는 블럭이 있을수도 )
        StopAllCoroutines();    // 이전에 실행하던 코루틴 멈추기

        // 5. building 시작
        StartCoroutine(F_StartBuild());
    }

    IEnumerator F_StartBuild()
    {
        while (true)
        {
            // 0. 생성할 오브젝트 return
            GameObject curr = F_InstanseTempGameobj(_blockTypeIdx, _blockBuilIdx);
            curr.name = "NowTempBlock";
            _nowSelectBlockName = curr.name;

            // 1. return 받은 오브젝트 생성
            F_CreatePrefab(curr);

            // 2. 오브젝트가 ray를 따라 움직이게
            F_ObjMoveToRay();
            // 3. block의 콜라이더 검사
            F_checkBlockCollider();

            // 4..설치
            if (_nowSelectTempObject != null && _endBuilding)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    F_FinallyBuild();
                    _endBuilding = false;
                }
            }

            // 4-1. 건축 조건 설정
            if (_endBuilding == false)
            {
                yield return null;
                _endBuilding = true;
            }


            yield return null;   // update 효과를 주기위한 코루틴
        }
    }

    private void F_CreatePrefab(GameObject v_ocj)
    {
        if (_nowSelectTempObject == null)
        {
            _nowSelectTempObject = Instantiate(v_ocj);

            // 0. model의 부모 지정
            _modelParent = _nowSelectTempObject.transform.GetChild(0);

            // 1. 콜라이더 끄기
            F_TempOnOffCollider(_modelParent , false);

            // 1-1. 플레이어와 충돌 방지 레이어 설정
            //_modelParent.gameObject.layer = _buildBlockLayer;

            // 1-2. 원래 material를 저정함
            _oriMaterial = _modelParent.GetChild(0).GetComponent<MeshRenderer>().material;

            // 1-3. 생성한 오브젝트의 자식 Model 안의 오브젝트의 material을 바꿈 
            F_TempChangeMaterial(_modelParent, _tempMaterial);
        }
    }

    // 인덱스에 해당하는 오브젝트 반환
    private GameObject F_InstanseTempGameobj(int v_typeIdx, int v_builgIDx)
    {
        // ui 상 내가 선택한 idx에 해당하는 오브젝트 return
        switch (v_typeIdx)
        {
            // type이 0이면 -> floor
            case 0:
                _nowSelectType = SelectBuildType.floor;
                return _floorObject[v_builgIDx];

            // type이 1이면 -> celling
            case 1:
                _nowSelectType = SelectBuildType.celling;
                return _cellingObject[v_builgIDx];

            // type이 2이면 -> wall
            case 2:
                _nowSelectType = SelectBuildType.wall;
                return _wallObject[v_builgIDx];

            // 예외 : null return
            default:
                return null;
        }
    }


    // 마우스 위치에서 ray 쏘기 ( housing ui 가 꺼지면 마우스 커서는 중앙에 위치함)
    private void F_ObjMoveToRay()
    {
        // 카메라 기준으로 ray 쏘기
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit))
        {
            Debug.DrawRay(_hit.point, _hit.normal, Color.green, 3f);
            _nowSelectTempObject.transform.position = _hit.point;
        }
    }

    // temp block의 중심에서 collider 검사
    private void F_checkBlockCollider()
    {
        // 본인 block 이랑 Connecor가 있는 BuildSphere 레이어랑 충돌 검사 
        Collider[] _colls = Physics.OverlapSphere(_nowSelectTempObject.transform.position, 1f, _connectorLayer);

        // 충돌이 있으면?
        if (_colls.Length > 0)
            F_CrashOther(_colls);

        // 충돌이 없으면?
        else
            F_NoneCrash();
    }

    // 내 block 이랑 , 다른 Connector랑 충돌이 일어났을 때 
    private void F_CrashOther(Collider[] v_colliders)
    {
        // v_Colliders 안에는 충돌한 Connector이 담겨있음
        // 1. connector이 접근 가능한가? => 가능하면 내가 snap 할 Connector
        Connector _otherConnector = null;
        foreach (Collider _col in v_colliders)
        {
            Connector temp = _col.GetComponent<Connector>();

            if (temp._canConnect)       // 접근가능하면?
            {
                _otherConnector = temp;
                break;
            }
        }

        // 2. 접근 가능한 connector가 없으면?
        //  && 내가 벽인데 이미 (상대커넥터에) 벽이 설치되어 있다면?
        //  && 내가 바닥인데 (상대커넥터에) 바닥이 설치 되어 있다면?
        if (_otherConnector == null
            || (_nowSelectType == SelectBuildType.wall && _otherConnector._isConnectToWall == true)
            || (_nowSelectType == SelectBuildType.floor && _otherConnector._isConnectToFloor == true)
            )
        {
            // mesh 끄기
            //F_TempOnOffMesh(_modelParent, false);
            _canTempObjectSnap = false;
            return;
        }


        // 3. 스냅 시키기
        F_SnapTempObject(_otherConnector);
    }

    // ** snap **
    public void F_SnapTempObject(Connector _other)
    {

        // 1. 상대 커넥터에 맞는 내 block의 커넥터 찾기
        _myTempConnector = F_FindTempObjectConnector(_other);

        // 2. 내 커넥터의 위치 찾기
        Transform _myConnPosi = F_FindTrsConnector(_nowSelectTempObject.transform.GetChild(1), _myTempConnector);

        // 3. 위치 수정
        _nowSelectTempObject.transform.position
            = _other.transform.position - (_myConnPosi.transform.position - _nowSelectTempObject.transform.position);

        // 3.1. 회전 설정
        // 내가 wall 일 때, -> floor의 커넥터 top, bottom은 90도 회전되어있음
        if (_nowSelectType == SelectBuildType.wall)
        {
            // 내 temp 오브젝트 회전 = 상대 커넥터의 회전
            Quaternion _myRota = _nowSelectTempObject.transform.rotation;
            _myRota.eulerAngles = new Vector3(_myRota.eulerAngles.x, _other.transform.eulerAngles.y, _myRota.eulerAngles.z);

            _nowSelectTempObject.transform.rotation = _myRota;
        }

        // 0. 혹시 꺼져있을 mesh 켜기
        //F_TempOnOffMesh(_modelParent, true);
        // 4. 설치가능
        _canTempObjectSnap = true;

    }

    // 매개변수와 같은 커넥터의 위치를 찾기
    private Transform F_FindTrsConnector(Transform v_trs, ConnectorType v_type)
    {
        foreach (Connector _conn in v_trs.GetComponentsInChildren<Connector>())
        {
            if (_conn._connectorType == v_type)
                return _conn.transform;
        }

        return null;
    }

    // 매개변수로 넘어온 커넥터의 타임에 따라 temp 블럭의 커넥터 찾기
    private ConnectorType F_FindTempObjectConnector(Connector v_other)
    {
        ConnectorType _otherConType = v_other._connectorType;       // 커넥터 타입

        // 0. 내가 wall , 상대가 기본 토대 (floor)
        if (_nowSelectType == SelectBuildType.wall && v_other._selectBuildType == SelectBuildType.defaultFloor)
            return ConnectorType.bottom;

        // 조건 1. 
        // 내가 wall , 상대가 floor
        // => 카메라 위치 , 회전에 따라 달라짐
        /*
        if (_nowSelectType == SelectBuildType.wall && v_other._selectBuildType == SelectBuildType.floor)
        {
            return ConnectorType.bottom;
        }
        */

        // 조건 2.
        // 내가 floor, 상태가 wall
        // => 카메라 위치, wall 회전에 따라 달라짐

        /*
        if (_nowSelectType == SelectBuildType.floor && v_other._selectBuildType == SelectBuildType.wall)
        {
            // 상대 최상위 오브젝트가 회전이 x
            if (v_other.transform.root.rotation.y == 0)
                return F_BuildFloor(false);

            // 상대 최상위 오브젝트 회전 0
            else
                return F_BuildFloor(true);
        }
        

        // 조건 3.
        // 내가 floor, 상대가 floor / 내가 wall , 상대가 wall
        // => 상대 type의 반대 type
        
        switch (_otherConType)
        {
            case ConnectorType.top:
                return ConnectorType.bottom;
            case ConnectorType.bottom:
                return ConnectorType.top;
            case ConnectorType.left:
                return ConnectorType.right;
            case ConnectorType.right:
                return ConnectorType.left;
            default:
                return ConnectorType.bottom;
        }
        */

        return ConnectorType.bottom;
    }

    // 내가 floor일 때 
    private ConnectorType F_BuildFloor(bool v_isRota)
    {
        Transform _cameraPosi = Camera.main.transform;

        // 회전 0
        if (v_isRota)
        {
            // 카메라 x 보다 temp의 x 가 작으면 -> right, 아니면 left
            if (_cameraPosi.position.x > _nowSelectTempObject.transform.position.x)
                return ConnectorType.right;
            else
                return ConnectorType.left;

        }
        // 회전 x
        else
        {
            // 카메라 z 보다 temp의 z 가 작으면 -> top , 아니면 bottom
            if (_cameraPosi.position.z > _nowSelectTempObject.transform.position.z)
                return ConnectorType.top;
            else
                return ConnectorType.bottom;
        }

    }

    // 내 block 이랑 , 다른 Connector랑 충돌이 x
    private void F_NoneCrash()
    {
        // mesh 끄기
        //F_TempOnOffMesh(_modelParent, false);
        // 설치 불가능
        _canTempObjectSnap = false;
    }

    // 최종 build
    private void F_FinallyBuild()
    {
        if (_nowSelectTempObject == null)
            return;

        if (_canTempObjectSnap)     // 오브젝트가 스냅 가능한 상태이면?   
        {
            // 0. 새로 생성해야함! -> 한번 temp 오브젝트 설정하면 계속 그 오브젝트
            // 1. 위치 설정
            GameObject _finalBlock = Instantiate(F_InstanseTempGameobj(_blockTypeIdx, _blockBuilIdx), _nowSelectTempObject.transform.position, _nowSelectTempObject.transform.rotation);
            _finalBlock.name = "새로생긴";

            Destroy(_nowSelectTempObject);
            _nowSelectTempObject = null;

            // 2. material를 원래 material로
            Transform _finalParent = _finalBlock.transform.GetChild(0);
            F_TempChangeMaterial(_finalParent, _oriMaterial);

            // 2-1. 레이어 설정
            //_finalParent.gameObject.layer = 0;

            // 3. 현재 temp 오브젝트의 connector 업데이트
            foreach (Connector _conn1 in _finalBlock.GetComponentsInChildren<Connector>())
            {
                _conn1.F_ConnectUpdate(true);
            }
        }
    }


    // 오브젝트의 material 바꾸기
    private void F_TempChangeMaterial( Transform v_parent , Material v_material) 
    {
        foreach ( MeshRenderer mh in v_parent.GetComponentsInChildren<MeshRenderer>()) 
        {
            mh.material = v_material;
        }
    }

    // 오브젝트의 mesh On/Off
    private void F_TempOnOffMesh( Transform v_parent , bool v_flag) 
    {
        foreach (MeshRenderer mh in v_parent.GetComponentsInChildren<MeshRenderer>())
        {
            mh.enabled = v_flag;
        }
    }

    // 오브젝트의 콜라이더 On/Off
    private void F_TempOnOffCollider( Transform v_parent , bool v_flag) 
    {
        v_parent.GetComponentInParent<Collider>().enabled = v_flag;

    }

    // 만약 건축망치를 놓으면?
    public void F_EndBuildingMode() 
    {
        // 코루틴 멈추기
        StopAllCoroutines();

        // 인덱스를 초기화
        _blockTypeIdx = -1;
        _blockBuilIdx = -1;

        // 임시 블럭 초기화
        _nowSelectTempObject = null;
        _oriMaterial = null;
    }

}
