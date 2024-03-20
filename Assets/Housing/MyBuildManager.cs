using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
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
    [SerializeField] public LayerMask _buildShpereLayer;
    [SerializeField] public LayerMask _test;

    [SerializeField] public int _buildShpere;
    [SerializeField] public int _buildBlockLayer;
    [SerializeField] public int _dontRayShpere;         // 임시 블럭의 connect를 잠시 다른 레이어로
    [SerializeField] public int _finishedBlock;         // 다 지어진 블럭 ( temp 블럭과 충돌 x )

    [Header( "Type")]
    [SerializeField] private MySelectedBuildType _mySelectBuildType;
   
    [Header("Snap")]    
    [SerializeField] private MyConnectorTpye _MyConnectorTpye;

    [Header("Temp Object Setting")]
    [SerializeField] Material _validBuildMaterial;
    [SerializeField] int _buildTypeIdx;     // 무슨 타입인지
    [SerializeField] int _buildDetailIdx;   // 그 타입안 몇번째 오브젝트 인지`

    [Header("Tesmp Object Building")]
    [SerializeField] GameObject _TempObjectBuilding;        // 임시 오브젝트
    [SerializeField] Transform _modelparent;
    [SerializeField] Transform _connectParent;
    [SerializeField] List<MyConnector> _connectorList;

    [SerializeField] bool _isTempValidPosition;             // 임시 오브젝트가 지어질 수 있는지

    public bool _flag = true;

    private void Start()
    {
        _connectorList = new List<MyConnector>();

        _buildShpere        = LayerMask.NameToLayer("_buildShpereLayer");
        _buildBlockLayer    = LayerMask.NameToLayer("BuildingBlock");           // 현재 짓고 있는 블럭
        _dontRayShpere      = LayerMask.NameToLayer("DontRaycastSphere");       // 현재 짓고 있는 블럭의 connector
        _finishedBlock      = LayerMask.NameToLayer("BuildFInishedBlock");      // 다 지어진 블럭
    }

    private void Update()
    {
        Debug.DrawRay(_player.transform.position, _player.transform.forward * 10f , Color.red);


    }

    public void F_GetbuildType( int v_type , int v_detail) 
    {
        _buildTypeIdx = v_type;
        _buildDetailIdx = v_detail;

        StopAllCoroutines();
        StartCoroutine(F_TEmpBuild());
    }

    IEnumerator F_TEmpBuild() 
    {
        GameObject _currBuild = F_GetCurBuild();
        while (true) 
        {
            F_CreateTempPrefab(_currBuild);

            F_CheckCollier();

            //if(_flag)
                F_MoveTempObjectToRaycast();
            yield return null;      // update 효과   
        }
    }

    private void F_CheckCollier() 
    {
        Collider[] colliders = Physics.OverlapSphere( _TempObjectBuilding.transform.position , 1f , _buildShpereLayer);
        if ( colliders.Length > 0)
        {
            F_TempConnecBuild(colliders);
        }
        else 
        {
            Debug.Log("충돌 된게없음");
            _isTempValidPosition = false;
            F_OnOFfMesh( _modelparent , false );
        }
    }

    private void F_TempConnecBuild(Collider[] v_coll) 
    {
        MyConnector _other = null;

        foreach( Collider mc in v_coll)
        {
            MyConnector _collCon = mc.GetComponent<MyConnector>();    
            if (_collCon._canConnect == true) 
            {
                _other = _collCon;
                break;
            }
        }

        Debug.Log("감지한 다른 커넥터" + _other.transform.position );

        F_OnOFfMesh(_modelparent, true);
        F_SnapTempConnector(_other);
    }

    private void F_MoveTempObjectToRaycast() 
    {

        RaycastHit _hit;

        // building 레이어만 감지, 내 connector들은 
        if (Physics.Raycast(_player.transform.position, _player.transform.forward, out _hit, 30f , _test)) // , _buildShpereLayer
        {
            Debug.Log(_hit.collider.name);
            _TempObjectBuilding.transform.position = _hit.point;

        }
    }

    /*
    public void F_TempBlockTriggerOther( MyConnector v_otherConnet) 
    {
        // 매개변수 : 내 블럭이랑 충돌 난 other 커넥터

        // 1. 연결 못하면
        if (v_otherConnet._canConnect == false)
            return;

        // 2. 내 타입이랑 같으면
        if (_mySelectBuildType == MySelectedBuildType.wall && v_otherConnet._isConnectWall == true
            || _mySelectBuildType == MySelectedBuildType.floor && v_otherConnet._isConnectFloor == true)
            return;

        F_SnapTempConnector( v_otherConnet );

    }*/


    private void F_SnapTempConnector( MyConnector v_type) 
    {

        _MyConnectorTpye = F_FindMyConnecType( v_type._myConnectorType );
        Transform _myTrs = null;

        for (int i = 0; i < _connectorList.Count; i++)
        {
            if (_connectorList[i]._myConnectorType == _MyConnectorTpye)
            {
                _myTrs = _connectorList[i].transform;
            }
        }

        Debug.Log( "내 타입 " + _MyConnectorTpye + " /  상대" + v_type._myConnectorType);

        _TempObjectBuilding.transform.position
            = v_type.transform.position - (_myTrs.position - _TempObjectBuilding.transform.position);


        /*
        Debug.Log("다른타입오브젝트 : " + v_type.transform.position);
        Debug.Log("내 타입 오브젝트 : " + _myTrs);
        Debug.Log( "오브젝트 위치 : " + _TempObjectBuilding.transform.position);
        
        Debug.Log( "이상적인 위치 : " + (v_type.transform.position - (_myTrs.position - _TempObjectBuilding.transform.position)) );
        */

        //_flag = false;

        _isTempValidPosition = true;
    }

    private MyConnectorTpye F_FindMyConnecType( MyConnectorTpye v_other) 
    {
        switch (v_other) 
        {
            case MyConnectorTpye.top:
                return MyConnectorTpye.bottom;
            case MyConnectorTpye.bottom:
                return MyConnectorTpye.top;
            case MyConnectorTpye.left:
                return MyConnectorTpye.right;
            case MyConnectorTpye.right:
                return MyConnectorTpye.left;
            default:
                return MyConnectorTpye.bottom;
        }
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

            // temp 블럭의 block에 있는 스크립트 가져오기
            _modelparent = _TempObjectBuilding.transform.GetChild(0);
            _connectParent = _TempObjectBuilding.transform.GetChild(1);

            foreach ( MyConnector my in _connectParent.GetComponentsInChildren<MyConnector>()) 
            {
                _connectorList.Add(my);
            }

            // 레이어 바꾸기
            F_ChangeLayer( 1, _modelparent , _buildBlockLayer );           // model의 레이어 바꾸기
            F_ChangeLayer( 2, _connectParent , _dontRayShpere , false);    // 커넥터의 레이어 바꾸기

            // Meterial 바꾸기
            F_ChangeMaterial(_modelparent, _validBuildMaterial);
        }
    }

    private void F_ChangeMaterial( Transform v_pa , Material material ) 
    {
        foreach ( MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>()) 
        {
            msr.material = material;
        }
    }

    private void F_OnOFfMesh(Transform v_pa, bool v_flag) 
    {
        foreach (MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>())
        {
            msr.enabled = !v_flag;
        }
    }

    private void F_ChangeLayer( int v_ty,Transform v_pa , int v_n , bool v_flag = true) 
    {
        // 부모 레이어 바꾸기
        if(v_ty == 1)
            v_pa.gameObject.layer = v_n;
        // 부모 밑 자식 레이어 까지 바꾸기
        else
        {
            v_pa.gameObject.layer = v_n;

            for (int i = 0; i < v_pa.childCount; i++) 
            {
                v_pa.GetChild(i).gameObject.layer = v_n;
            }
        }
    }



}
