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
    [SerializeField] public int _dontRayShpere;         // �ӽ� ���� connect�� ��� �ٸ� ���̾��
    [SerializeField] public int _finishedBlock;         // �� ������ �� ( temp ���� �浹 x )

    [Header( "Type")]
    [SerializeField] private MySelectedBuildType _mySelectBuildType;
   
    [Header("Snap")]    
    [SerializeField] private MyConnectorTpye _MyConnectorTpye;

    [Header("Temp Object Setting")]
    [SerializeField] Material _validBuildMaterial;
    [SerializeField] int _buildTypeIdx;     // ���� Ÿ������
    [SerializeField] int _buildDetailIdx;   // �� Ÿ�Ծ� ���° ������Ʈ ����`

    [Header("Tesmp Object Building")]
    [SerializeField] GameObject _TempObjectBuilding;        // �ӽ� ������Ʈ
    [SerializeField] Transform _modelparent;
    [SerializeField] Transform _connectParent;
    [SerializeField] List<MyConnector> _connectorList;

    [SerializeField] bool _isTempValidPosition;             // �ӽ� ������Ʈ�� ������ �� �ִ���

    public bool _flag = true;

    private void Start()
    {
        _connectorList = new List<MyConnector>();

        _buildShpere        = LayerMask.NameToLayer("_buildShpereLayer");
        _buildBlockLayer    = LayerMask.NameToLayer("BuildingBlock");           // ���� ���� �ִ� ��
        _dontRayShpere      = LayerMask.NameToLayer("DontRaycastSphere");       // ���� ���� �ִ� ���� connector
        _finishedBlock      = LayerMask.NameToLayer("BuildFInishedBlock");      // �� ������ ��
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
            yield return null;      // update ȿ��   
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
            Debug.Log("�浹 �ȰԾ���");
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

        Debug.Log("������ �ٸ� Ŀ����" + _other.transform.position );

        F_OnOFfMesh(_modelparent, true);
        F_SnapTempConnector(_other);
    }

    private void F_MoveTempObjectToRaycast() 
    {

        RaycastHit _hit;

        // building ���̾ ����, �� connector���� 
        if (Physics.Raycast(_player.transform.position, _player.transform.forward, out _hit, 30f , _test)) // , _buildShpereLayer
        {
            Debug.Log(_hit.collider.name);
            _TempObjectBuilding.transform.position = _hit.point;

        }
    }

    /*
    public void F_TempBlockTriggerOther( MyConnector v_otherConnet) 
    {
        // �Ű����� : �� ���̶� �浹 �� other Ŀ����

        // 1. ���� ���ϸ�
        if (v_otherConnet._canConnect == false)
            return;

        // 2. �� Ÿ���̶� ������
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

        Debug.Log( "�� Ÿ�� " + _MyConnectorTpye + " /  ���" + v_type._myConnectorType);

        _TempObjectBuilding.transform.position
            = v_type.transform.position - (_myTrs.position - _TempObjectBuilding.transform.position);


        /*
        Debug.Log("�ٸ�Ÿ�Կ�����Ʈ : " + v_type.transform.position);
        Debug.Log("�� Ÿ�� ������Ʈ : " + _myTrs);
        Debug.Log( "������Ʈ ��ġ : " + _TempObjectBuilding.transform.position);
        
        Debug.Log( "�̻����� ��ġ : " + (v_type.transform.position - (_myTrs.position - _TempObjectBuilding.transform.position)) );
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
        // ��������
        // temp������Ʈ�� null�̵Ǹ� �ٷ� ������!
        if (_TempObjectBuilding == null)
        {
            
            _TempObjectBuilding = Instantiate(v_temp);

            // temp ���� block�� �ִ� ��ũ��Ʈ ��������
            _modelparent = _TempObjectBuilding.transform.GetChild(0);
            _connectParent = _TempObjectBuilding.transform.GetChild(1);

            foreach ( MyConnector my in _connectParent.GetComponentsInChildren<MyConnector>()) 
            {
                _connectorList.Add(my);
            }

            // ���̾� �ٲٱ�
            F_ChangeLayer( 1, _modelparent , _buildBlockLayer );           // model�� ���̾� �ٲٱ�
            F_ChangeLayer( 2, _connectParent , _dontRayShpere , false);    // Ŀ������ ���̾� �ٲٱ�

            // Meterial �ٲٱ�
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
        // �θ� ���̾� �ٲٱ�
        if(v_ty == 1)
            v_pa.gameObject.layer = v_n;
        // �θ� �� �ڽ� ���̾� ���� �ٲٱ�
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
