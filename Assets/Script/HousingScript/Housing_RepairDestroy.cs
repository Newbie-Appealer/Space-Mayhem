using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Housing_RepairDestroy : MonoBehaviour
{
    [SerializeField] private Connector[] _connectorArr;
    [SerializeField] private Connector _currConnector;
    public GameObject _connectorObject;

    private HashSet<Tuple<ConnectorType, Vector3>> _detectConnectorOnDestroyBlock;            // destory�� �� ��ġ���� ������ Connector

    // Connector ���� , HousingDataManager���� ����� 
    public void F_SetConnArr(Connector con1, Connector con2, Connector con3, Connector con4)
    {
        // ##TODO : Ŀ���ͱ׷� enum �����ؼ� �ٲ���� 
        _connectorArr = new Connector[System.Enum.GetValues(typeof(ConnectorType)).Length];       // Ŀ���� Ÿ�Ը�ŭ �迭 ����

        _connectorArr[0] = con1;
        _connectorArr[1] = con2;
        _connectorArr[2] = con3;
        _connectorArr[3] = con4;
    }

    private void Awake()
    {
        // 1. �ʱ�ȭ
        _detectConnectorOnDestroyBlock = new HashSet<Tuple<ConnectorType, Vector3>>();
    }

    // ���� & �ı����� ���� 
    public void F_RepairAndDestroyTool( LayerMask v_currLayer )
    {
        // 0. ��Ŭ�� ���� ��
        if (Input.GetMouseButtonDown(0))
        {
            // 1. ray ���� finished ���� ������
            RaycastHit _hit;
            if (Physics.Raycast(BuildMaster.Instance._playerCamera.transform.position,
                BuildMaster.Instance._playerCamera.transform.forward * 10, out _hit, 5f, v_currLayer))   // Ÿ�� : LayerMask
            {
                // 1. myBlock �������� ( �浹�� buildFinished ������Ʈ�� �θ���, mybuildingBlock ��ũ��Ʈ )
                MyBuildingBlock my = _hit.collider.gameObject.transform.parent.GetComponent<MyBuildingBlock>();

                // 2. repair ����
                if (BuildMaster.Instance._buildDetailIdx == 0)
                    F_RepairTool(my);
                // 3. destroy ����
                else
                    F_DestroyTool(my);
            }
        }

    }


    // Destory Tool 
    private void F_DestroyTool(MyBuildingBlock v_mb)
    {
        // 1. Ŀ���� ������Ʈ ( ���� )
        F_DestroyConnetor((SelectedBuildType)v_mb.MyBlockTypeIdx, v_mb.gameObject.transform);

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

    public void F_CreateConnector(SelectedBuildType v_type, Transform v_stanardTrs) // ������ �Ǵ� ���� trs 
    {
        // 0. _currConnector Setting 
        _currConnector = F_SettingConnector(v_type, v_stanardTrs.rotation);

        // ���� connector���� List �� ���� 
        for (int i = 0; i < _currConnector.connectorList.Count; i++)
        {
            ConnectorType _conType = _currConnector.connectorList[i].Item1;
            Vector3 _posi = _currConnector.connectorList[i].Item2 + v_stanardTrs.position;

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

    public void F_DestroyConnetor(SelectedBuildType v_buildType, Transform v_stanardTrs)
    {
        // 0. �ʱ�ȭ
        _detectConnectorOnDestroyBlock.Clear();
        List<GameObject> _connectorList = new List<GameObject>();       // Ŀ���� ��Ƴ��� -> idx�� �����ؼ� destory �ؾ��� 

        // 1. �� �����̳�
        Vector3 _standartPosi = v_stanardTrs.position;
        Quaternion _standartRota = v_stanardTrs.rotation;

        // 2. �� ���� �� ����  
        Destroy(v_stanardTrs.gameObject);
        // 2. ���ڸ� Ŀ���� Ÿ�� ���� 
        ConnectorType _standartConnectorType = F_SettingConnectorType(v_buildType, _standartRota);

        // 0. ���� �� �������� Ŀ���� �˻� , wholeLayer �˻� -> hashSet�� ��Ƶα� (�ߺ�x) 
        // 1. Ŀ���� Ÿ�� ���� ���� ��
        // 2. 1�� ��ġ���� Ŀ���� �˻�
        // 2-1. buildFinished�� ������? -> Ŀ���ʹ� �����־����
        // 2-2. `` ������ -> Ŀ���� �����ؾ��� 

        // 0. Ŀ���� �����ϱ� 
        _currConnector = F_SettingConnector(v_buildType, _standartRota);
        Connector _myConnector = F_SettingConnector(v_buildType, _standartRota);

        for (int i = 0; i < _currConnector.connectorList.Count; i++)
        {
            ConnectorType _type = _currConnector.connectorList[i].Item1;
            Vector3 _position = _currConnector.connectorList[i].Item2 + _standartPosi;

            Collider[] coll = F_DetectOBject(_position, BuildMaster.Instance._ConnectorWholelayer);     // ������ġ����, ��üĿ���ͷ��̾�

            // buildConnector�� �����ϰ�, Connector�� �������Ǹ�
            if (coll.Length > 0)
            {
                // 0. �ߺ�����, hashSet�� ��� 
                if (_detectConnectorOnDestroyBlock.Add(new Tuple<ConnectorType, Vector3>(_type, _position)))
                {
                    // haset�� �ߺ��Ȱ� ��� �������?
                    _connectorList.Add(coll[0].gameObject);
                }
            }
        }

        StartCoroutine(F_Test(_connectorList, _standartPosi, _standartConnectorType, _myConnector));
    }

    IEnumerator F_Test(List<GameObject> v_connectorList, Vector3 v_stanPosi, ConnectorType v_stanConType, Connector v_myConn)
    {
        yield return new WaitForSeconds(0.02f);

        int idx = 0;
        // 1. hashSet�� ��� ��ġ���� �˻�
        foreach (var _hash in _detectConnectorOnDestroyBlock)
        {
            // 1. Ŀ���� Ÿ�� ���� ���� 
            ConnectorType _stnadType = _hash.Item1;
            Vector3 _standPosi = _hash.Item2;

            Connector _myConnector = _connectorArr[(int)_stnadType];

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
                Destroy(v_connectorList[idx].gameObject);
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
                return _connectorArr[0];                                    // floor Ŀ����  
            case SelectedBuildType.Celling:
                return _connectorArr[1];                                    // celling Ŀ���� 
            case SelectedBuildType.Wall:                                    // window, door, window�� ���� wall ���̾� ��� 
            case SelectedBuildType.Door:
            case SelectedBuildType.Window:
                if (v_quteRotation.eulerAngles.y != 0)                      // ȸ������ ������?
                    return _connectorArr[3];                                // ȸ�� 0 wall Ŀ����
                else
                    return _connectorArr[2];                                // ȸ�� x wall Ŀ����

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
