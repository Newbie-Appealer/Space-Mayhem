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
    [SerializeField] public LayerMask _connectorLayer;  // connector�� �ִ� ���̾�� ����
    [SerializeField] int _buildBlockLayer;              // buildBlock ���̾�

    [Header("building object")]
    [SerializeField] GameObject[] _floorObject;                      // �ٴ� ������Ʈ
    [SerializeField] GameObject[] _cellingObject;                    // õ�� ������Ʈ
    [SerializeField] GameObject[] _wallObject;                       // �� ������Ʈ

    [Header("now Select Object")]
    [SerializeField]
    SelectBuildType _nowSelectType;                 // ���� ���� �� �ӽ� ������Ʈ�� Ÿ�� ����
    [SerializeField]
    ConnectorType _myTempConnector;                 // ���� ���õ� Ŀ���� Ÿ��
    [SerializeField]
    GameObject _nowSelectTempObject;                // ���� ���� �� �ӽ� ������Ʈ
    [SerializeField]
    Transform _modelParent;                         // �ӽ� ������Ʈ�� model 
    [SerializeField]
    bool _canTempObjectSnap = false;                // �ӽ� ������Ʈ�� snap ��������?
    [SerializeField]
    public string _nowSelectBlockName;              // ���� ���õ� ���� �̸� (�ӽú��� �̸�)

    [Header(" Material ")]
    [SerializeField] Material _tempMaterial;                         // �ӽ� ������Ʈ�� material
    [SerializeField] Material _oriMaterial;                          // ���� material

    [Header("���� �� ��ġ idx")]
    [SerializeField] private int _blockTypeIdx = -1;
    [SerializeField] private int _blockBuilIdx = -1;

    //[SerializeField] bool _startBuilding = true;    // building ���� ���� ( )
    [SerializeField] bool _endBuilding = true;      // building �� ����
    [SerializeField] bool _blockisMove;             // ���� snap �� ����?

    private void Start()
    {
        instance = this;        // �̱���

        // layer num ã��
        _buildBlockLayer    = LayerMask.NameToLayer("BuildingBlock");

    }

    private void Update()
    {
        // �÷��̾� ������ ray ���
        Debug.DrawRay(_player.transform.position, _player.transform.forward * 10, Color.red);
    }

    // HousingUiManager���� ���콺 ��Ŭ���� ���� ����
    public void F_BuildingTypeNBlockiIdx(int v_typeIdx, int v_builgIdx)
    {
        // 0. �ε��� ����
        _blockTypeIdx = v_typeIdx;
        _blockBuilIdx = v_builgIdx;

        // 1. ���� ����
        _endBuilding = true;

        // 2. ���� ���� ����
        _canTempObjectSnap = false;

        // 3. ���� ������Ʈ ������ destroy
        if (_nowSelectTempObject != null)
            Destroy( _nowSelectTempObject );
        _nowSelectTempObject = null;

        // 4. ���ο� ���� ��ġ �� �� ( ���� ��ġ �ǰ��ִ� ���� �������� )
        StopAllCoroutines();    // ������ �����ϴ� �ڷ�ƾ ���߱�

        // 5. building ����
        StartCoroutine(F_StartBuild());
    }

    IEnumerator F_StartBuild()
    {
        while (true)
        {
            // 0. ������ ������Ʈ return
            GameObject curr = F_InstanseTempGameobj(_blockTypeIdx, _blockBuilIdx);
            curr.name = "NowTempBlock";
            _nowSelectBlockName = curr.name;

            // 1. return ���� ������Ʈ ����
            F_CreatePrefab(curr);

            // 2. ������Ʈ�� ray�� ���� �����̰�
            F_ObjMoveToRay();
            // 3. block�� �ݶ��̴� �˻�
            F_checkBlockCollider();

            // 4..��ġ
            if (_nowSelectTempObject != null && _endBuilding)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    F_FinallyBuild();
                    _endBuilding = false;
                }
            }

            // 4-1. ���� ���� ����
            if (_endBuilding == false)
            {
                yield return null;
                _endBuilding = true;
            }


            yield return null;   // update ȿ���� �ֱ����� �ڷ�ƾ
        }
    }

    private void F_CreatePrefab(GameObject v_ocj)
    {
        if (_nowSelectTempObject == null)
        {
            _nowSelectTempObject = Instantiate(v_ocj);

            // 0. model�� �θ� ����
            _modelParent = _nowSelectTempObject.transform.GetChild(0);

            // 1. �ݶ��̴� ����
            F_TempOnOffCollider(_modelParent , false);

            // 1-1. �÷��̾�� �浹 ���� ���̾� ����
            //_modelParent.gameObject.layer = _buildBlockLayer;

            // 1-2. ���� material�� ������
            _oriMaterial = _modelParent.GetChild(0).GetComponent<MeshRenderer>().material;

            // 1-3. ������ ������Ʈ�� �ڽ� Model ���� ������Ʈ�� material�� �ٲ� 
            F_TempChangeMaterial(_modelParent, _tempMaterial);
        }
    }

    // �ε����� �ش��ϴ� ������Ʈ ��ȯ
    private GameObject F_InstanseTempGameobj(int v_typeIdx, int v_builgIDx)
    {
        // ui �� ���� ������ idx�� �ش��ϴ� ������Ʈ return
        switch (v_typeIdx)
        {
            // type�� 0�̸� -> floor
            case 0:
                _nowSelectType = SelectBuildType.floor;
                return _floorObject[v_builgIDx];

            // type�� 1�̸� -> celling
            case 1:
                _nowSelectType = SelectBuildType.celling;
                return _cellingObject[v_builgIDx];

            // type�� 2�̸� -> wall
            case 2:
                _nowSelectType = SelectBuildType.wall;
                return _wallObject[v_builgIDx];

            // ���� : null return
            default:
                return null;
        }
    }


    // ���콺 ��ġ���� ray ��� ( housing ui �� ������ ���콺 Ŀ���� �߾ӿ� ��ġ��)
    private void F_ObjMoveToRay()
    {
        // ī�޶� �������� ray ���
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit))
        {
            Debug.DrawRay(_hit.point, _hit.normal, Color.green, 3f);
            _nowSelectTempObject.transform.position = _hit.point;
        }
    }

    // temp block�� �߽ɿ��� collider �˻�
    private void F_checkBlockCollider()
    {
        // ���� block �̶� Connecor�� �ִ� BuildSphere ���̾�� �浹 �˻� 
        Collider[] _colls = Physics.OverlapSphere(_nowSelectTempObject.transform.position, 1f, _connectorLayer);

        // �浹�� ������?
        if (_colls.Length > 0)
            F_CrashOther(_colls);

        // �浹�� ������?
        else
            F_NoneCrash();
    }

    // �� block �̶� , �ٸ� Connector�� �浹�� �Ͼ�� �� 
    private void F_CrashOther(Collider[] v_colliders)
    {
        // v_Colliders �ȿ��� �浹�� Connector�� �������
        // 1. connector�� ���� �����Ѱ�? => �����ϸ� ���� snap �� Connector
        Connector _otherConnector = null;
        foreach (Collider _col in v_colliders)
        {
            Connector temp = _col.GetComponent<Connector>();

            if (temp._canConnect)       // ���ٰ����ϸ�?
            {
                _otherConnector = temp;
                break;
            }
        }

        // 2. ���� ������ connector�� ������?
        //  && ���� ���ε� �̹� (���Ŀ���Ϳ�) ���� ��ġ�Ǿ� �ִٸ�?
        //  && ���� �ٴ��ε� (���Ŀ���Ϳ�) �ٴ��� ��ġ �Ǿ� �ִٸ�?
        if (_otherConnector == null
            || (_nowSelectType == SelectBuildType.wall && _otherConnector._isConnectToWall == true)
            || (_nowSelectType == SelectBuildType.floor && _otherConnector._isConnectToFloor == true)
            )
        {
            // mesh ����
            //F_TempOnOffMesh(_modelParent, false);
            _canTempObjectSnap = false;
            return;
        }


        // 3. ���� ��Ű��
        F_SnapTempObject(_otherConnector);
    }

    // ** snap **
    public void F_SnapTempObject(Connector _other)
    {

        // 1. ��� Ŀ���Ϳ� �´� �� block�� Ŀ���� ã��
        _myTempConnector = F_FindTempObjectConnector(_other);

        // 2. �� Ŀ������ ��ġ ã��
        Transform _myConnPosi = F_FindTrsConnector(_nowSelectTempObject.transform.GetChild(1), _myTempConnector);

        // 3. ��ġ ����
        _nowSelectTempObject.transform.position
            = _other.transform.position - (_myConnPosi.transform.position - _nowSelectTempObject.transform.position);

        // 3.1. ȸ�� ����
        // ���� wall �� ��, -> floor�� Ŀ���� top, bottom�� 90�� ȸ���Ǿ�����
        if (_nowSelectType == SelectBuildType.wall)
        {
            // �� temp ������Ʈ ȸ�� = ��� Ŀ������ ȸ��
            Quaternion _myRota = _nowSelectTempObject.transform.rotation;
            _myRota.eulerAngles = new Vector3(_myRota.eulerAngles.x, _other.transform.eulerAngles.y, _myRota.eulerAngles.z);

            _nowSelectTempObject.transform.rotation = _myRota;
        }

        // 0. Ȥ�� �������� mesh �ѱ�
        //F_TempOnOffMesh(_modelParent, true);
        // 4. ��ġ����
        _canTempObjectSnap = true;

    }

    // �Ű������� ���� Ŀ������ ��ġ�� ã��
    private Transform F_FindTrsConnector(Transform v_trs, ConnectorType v_type)
    {
        foreach (Connector _conn in v_trs.GetComponentsInChildren<Connector>())
        {
            if (_conn._connectorType == v_type)
                return _conn.transform;
        }

        return null;
    }

    // �Ű������� �Ѿ�� Ŀ������ Ÿ�ӿ� ���� temp ���� Ŀ���� ã��
    private ConnectorType F_FindTempObjectConnector(Connector v_other)
    {
        ConnectorType _otherConType = v_other._connectorType;       // Ŀ���� Ÿ��

        // 0. ���� wall , ��밡 �⺻ ��� (floor)
        if (_nowSelectType == SelectBuildType.wall && v_other._selectBuildType == SelectBuildType.defaultFloor)
            return ConnectorType.bottom;

        // ���� 1. 
        // ���� wall , ��밡 floor
        // => ī�޶� ��ġ , ȸ���� ���� �޶���
        /*
        if (_nowSelectType == SelectBuildType.wall && v_other._selectBuildType == SelectBuildType.floor)
        {
            return ConnectorType.bottom;
        }
        */

        // ���� 2.
        // ���� floor, ���°� wall
        // => ī�޶� ��ġ, wall ȸ���� ���� �޶���

        /*
        if (_nowSelectType == SelectBuildType.floor && v_other._selectBuildType == SelectBuildType.wall)
        {
            // ��� �ֻ��� ������Ʈ�� ȸ���� x
            if (v_other.transform.root.rotation.y == 0)
                return F_BuildFloor(false);

            // ��� �ֻ��� ������Ʈ ȸ�� 0
            else
                return F_BuildFloor(true);
        }
        

        // ���� 3.
        // ���� floor, ��밡 floor / ���� wall , ��밡 wall
        // => ��� type�� �ݴ� type
        
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

    // ���� floor�� �� 
    private ConnectorType F_BuildFloor(bool v_isRota)
    {
        Transform _cameraPosi = Camera.main.transform;

        // ȸ�� 0
        if (v_isRota)
        {
            // ī�޶� x ���� temp�� x �� ������ -> right, �ƴϸ� left
            if (_cameraPosi.position.x > _nowSelectTempObject.transform.position.x)
                return ConnectorType.right;
            else
                return ConnectorType.left;

        }
        // ȸ�� x
        else
        {
            // ī�޶� z ���� temp�� z �� ������ -> top , �ƴϸ� bottom
            if (_cameraPosi.position.z > _nowSelectTempObject.transform.position.z)
                return ConnectorType.top;
            else
                return ConnectorType.bottom;
        }

    }

    // �� block �̶� , �ٸ� Connector�� �浹�� x
    private void F_NoneCrash()
    {
        // mesh ����
        //F_TempOnOffMesh(_modelParent, false);
        // ��ġ �Ұ���
        _canTempObjectSnap = false;
    }

    // ���� build
    private void F_FinallyBuild()
    {
        if (_nowSelectTempObject == null)
            return;

        if (_canTempObjectSnap)     // ������Ʈ�� ���� ������ �����̸�?   
        {
            // 0. ���� �����ؾ���! -> �ѹ� temp ������Ʈ �����ϸ� ��� �� ������Ʈ
            // 1. ��ġ ����
            GameObject _finalBlock = Instantiate(F_InstanseTempGameobj(_blockTypeIdx, _blockBuilIdx), _nowSelectTempObject.transform.position, _nowSelectTempObject.transform.rotation);
            _finalBlock.name = "���λ���";

            Destroy(_nowSelectTempObject);
            _nowSelectTempObject = null;

            // 2. material�� ���� material��
            Transform _finalParent = _finalBlock.transform.GetChild(0);
            F_TempChangeMaterial(_finalParent, _oriMaterial);

            // 2-1. ���̾� ����
            //_finalParent.gameObject.layer = 0;

            // 3. ���� temp ������Ʈ�� connector ������Ʈ
            foreach (Connector _conn1 in _finalBlock.GetComponentsInChildren<Connector>())
            {
                _conn1.F_ConnectUpdate(true);
            }
        }
    }


    // ������Ʈ�� material �ٲٱ�
    private void F_TempChangeMaterial( Transform v_parent , Material v_material) 
    {
        foreach ( MeshRenderer mh in v_parent.GetComponentsInChildren<MeshRenderer>()) 
        {
            mh.material = v_material;
        }
    }

    // ������Ʈ�� mesh On/Off
    private void F_TempOnOffMesh( Transform v_parent , bool v_flag) 
    {
        foreach (MeshRenderer mh in v_parent.GetComponentsInChildren<MeshRenderer>())
        {
            mh.enabled = v_flag;
        }
    }

    // ������Ʈ�� �ݶ��̴� On/Off
    private void F_TempOnOffCollider( Transform v_parent , bool v_flag) 
    {
        v_parent.GetComponentInParent<Collider>().enabled = v_flag;

    }

    // ���� �����ġ�� ������?
    public void F_EndBuildingMode() 
    {
        // �ڷ�ƾ ���߱�
        StopAllCoroutines();

        // �ε����� �ʱ�ȭ
        _blockTypeIdx = -1;
        _blockBuilIdx = -1;

        // �ӽ� �� �ʱ�ȭ
        _nowSelectTempObject = null;
        _oriMaterial = null;
    }

}
