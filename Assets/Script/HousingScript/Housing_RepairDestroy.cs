using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Color = UnityEngine.Color;
using ColorUtility = UnityEngine.ColorUtility;

enum RepairDestory
{
    Repair,
    Destroy
} 

public class Housing_RepairDestroy : MonoBehaviour
{
    [Header("===Connector===")]
    [SerializeField] private Connector[] _connectorContainer;
    [SerializeField] private Connector _currConnector;
    public GameObject _connectorObject;

    [Header("===OutLine===")]
    private GameObject _outlineObject = default;
    private List<Tuple<Color, float>> _outlineData;

    private void Awake()
    {
        // 1. outline
        _outlineData = new List<Tuple<Color, float>>
        {
            new Tuple<Color,float>(new Color(0.4f, 0.8f, 0.2f) , 10f ),     // repair
            new Tuple<Color,float>(new Color(0.8f, 0.4f, 0.2f) , 10f)       // destory 
        };
    }

    // Connector ���� , HousingDataManager���� ����� 
    public void F_SetConnArr(Connector con1, Connector con2, Connector con3, Connector con4)
    {
        _connectorContainer = new Connector[System.Enum.GetValues(typeof(ConnectorGroupType)).Length];       // Ŀ���� Ÿ�Ը�ŭ �迭 ����

        _connectorContainer[ (int)ConnectorGroupType.FloorConnectorGroup ]        = con1;
        _connectorContainer[ (int)ConnectorGroupType.CellingConnectorGroup ]      = con2;
        _connectorContainer[ (int)ConnectorGroupType.BasicWallConnectorGroup ]    = con3;
        _connectorContainer[ (int)ConnectorGroupType.RotatedWallConnnectorGroup ] = con4;
        _connectorContainer[ (int)ConnectorGroupType.None ]                       = Connector.Defalt;
    }

    // housing ���� �������� ��, outlind object �ʱ�ȭ 
    public void F_InitOutlineObject() 
    {
        // 0. ������Ʈ �ƿ����� ����
        if (_outlineObject != null)
            _outlineObject.GetComponent<ObjectOutline>().enabled = false;
        _outlineObject = null;

        // 1. repair text ����
        BuildMaster.Instance.housingUiManager.F_OnOffRepairText(null, false);
    }

    // ���� & �ı����� ���� 
    public void F_RepairAndDestroyTool( int v_detailIdx ,LayerMask v_currLayer )
    {
        RaycastHit _hit;

        // 0. outline ȿ�� 
        if (Physics.Raycast(BuildMaster.Instance._playerCamera.transform.position,
                BuildMaster.Instance._playerCamera.transform.forward * 10, out _hit, 5f, v_currLayer))   // Ÿ�� : LayerMask
        {
            if(_outlineObject == null)
                _outlineObject = _hit.collider.gameObject.transform.parent.transform.parent.gameObject;

            if (!System.Object.ReferenceEquals(_outlineObject, _hit.collider.gameObject) && _outlineObject != null )
            {
                _outlineObject.GetComponent<ObjectOutline>().enabled = false;
                _outlineObject = _hit.collider.gameObject.transform.parent.transform.parent.gameObject;

                ObjectOutline _objectOutline = _outlineObject.GetComponent<ObjectOutline>();
                _objectOutline.enabled = true;
                _objectOutline.outlineColor = _outlineData[v_detailIdx].Item1;
                _objectOutline.outlineWidth = _outlineData[v_detailIdx].Item2;
            }
            else 
            {
                _outlineObject = _hit.collider.gameObject.transform.parent.transform.parent.gameObject;
            }

            // 1. repair tool �϶��� ui �ѱ� 
            if(v_detailIdx == 0)
                BuildMaster.Instance.housingUiManager.F_OnOffRepairText(_outlineObject.GetComponent<MyBuildingBlock>(), true);
        }

        // 0. ��Ŭ�� ���� ��
        if (Input.GetMouseButtonDown(0))
        {
            if (_outlineObject == null)
                return;

            // 1. myBlock �������� ( �浹�� buildFinished ������Ʈ�� �θ���, mybuildingBlock ��ũ��Ʈ )
            MyBuildingBlock my = _outlineObject.GetComponent<MyBuildingBlock>();

            // 2. repair ����
            if (v_detailIdx == 0)
                F_RepairTool(my);
            
            // 3. destroy ����
            else
                F_DestroyTool(my);
            
        }

    }

    // Destory Tool 
    private void F_DestroyTool(MyBuildingBlock v_mb)
    {
        // 1. Ŀ���� ������Ʈ ( ���� )
        F_DestroyConnetor(v_mb, v_mb.gameObject.transform);

        // 2. ������Ʈ �ı� ���� ���
        SoundManager.Instance.F_PlaySFX(SFXClip.DESTORY);
    }

    // Repair Tool  
    private void F_RepairTool(MyBuildingBlock v_mb)
    {
        // 1. ��ᰡ ����ϸ�?
        if (BuildMaster.Instance.mybuildCheck.F_WholeSourseIsEnough())
        {
            // 1. max ���� ������ , 1 ����
            if (v_mb.MyBlockMaxHp > v_mb.MyBlockHp)
            {
                // 1-1. �κ��丮 ������Ʈ (��� �Ҹ�)
                BuildMaster.Instance.mybuildCheck.F_UpdateInvenToBuilding();

                // 1-2. ������� , ui ������Ʈ 
                BuildMaster.Instance.mybuildCheck.F_BuildingStart();

                // 1-2. 1����
                v_mb.MyBlockHp += 1;

                // 2. ������Ʈ ���� ���� ��� ( �ӽ÷� �ı� ����� ���� )
                SoundManager.Instance.F_PlaySFX(SFXClip.DESTORY);
            }
            else
                return;
        }
        else
            return;
    }


    // Ŀ���� ���� 
    private void F_InstaceConnector(Vector3 v_genePosi, ConnectorType v_type)
    {
        // wall �ε�, ���Ͽ� (y<0) ��ü�Ǹ�?
        if ((v_type == ConnectorType.RotatedWallConnector || v_type == ConnectorType.BasicWallConnector)
            && v_genePosi.y <= 0)
            return;

        GameObject _connectorInstance = Instantiate(_connectorObject, v_genePosi, Quaternion.identity);
        _connectorInstance.transform.parent = BuildMaster.Instance._parentTransform;     // �θ��� 

        // 2. dir�� ���� ȸ���� ����, ����
        _connectorInstance.transform.rotation = F_SettingTypeToRatation(v_type);

        // 3. myBuildingBlock �߰�
        _connectorInstance.GetComponent<MyBuildingBlock>().F_SetBlockFeild(((int)v_type + 1) * -1, -1, -100, -100);      // floor : -1 , celling : -2 , basic wall : -3 , rotated wall : -4 

        // 4. ���̾� , Ÿ���� celling�� ��, celling�� y�� 0 �̸�? -> floor���̾����
        if (v_type == ConnectorType.CellingConnector && v_genePosi.y == 0)
            _connectorInstance.layer = BuildMaster.Instance._connectorLayer[(int)ConnectorType.FloorConnector].Item2;
        else
            _connectorInstance.layer = BuildMaster.Instance._connectorLayer[(int)v_type].Item2;

    }

    public void F_CreateConnector(Transform v_standardTrs) // ������ �Ǵ� ���� trs 
    {
        // 0. BuildMaster�� _currBlockData�� ConnectorGroup�� ���� �޶��� 
        ConnectorGroupType _currConGroup = BuildMaster.Instance.currBlockData.blockConnectorGroup;
        _currConnector = _connectorContainer[(int)_currConGroup];

        // 0-1 .wallConnectorGroup�ε� ȸ���� 0�� �ƴϸ� -> rotatorGroup
        if (_currConGroup == ConnectorGroupType.BasicWallConnectorGroup && v_standardTrs.rotation.y != 0)
            _currConnector = _connectorContainer[ (int)ConnectorGroupType.RotatedWallConnnectorGroup];
        
        // 0-2. blockConnectorGroup�� none �̸� return
        else if (_currConGroup == ConnectorGroupType.None)
            return;

        // 1. ���� connector���� List �� ���� 
        for (int i = 0; i < _currConnector.connectorList.Count; i++)
        {
            ConnectorType _conType = _currConnector.connectorList[i].Item1;
            Vector3 _posi = _currConnector.connectorList[i].Item2 + v_standardTrs.position;

            // Layermask + buildFinished LayerMask 
            Collider[] coll = F_DetectOBject(_posi, F_returnLayerType(_conType, _posi) | BuildMaster.Instance._buildFinishedLayer);

            // 2. �˻��ؼ� ������? ��ġ x
            if (coll.Length > 0)
                continue;

            // 3. �˻��ؼ� �� ������? -> ��ġ0
            else
                F_InstaceConnector(_posi, _conType);

        }
    }

    public void F_DestroyConnetor(MyBuildingBlock v_mybuilding, Transform v_stanardTrs)
    {
        // 0. �˻��ؾ��� Ŀ���� 
        List<Tuple<ConnectorType , Vector3 >> _connectorList = new List<Tuple<ConnectorType, Vector3>>();
        List<GameObject> _connectorObject = new List<GameObject>();

        // 1. �� �����̳�
        Vector3 _standartPosi = v_stanardTrs.position;
        Quaternion _standartRota = v_stanardTrs.rotation;

        // 2. �� ���� �� ����  
        Destroy(v_stanardTrs.gameObject);
        // 2. ���ڸ� Ŀ���� Ÿ�� ���� 
        ConnectorType _standartConnectorType = F_SettingConnectorType( (SelectedBuildType)v_mybuilding.MyBlockTypeIdx , _standartRota);
        
        // 0. ���� �� �������� Ŀ���� �˻� 
        // 1. Ŀ���� Ÿ�� ���� ���� ��
        // 2. 1�� ��ġ���� Ŀ���� �˻�
        // 2-1. buildFinished�� ������? -> Ŀ���ʹ� �����־����
        // 2-2. `` ������ -> Ŀ���� �����ؾ��� 

        // 0. Ŀ���� �����ϱ� : typeidx�� detail idx�� housinblock ���� -> ����ü�� ConnecorGroup �������� 
        HousingBlock _myhousingblock 
            = BuildMaster.Instance.housingDataManager.blockDataList[v_mybuilding.MyBlockTypeIdx][v_mybuilding.MyBlockDetailIdx];

        // ���� �� , ȸ���� ���� connector �޶���
        if( v_mybuilding.MyBlockTypeIdx == (int)SelectedBuildType.Wall && _standartRota.y != 0 )        // ȸ�� 0 
            _currConnector = _connectorContainer[ _connectorContainer.Length - 1 ];                     // roatated wall
        else
            _currConnector = _connectorContainer[(int)_myhousingblock.blockConnectorGroup];             // connector Group ���� 

        // 1. default �������̸� -> pass 
        if (_currConnector.name == string.Empty)
        {
            F_InstaceConnector( _standartPosi , _standartConnectorType );
            return;
        }

        for (int i = 0; i < _currConnector.connectorList.Count; i++)
        {
            ConnectorType _type = _currConnector.connectorList[i].Item1;
            Vector3 _position = _currConnector.connectorList[i].Item2 + _standartPosi;

            Collider[] coll = F_DetectOBject(_position, BuildMaster.Instance._connectorLayer[(int)_type].Item1);     // ������ġ����, ��üĿ���ͷ��̾�

            // Connector�� �������Ǹ�
            if (coll.Length > 0)
            {
                _connectorList.Add( new Tuple<ConnectorType,Vector3>(_type , _position ));
                _connectorObject.Add(coll[0].gameObject);
            }
            
        }

        StartCoroutine(F_IdentifyConnector(_connectorList, _connectorObject ,_standartPosi, _standartConnectorType, _currConnector));
    }

    IEnumerator F_IdentifyConnector(List<Tuple<ConnectorType, Vector3>> v_connectorList, List<GameObject> v_connObject ,Vector3 v_stanPosi, ConnectorType v_stanConType, Connector v_myConn)
    {
        yield return new WaitForSeconds(0.02f);

        int idx = 0;
        // 1. hashSet�� ��� ��ġ���� �˻�
        foreach (var _temp in v_connectorList)
        {
            // 1. Ŀ���� Ÿ�� ���� ���� 
            ConnectorType _stnadType = _temp.Item1;
            Vector3 _standPosi = _temp.Item2;

            Connector _myConnector = _connectorContainer[(int)_stnadType];

            bool _isDetected = false;       // buildFinished�� ���� �Ǿ�����? 

            // 2. Ŀ���� �˻�
            for (int i = 0; i < _myConnector.connectorList.Count; i++)
            {
                ConnectorType _typetype = _myConnector.connectorList[i].Item1;
                Vector3 _posiposi = _myConnector.connectorList[i].Item2 + _standPosi;

                Collider[] _coll = F_DetectOBject(_posiposi, BuildMaster.Instance._buildFinishedLayer);    // ������ġ����, buildFInisehd ���̾� �˻�

                // 2-1. ������ �Ǹ�? -> break�� for�� Ż��
                if (_coll.Length > 0)
                {
                    _isDetected = true; //�����  
                    break;
                }
            }

            // 2-1. ����Ǹ�? -> �н�
            // 2-2. ���� �ȵǸ�? -> Ŀ���� ����� 
            if (!_isDetected)
            {
                Destroy(v_connObject[idx]);
            }
            idx++;
        }

        yield return new WaitForSeconds(0.02f);

        bool _isMyConnectorUsed = false;
        // ���� �� ���� �� ������ �� ��ġ�� Ŀ���� ��ġ => Ŀ���Ͱ� �ΰ� ��ġ�ɼ��� ?
        for (int i = 0; i < v_myConn.connectorList.Count; i++)
        {
            ConnectorType _type = v_myConn.connectorList[i].Item1;
            Vector3 _posi = v_myConn.connectorList[i].Item2 + v_stanPosi;

            Collider[] _coll = F_DetectOBject(_posi, BuildMaster.Instance._buildFinishedLayer);

            // 1. �����Ǹ� -> ��ġ
            if (_coll.Length > 0)
            {
                _isMyConnectorUsed = true;
                break;
            }
        }

        // �����Ǹ�? -> ��ġ 
        if (_isMyConnectorUsed)
            F_InstaceConnector(v_stanPosi, v_stanConType);
    }

    // Type�� ���� ���̾� ���� 
    private LayerMask F_returnLayerType(ConnectorType v_type, Vector3 v_genePosi)
    {
        if (v_type == ConnectorType.CellingConnector && v_genePosi.y == 0)
            return BuildMaster.Instance._connectorLayer[(int)ConnectorType.FloorConnector].Item1;
        else
            return BuildMaster.Instance._connectorLayer[(int)v_type].Item1;
    }

    // �ش� ��ġ���� Lay
    private Collider[] F_DetectOBject(Vector3 v_posi, LayerMask v_layer)
    {
        // �ش� ��ġ���� layermask �� �˻��ؼ� return
        Collider[] _coll = Physics.OverlapSphere(v_posi, 1f, v_layer);

        return _coll;
    }

    // Select type�� ���� 'Ŀ����' ���� 
    public Connector F_SettingConnector(SelectedBuildType v_type, Quaternion v_quteRotation)
    {
        switch (v_type)
        {
            case SelectedBuildType.Floor:
                return _connectorContainer[0];                                    // floor Ŀ����  
            case SelectedBuildType.Celling:
                return _connectorContainer[1];                                    // celling Ŀ���� 
            case SelectedBuildType.Wall:                                    // window, door, window�� ���� wall ���̾� ��� 
            case SelectedBuildType.Door:
            case SelectedBuildType.Window:
                if (v_quteRotation.eulerAngles.y != 0)                      // ȸ������ ������?
                    return _connectorContainer[3];                                // ȸ�� 0 wall Ŀ����
                else
                    return _connectorContainer[2];                                // ȸ�� x wall Ŀ����

            default:
                return default;
        }
    }

    // 'Ŀ����Ÿ��'�� ���� ȸ�� ���� 
    public Quaternion F_SettingTypeToRatation(ConnectorType v_type)
    {
        Quaternion _rot = Quaternion.identity;
        switch (v_type)
        {
            case ConnectorType.FloorConnector:
                _rot.eulerAngles = new Vector3(90f, 0, 0);
                return _rot;
            case ConnectorType.CellingConnector:
                _rot.eulerAngles = new Vector3(90f, 0, 0);
                return _rot;
            case ConnectorType.BasicWallConnector:
                _rot.eulerAngles = new Vector3(0, 0, 0);
                return _rot;
            case ConnectorType.RotatedWallConnector:
                _rot.eulerAngles = new Vector3(0, 90f, 0);
                return _rot;
            default:
                return _rot;
        }

    }

    // selectType�� ȸ���� ���� 'Ŀ����Ÿ��' ��ȯ 
    private ConnectorType F_SettingConnectorType(SelectedBuildType v_buildType, Quaternion v_rotation)
    {
        switch (v_buildType)
        {
            case SelectedBuildType.Floor:
                return ConnectorType.FloorConnector;
            case SelectedBuildType.Celling:
                return ConnectorType.CellingConnector;
            case SelectedBuildType.Wall:
            case SelectedBuildType.Window:
            case SelectedBuildType.Door:
                if (v_rotation.eulerAngles.y == 0)
                    return ConnectorType.BasicWallConnector;
                else
                    return ConnectorType.RotatedWallConnector;
            default:
                return default;

        }
    }
}
