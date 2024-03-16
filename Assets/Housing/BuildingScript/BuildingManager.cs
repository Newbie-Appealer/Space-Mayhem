using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public enum SelectBuildType 
{
    floor,
    celling,
    wall
}

[System.Serializable]
public enum ConnectorType 
{
    top, bottom, left, right
}

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    [Header("Player")]
    public GameObject _player;

    [Header("Build Setting")]
    [SerializeField] public LayerMask _connectorLayer;  // connector�� �ִ� ���̾�� ����
    [SerializeField] int _buildBlockLayer;              // buildBlock ���̾�
    [SerializeField] int _deFaultLayer = 0;             // defult ���̾�

    [Header("building object")]
    [SerializeField] GameObject[] _floorObject;                      // �ٴ� ������Ʈ
    [SerializeField] GameObject[] _cellingObject;                    // õ�� ������Ʈ
    [SerializeField] GameObject[] _wallObject;                       // �� ������Ʈ

    [Header("now Select Object")]
    [SerializeField]
    GameObject _nowSelectTempObject;                // ���� ���� �� �ӽ� ������Ʈ
    [SerializeField]
    Transform _modelParent;                         // �ӽ� ������Ʈ�� model 
    [SerializeField]
    SelectBuildType _nowSelectType;                 // ���� ���� �� �ӽ� ������Ʈ�� Ÿ�� ����
    [SerializeField]
    bool _canTempObjectSnap = false;                // �ӽ� ������Ʈ�� snap ��������?
    [SerializeField]
    bool _checkUpdateFlag = true;                   // update ������ ���ǹ�

    [Header(" Material ")]
    [SerializeField] Material _tempMaterial;                         // �ӽ� ������Ʈ�� material
    [SerializeField] Material _oriMaterial;                          // ���� material

    private void Start()
    {
        instance = this;        // �̱���

        // layer num ã��
        _buildBlockLayer = LayerMask.NameToLayer("BuildingBlock");
    }

    private void Update()
    {
        // �÷��̾� ������ ray ���
        Debug.DrawRay(_player.transform.position, _player.transform.forward * 10, Color.red);

        // ���� build �� ������Ʈ�� �ְ�
        // ��Ŭ���� ���� �� -> ��ġ
        if (_checkUpdateFlag && _nowSelectTempObject != null) 
        {
            if (Input.GetMouseButton(0)) 
            {
                // ��ġ�ϴ� �Լ� �߰�
                F_FinallyBuild();
            }
        }
    }

    // HousingUiManager���� ���콺 ��Ŭ���� ���� ����
    public void F_startBuiling(int v_typeIdx, int v_builgIdx)  
    {
        // ���� ui �� �ƹ��͵� ���� �� �Ǿ�������
        if (v_typeIdx == -1 && v_builgIdx == -1)
            return;

        // 1. ��ȯ�� ������Ʈ�� instantiate ��
        _nowSelectTempObject = Instantiate(F_InstanseTempGameobj( v_typeIdx, v_builgIdx) );
        _modelParent = _nowSelectTempObject.transform.GetChild(0);

        // 1-1. �÷��̾�� �浹 ���� ���̾� ����
        _nowSelectTempObject.layer = _buildBlockLayer;

        // 1-2. ���� material�� ������
        _oriMaterial = _modelParent.GetChild(0).GetComponent<MeshRenderer>().material;

        // 1-3. ������ ������Ʈ�� �ڽ� Model ���� ������Ʈ�� material�� �ٲ� 
        F_TempChangeMaterial( _modelParent , _tempMaterial );

        // 2. ������Ʈ�� ȭ��� ��� ( ray ���)
        if (_nowSelectTempObject != null)
            StartCoroutine(F_SetTempBuildingObj());


    }

    // �ε����� �ش��ϴ� ������Ʈ ��ȯ
    private GameObject F_InstanseTempGameobj(int v_typeIdx, int v_builgIDx) 
    {
        // ui �� ���� ������ idx�� �ش��ϴ� ������Ʈ return
        switch (v_typeIdx) 
        {
            // type�� 0�̸� -> wall
            case 0:
                _nowSelectType = SelectBuildType.wall;
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

    // ray�� ������Ʈ ����
    IEnumerator F_SetTempBuildingObj() 
    {
        while (true) 
        {
            F_ObjMoveToRay();                           // ������Ʈ�� ray�� ���� �����̰�
            F_checkBlockCollider();                     // block�� �ݶ��̴� �˻�

            yield return new WaitForSeconds(0.01f);     // update ȿ���� �ֱ����� �ڷ�ƾ
        }
    }

    // ���콺 ��ġ���� ray ��� ( housing ui �� ������ ���콺 Ŀ���� �߾ӿ� ��ġ��)
    private void F_ObjMoveToRay() 
    {
        // ī�޶� �������� ray ���
        Debug.Log("ray �� �����̴°� ���� ��");
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit))
        {
            _nowSelectTempObject.transform.position = _hit.point;
        }
    }

    // temp block�� �߽ɿ��� collider �˻�
    private void F_checkBlockCollider()
    {
        // ���� block �̶� Connecor�� �ִ� BuildSphere ���̾�� �浹 �˻� 
        Collider[] _colls = Physics.OverlapSphere(transform.position, 1f , _connectorLayer);

        // �浹�� ������?
        if (_colls.Length > 0)
            F_CrashOther(_colls);
        // �浹�� ������?
        else
            F_NoneCrash();
    }

    // �� block �̶� , �ٸ� Connector�� �浹�� �Ͼ�� �� 
    private void F_CrashOther(Collider[] v_colliders ) 
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
            && (_nowSelectType == SelectBuildType.wall && !_otherConnector._canConnectToWall)
            && (_nowSelectType == SelectBuildType.floor && !_otherConnector._canConnectToFloor)
            )
        {
            // mesh ����
            F_TempOnOffMesh(_modelParent, false);
            return;
        }

        // 3. ���� ��Ű��
        F_SnapTempObject(_otherConnector);
    }

    // ** snap **
    public void F_SnapTempObject( Connector _other ) 
    {
        // 0. Ȥ�� �������� mesh �ѱ�
        F_TempOnOffMesh( _modelParent , true );

        // 1. ��� Ŀ���Ϳ� �´� �� block�� Ŀ���� ã��
        ConnectorType _myTempConnector = F_FindTempObjectConnector(_other);

        // 2. �� Ŀ������ ��ġ ã��
        Transform _myConnPosi = F_FindTrsConnector( _nowSelectTempObject.transform.GetChild(1) , _myTempConnector );

        // 3. ��ġ ����
        _nowSelectTempObject.transform.position
            = _other.transform.position - (_myConnPosi.position - _nowSelectTempObject.transform.position);

        // 3.1. ȸ�� ����
        // ���� wall �� ��, -> floor�� Ŀ���� top, bottom�� 90�� ȸ���Ǿ�����
        if ( _nowSelectType == SelectBuildType.wall ) 
        {
            // �� temp ������Ʈ ȸ�� = ��� Ŀ������ ȸ��
            Quaternion _myRota = _nowSelectTempObject.transform.rotation;
            _myRota.eulerAngles = _other.transform.rotation.eulerAngles;

            _nowSelectTempObject.transform.eulerAngles = _myRota.eulerAngles;
        }

        // 4. ��ġ����
        _canTempObjectSnap = true;

    }

    // �Ű������� ���� Ŀ������ ��ġ�� ã��
    private Transform F_FindTrsConnector( Transform v_trs , ConnectorType v_type) 
    {
        foreach ( Connector _conn in v_trs.GetComponentsInChildren<Connector>()) 
        {
            if (_conn._connectorType == v_type)
                return _conn.transform;
        }

        return null;
    }

    // �Ű������� �Ѿ�� Ŀ������ Ÿ�ӿ� ���� temp ���� Ŀ���� ã��
    private ConnectorType F_FindTempObjectConnector( Connector v_other) 
    {
        ConnectorType _otherConType     = v_other._connectorType;       // Ŀ���� Ÿ��
        SelectBuildType _otherBuiltType = v_other._selectBuildType;     // build Ÿ��

        // ���� 1. 
        // ���� wall , ��밡 floor
        // => ī�޶� ��ġ , ȸ���� ���� �޶���
        if (_nowSelectType == SelectBuildType.wall && _otherBuiltType == SelectBuildType.floor)
            return ConnectorType.bottom;

        // ���� 2.
        // ���� floor, ���°� wall
        // => ī�޶� ��ġ, wall ȸ���� ���� �޶���
        if (_nowSelectType == SelectBuildType.floor && _otherBuiltType == SelectBuildType.wall)
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
            case ConnectorType.top :
                return ConnectorType.bottom;
            case ConnectorType.bottom :
                return ConnectorType.top;
            case ConnectorType.left :
                return ConnectorType.right;
            case ConnectorType.right :
                return ConnectorType.left;
        }

        return _otherConType;
    }

    // ���� floor�� �� 
    private ConnectorType F_BuildFloor( bool v_isRota) 
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
        F_TempOnOffMesh( _modelParent , false);
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
            // temp ������Ʈ�� ���� �� ��ġ��
            float x = _nowSelectTempObject.transform.position.x;
            float y = _nowSelectTempObject.transform.position.y;
            float z = _nowSelectTempObject.transform.position.z;

            // 1. ��ġ ����
            _nowSelectTempObject.transform.position = new Vector3(x, y, z);

            // 2. material�� ���� material��
            F_TempChangeMaterial(_modelParent, _oriMaterial);

            // 2-1. ���̾� ����
            _nowSelectTempObject.layer = _deFaultLayer;

            // 3. ���� temp ������Ʈ�� connector ������Ʈ
            Transform _connectParent = _nowSelectTempObject.transform.GetChild(1);
            foreach (Connector _conn in _connectParent.GetComponentsInChildren<Connector>())
            {
                _conn.F_ConnectUpdate(true);  
            }

            // 4. �� �ٲ����� null ��
            _nowSelectTempObject = null;
            _oriMaterial = null;

            // 5. update�� ���� ����
            _checkUpdateFlag = false;         
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

}
