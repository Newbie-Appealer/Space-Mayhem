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
    [SerializeField] LayerMask _nowTempLayer;       // �׷��� ���� ���̾�
    [SerializeField] LayerMask _tempFloorLayer;     // �ӽ� floor ���̾�
    [SerializeField] int _floorLayer;               // temp floor�� Layer (int)
    [SerializeField] LayerMask _tempWallLayer;      // �ӽ� wall ���̾�  
    [SerializeField] int _wallLayer;                // temp wall�� Layer (int)
    [SerializeField] public LayerMask _finishedLayer;      // �� ���� ���� ���̾�
    [SerializeField] int _buildFinished;            // �� ���� ���� layer (int)
                                                    // 
    [SerializeField] int _dontRaycastLayer;         // temp ��ġ �߿� temp wall&floor�� �浹����
    [SerializeField] int _buildingBlocklayer;       // ���� ���� �ִ� ���� layer (�÷��̾� / �ٸ����� �浹 x)

    [Header( "Type")]
    [SerializeField] private MySelectedBuildType _mySelectBuildType;
   
    [Header("Temp Object Setting")]
    [SerializeField] int _buildTypeIdx;     // ���� Ÿ������
    [SerializeField] int _buildDetailIdx;   // �� Ÿ�Ծ� ���° ������Ʈ ����`

    [Header("Tesmp Object Building")]
    [SerializeField] GameObject _TempObjectBuilding;    // �ӽ� ������Ʈ
    [SerializeField] Transform _TempModelparent;        // �ӽ� ������Ʈ model �θ�
    [SerializeField] Transform _TempFloorParent;        // �ӽ� ������Ʈ �ؿ� tempFloor �θ�
    [SerializeField] Transform _TempWallParent;         // �ӽ� ������Ʈ �ؿ� tempWall �θ�
    [SerializeField] Transform _otehrConnectorTr;       // �浹�� �ٸ� ������Ʈ 

    [Header("Ori Material")]
    [SerializeField] Material _validBuildMaterial;
    [SerializeField] Material _oriMaterial;

    [SerializeField] bool _isTempValidPosition = false;             // �ӽ� ������Ʈ�� ������ �� �ִ���

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
        // ���� ������Ʈ return
        GameObject _currBuild = F_GetCurBuild();
        F_TempRaySetting( _mySelectBuildType );         // �� �� Ÿ�Կ� ���� �˻��� layer ���ϱ�

        while (true) 
        {
            // �޾ƿ°� ����
            F_CreateTempPrefab(_currBuild);

            // layer���� ����ĳ��Ʈ
            F_Raycast(_nowTempLayer);

            // �ݶ��̴� �˻�
            F_CheckCollision();

            // ��ġ
            if (Input.GetMouseButtonDown(0))
                F_BuildTemp();

            yield return null;      // update ȿ��   
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
        // �Ѿ�� Layer�� ���� raycast
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

        // ��ġ ������ ��ġ�� ������
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
        // ��������
        // temp������Ʈ�� null�̵Ǹ� �ٷ� ������!
        if (_TempObjectBuilding == null)
        {
            _TempObjectBuilding = Instantiate(v_temp);

            _TempObjectBuilding.name = "�ӽ� ������Ʈ";

            _TempModelparent = _TempObjectBuilding.transform.GetChild(0);     // temp model
            _TempFloorParent = _TempObjectBuilding.transform.GetChild(1);     // temp floor
            _TempWallParent = _TempObjectBuilding.transform.GetChild (2);     // temp wall

            // Parent ���� ������Ʈ�� ���̾� �ٲٱ�
            F_ChangeChlidLayer( _TempFloorParent , _dontRaycastLayer );        // Tempfloor�� ray �ȵǰ�
            F_ChangeChlidLayer( _TempWallParent, _dontRaycastLayer);           // TempWall�� ray �ȵǰ�

            F_ChangeChlidLayer( _TempModelparent , _buildingBlocklayer );       // model�� layerl ���� , �ٸ��Ͱ� �浹 �ȵǰ�

            //���� material ����
            _oriMaterial = _TempModelparent.GetChild(0).GetComponent<MeshRenderer>().material;

            // Material �ٲٱ�
            F_ChangeMaterial(_TempModelparent , _validBuildMaterial );
        }
    }

    private void F_BuildTemp() 
    { 
        if( _TempObjectBuilding != null && _isTempValidPosition == true) 
        { 
            // ����
            GameObject _nowbuild = Instantiate(F_GetCurBuild(), _TempObjectBuilding.transform.position , _TempObjectBuilding.transform.rotation );

            _nowbuild.gameObject.name = "���λ����� ������Ʈ";

            DestroyImmediate( _TempObjectBuilding);
            _TempObjectBuilding = null;

            // material ����
            F_ChangeMaterial(_nowbuild.transform.GetChild(0) , _oriMaterial);

            // _TempWall & _TempFloor �� ���̾� ���� �ʿ�
            F_ChangeChlidLayer( _nowbuild.transform.GetChild(0) , _buildFinished);      // buildFinished�� ����
            F_ChangeChlidLayer( _nowbuild.transform.GetChild(1) , _floorLayer);         // temp floor�� ����
            F_ChangeChlidLayer( _nowbuild.transform.GetChild(2) , _wallLayer );         // temp walll�� ����

            // �� temp , wall ������Ʈ �浹 ó�� �� ������Ʈ
            foreach ( MyConnector mc in _nowbuild.transform.GetChild(1).GetComponentsInChildren<MyConnector>()) 
            {
                Debug.Log( mc.gameObject.name);
                mc.F_UpdateConnector();
            }
            foreach(MyConnector mc in _nowbuild.transform.GetChild(2).GetComponentsInChildren<MyConnector>()) 
            {
                mc.F_UpdateConnector();
            }

            // ���� connector�� update
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
