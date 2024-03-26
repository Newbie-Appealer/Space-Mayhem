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
    [SerializeField] LayerMask _nowTempLayer;       // �׷��� ���� ���̾�
    [SerializeField] int _buildingBlocklayer;       // ���� ���� �ִ� ���� layer (�÷��̾� / �ٸ����� �浹 x)
    [SerializeField] int _buildFinishedLayer;       // �� ���� ���� layer 
    [SerializeField] int _dontRaycastLayer;         // temp ��ġ �߿� temp block�� �浹���� ����
    [SerializeField] List< Tuple<LayerMask , int > > _tempUnderBlockLayer;   // �ӽ� �� ���̾�
        // 0. Temp floor ���̾�
        // 1. Temp celling ���̾�
        // 2. Temp wall ���̾�
        // 3. Temp Ladder ���̾�

    [Header( "Type")]
    [SerializeField] private MySelectedBuildType _mySelectBuildType;
   
    [Header("Temp Object Setting")]
    [SerializeField] int _buildTypeIdx;     // ���� Ÿ������
    [SerializeField] int _buildDetailIdx;   // �� Ÿ�Ծ� ���° ������Ʈ ����
    [SerializeField] bool _isTempValidPosition = false;             // �ӽ� ������Ʈ�� ������ �� �ִ���

    [Header("Tesmp Object Building")]
    [SerializeField] GameObject _TempObjectBuilding;        // �ӽ� ������Ʈ
    [SerializeField] List<Transform> _tempUnderParentTrs;   // �ӽ� ������Ʈ ���� �� �θ� trs
        // 0. �ӽ� ������Ʈ model �θ�
        // 1. `` tempFloor �θ�
        // 2. `` celling �θ�
        // 3. `` tempWall �θ�
        // 4. (wall �϶���) `` ladder �θ� 
    [SerializeField] Transform _otehrConnectorTr;       // �浹�� �ٸ� ������Ʈ 

    [Header("Ori Material")]
    [SerializeField] Material _validBuildMaterial;
    [SerializeField] Material _oriMaterial;

    // ������Ƽ
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
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempFloorLayer") , 17 ),        // temp floor ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempCellingLayer") , 16 ),      // temp celling ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempWallLayer") , 15 ),         // temp wall ���̾�
            
        };

    }

    private void Update()
    {
        Debug.DrawRay(_player.transform.position + new Vector3(0, 1f, 0) , _player.transform.forward * 10f , Color.red);
    }

    public void F_GetbuildType( int v_type = 0 , int v_detail = 1) 
    {
        // 1. index �ʱ�ȭ
        _buildTypeIdx = v_type;
        _buildDetailIdx = v_detail;

        // 2. �ӽ� ������Ʈ Ȯ��
        if (_TempObjectBuilding != null)
            Destroy(_TempObjectBuilding);
        _TempObjectBuilding = null;

        // 3. ���� ���� 
        StopAllCoroutines();
        StartCoroutine(F_TempBuild());
        

    }

    IEnumerator F_TempBuild() 
    {
        // 0. index�� �ش��ϴ� ���� ������Ʈ return
        GameObject _currBuild = F_GetCurBuild();
        // 0.1. �� �� Ÿ�Կ� ���� �˻��� layer ���ϱ�
        F_TempRaySetting( _mySelectBuildType );         

        while (true) 
        {
            // 1. index�� �ش��ϴ� ���ӿ�����Ʈ ����
            F_CreateTempPrefab(_currBuild);

            // 2. �ش� ���̶� ���� Layer�� raycast
            F_Raycast(_nowTempLayer);

            // 3. temp ������Ʈ�� �ݶ��̴� �˻�
            F_CheckCollision();

            // 4. ��ġ
            if (Input.GetMouseButtonDown(0))
                F_BuildTemp();

            // update ȿ�� 
            yield return null;     
        }
    }


    private void F_Raycast( LayerMask v_layer ) 
    {
        // �Ѿ�� Layer�� ���� raycast
        RaycastHit _hit;

        if (Physics.Raycast( _player.transform.position + new Vector3(0,1f,0) , _player.transform.forward * 10 , out _hit , 5f , v_layer)) // Ÿ�� : LayerMask
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
                _nowTempLayer = _tempUnderBlockLayer[0].Item1;          // floor ���̾�
                break;
            case MySelectedBuildType.celling:
                _nowTempLayer = _tempUnderBlockLayer[1].Item1;          // celling ���̾�
                break;
            case MySelectedBuildType.wall:                              // window, door, window�� ���� wall ���̾� ��� 
            case MySelectedBuildType.door:
            case MySelectedBuildType.window:
                _nowTempLayer = _tempUnderBlockLayer[2].Item1;          // wall ���̾�
                break;
        }

    }


    private void F_CreateTempPrefab( GameObject v_temp) 
    {
        // ��������
        // temp������Ʈ�� null�̵Ǹ� �ٷ� ������!
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

            // �� Parent ���� ������Ʈ�� ���̾� �ٲٱ�
            for (int i = 1; i < _tempUnderParentTrs.Count; i++) 
            {
                F_ChangeLayer(_tempUnderParentTrs[i], _dontRaycastLayer);        
            }  

            F_ChangeLayer(_tempUnderParentTrs[0], _buildingBlocklayer , true);       // model�� layerl ���� , �ٸ��Ͱ� �浹 �ȵǰ�

            //���� material ����
            _oriMaterial = _tempUnderParentTrs[0].GetChild(0).GetComponent<MeshRenderer>().material;

            // Material �ٲٱ�
            F_ChangeMaterial( _tempUnderParentTrs[0], _validBuildMaterial );
        }
    }

    private void F_BuildTemp() 
    { 
        if( _TempObjectBuilding != null && _isTempValidPosition == true) 
        { 
            // ����
            GameObject _nowbuild = Instantiate(F_GetCurBuild(), _TempObjectBuilding.transform.position , _TempObjectBuilding.transform.rotation );

            Destroy( _TempObjectBuilding);
            _TempObjectBuilding = null;

            // material ����
            F_ChangeMaterial(_nowbuild.transform.GetChild(0) , _oriMaterial);

            // buildFinished�� ����
            F_ChangeLayer( _nowbuild.transform.GetChild(0) , _buildFinishedLayer);

            // �� ������Ʈ�� �´� temp ���̾�� ��ȯ
            for (int i = 1; i < _nowbuild.transform.childCount - 1; i++) 
            {
                F_ChangeLayer( _nowbuild.transform.GetChild(i) , _tempUnderBlockLayer[i-1].Item2 );   
            }

            // �� temp ������Ʈ �浹 ó�� �� Connector ������Ʈ
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

            // ���� ���� �浹�� connector�� update
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

    // �Ǽ� ���� ������ �� �ʱ�ȭ �Լ�
    

}
