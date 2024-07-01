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

    // Connector 세팅 , HousingDataManager에서 사용중 
    public void F_SetConnArr(Connector con1, Connector con2, Connector con3, Connector con4)
    {
        _connectorContainer = new Connector[System.Enum.GetValues(typeof(ConnectorGroupType)).Length];       // 커넥터 타입만큼 배열 생성

        _connectorContainer[ (int)ConnectorGroupType.FloorConnectorGroup ]        = con1;
        _connectorContainer[ (int)ConnectorGroupType.CellingConnectorGroup ]      = con2;
        _connectorContainer[ (int)ConnectorGroupType.BasicWallConnectorGroup ]    = con3;
        _connectorContainer[ (int)ConnectorGroupType.RotatedWallConnnectorGroup ] = con4;
        _connectorContainer[ (int)ConnectorGroupType.None ]                       = Connector.Defalt;
    }

    // housing 도구 내려놓을 떄, outlind object 초기화 
    public void F_InitOutlineObject() 
    {
        // 0. 오브젝트 아웃라인 끄기
        if (_outlineObject != null)
            _outlineObject.GetComponent<ObjectOutline>().enabled = false;
        _outlineObject = null;

        // 1. repair text 끄기
        BuildMaster.Instance.housingUiManager.F_OnOffRepairText(null, false);
    }

    // 수리 & 파괴도구 동작 
    public void F_RepairAndDestroyTool( int v_detailIdx ,LayerMask v_currLayer )
    {
        RaycastHit _hit;

        // 0. outline 효과 
        if (Physics.Raycast(BuildMaster.Instance._playerCamera.transform.position,
                BuildMaster.Instance._playerCamera.transform.forward * 10, out _hit, 5f, v_currLayer))   // 타입 : LayerMask
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

            // 1. repair tool 일때만 ui 켜기 
            if(v_detailIdx == 0)
                BuildMaster.Instance.housingUiManager.F_OnOffRepairText(_outlineObject.GetComponent<MyBuildingBlock>(), true);
        }

        // 0. 우클릭 했을 때
        if (Input.GetMouseButtonDown(0))
        {
            if (_outlineObject == null)
                return;

            // 1. myBlock 가져오기 ( 충돌한 buildFinished 오브젝트의 부모의, mybuildingBlock 스크립트 )
            MyBuildingBlock my = _outlineObject.GetComponent<MyBuildingBlock>();

            // 2. repair 도구
            if (v_detailIdx == 0)
                F_RepairTool(my);
            
            // 3. destroy 도구
            else
                F_DestroyTool(my);
            
        }

    }

    // Destory Tool 
    private void F_DestroyTool(MyBuildingBlock v_mb)
    {
        // 1. 커넥터 업데이트 ( 삭제 )
        F_DestroyConnetor(v_mb, v_mb.gameObject.transform);

        // 2. 오브젝트 파괴 사운드 재생
        SoundManager.Instance.F_PlaySFX(SFXClip.DESTORY);
    }

    // Repair Tool  
    private void F_RepairTool(MyBuildingBlock v_mb)
    {
        // 1. 재료가 충분하면?
        if (BuildMaster.Instance.mybuildCheck.F_WholeSourseIsEnough())
        {
            // 1. max 보다 작으면 , 1 증가
            if (v_mb.MyBlockMaxHp > v_mb.MyBlockHp)
            {
                // 1-1. 인벤토리 업데이트 (재료 소모)
                BuildMaster.Instance.mybuildCheck.F_UpdateInvenToBuilding();

                // 1-2. 정보담기 , ui 업데이트 
                BuildMaster.Instance.mybuildCheck.F_BuildingStart();

                // 1-2. 1증가
                v_mb.MyBlockHp += 1;

                // 2. 오브젝트 수리 사운드 재생 ( 임시로 파괴 사운드와 동일 )
                SoundManager.Instance.F_PlaySFX(SFXClip.DESTORY);
            }
            else
                return;
        }
        else
            return;
    }


    // 커넥터 생성 
    private void F_InstaceConnector(Vector3 v_genePosi, ConnectorType v_type)
    {
        // wall 인데, 지하에 (y<0) 설체되면?
        if ((v_type == ConnectorType.RotatedWallConnector || v_type == ConnectorType.BasicWallConnector)
            && v_genePosi.y <= 0)
            return;

        GameObject _connectorInstance = Instantiate(_connectorObject, v_genePosi, Quaternion.identity);
        _connectorInstance.transform.parent = BuildMaster.Instance._parentTransform;     // 부모설정 

        // 2. dir에 따라 회전값 조정, 적용
        _connectorInstance.transform.rotation = F_SettingTypeToRatation(v_type);

        // 3. myBuildingBlock 추가
        _connectorInstance.GetComponent<MyBuildingBlock>().F_SetBlockFeild(((int)v_type + 1) * -1, -1, -100, -100);      // floor : -1 , celling : -2 , basic wall : -3 , rotated wall : -4 

        // 4. 레이어 , 타입이 celling일 때, celling의 y가 0 이면? -> floor레이어야함
        if (v_type == ConnectorType.CellingConnector && v_genePosi.y == 0)
            _connectorInstance.layer = BuildMaster.Instance._connectorLayer[(int)ConnectorType.FloorConnector].Item2;
        else
            _connectorInstance.layer = BuildMaster.Instance._connectorLayer[(int)v_type].Item2;

    }

    public void F_CreateConnector(Transform v_standardTrs) // 기준이 되는 블럭의 trs 
    {
        // 0. BuildMaster의 _currBlockData의 ConnectorGroup에 따라 달라짐 
        ConnectorGroupType _currConGroup = BuildMaster.Instance.currBlockData.blockConnectorGroup;
        _currConnector = _connectorContainer[(int)_currConGroup];

        // 0-1 .wallConnectorGroup인데 회전이 0이 아니면 -> rotatorGroup
        if (_currConGroup == ConnectorGroupType.BasicWallConnectorGroup && v_standardTrs.rotation.y != 0)
            _currConnector = _connectorContainer[ (int)ConnectorGroupType.RotatedWallConnnectorGroup];
        
        // 0-2. blockConnectorGroup이 none 이면 return
        else if (_currConGroup == ConnectorGroupType.None)
            return;

        // 1. 현재 connector안의 List 에 접근 
        for (int i = 0; i < _currConnector.connectorList.Count; i++)
        {
            ConnectorType _conType = _currConnector.connectorList[i].Item1;
            Vector3 _posi = _currConnector.connectorList[i].Item2 + v_standardTrs.position;

            // Layermask + buildFinished LayerMask 
            Collider[] coll = F_DetectOBject(_posi, F_returnLayerType(_conType, _posi) | BuildMaster.Instance._buildFinishedLayer);

            // 2. 검사해서 잡히면? 설치 x
            if (coll.Length > 0)
                continue;

            // 3. 검사해서 안 잡히면? -> 설치0
            else
                F_InstaceConnector(_posi, _conType);

        }
    }

    public void F_DestroyConnetor(MyBuildingBlock v_mybuilding, Transform v_stanardTrs)
    {
        // 0. 검사해야할 커넥터 
        List<Tuple<ConnectorType , Vector3 >> _connectorList = new List<Tuple<ConnectorType, Vector3>>();
        List<GameObject> _connectorObject = new List<GameObject>();

        // 1. 값 컨테이너
        Vector3 _standartPosi = v_stanardTrs.position;
        Quaternion _standartRota = v_stanardTrs.rotation;

        // 2. 값 담은 뒤 삭제  
        Destroy(v_stanardTrs.gameObject);
        // 2. 그자리 커넥터 타입 지정 
        ConnectorType _standartConnectorType = F_SettingConnectorType( (SelectedBuildType)v_mybuilding.MyBlockTypeIdx , _standartRota);
        
        // 0. 삭제 블럭 기준으로 커넥터 검사 
        // 1. 커넥터 타입 새로 지정 후
        // 2. 1번 위치에서 커넥터 검사
        // 2-1. buildFinished가 있으면? -> 커넥터는 남아있어야함
        // 2-2. `` 없으면 -> 커넥터 삭제해야함 

        // 0. 커넥터 지정하기 : typeidx와 detail idx로 housinblock 지정 -> 구조체의 ConnecorGroup 가져오기 
        HousingBlock _myhousingblock 
            = BuildMaster.Instance.housingDataManager.blockDataList[v_mybuilding.MyBlockTypeIdx][v_mybuilding.MyBlockDetailIdx];

        // 벽일 때 , 회전에 따라 connector 달라짐
        if( v_mybuilding.MyBlockTypeIdx == (int)SelectedBuildType.Wall && _standartRota.y != 0 )        // 회전 0 
            _currConnector = _connectorContainer[ _connectorContainer.Length - 1 ];                     // roatated wall
        else
            _currConnector = _connectorContainer[(int)_myhousingblock.blockConnectorGroup];             // connector Group 따라감 

        // 1. default 구초제이면 -> pass 
        if (_currConnector.name == string.Empty)
        {
            F_InstaceConnector( _standartPosi , _standartConnectorType );
            return;
        }

        for (int i = 0; i < _currConnector.connectorList.Count; i++)
        {
            ConnectorType _type = _currConnector.connectorList[i].Item1;
            Vector3 _position = _currConnector.connectorList[i].Item2 + _standartPosi;

            Collider[] coll = F_DetectOBject(_position, BuildMaster.Instance._connectorLayer[(int)_type].Item1);     // 기준위치에서, 전체커넥터레이어

            // Connector가 감지가되면
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
        // 1. hashSet에 담긴 위치에서 검사
        foreach (var _temp in v_connectorList)
        {
            // 1. 커넥터 타입 새로 지정 
            ConnectorType _stnadType = _temp.Item1;
            Vector3 _standPosi = _temp.Item2;

            Connector _myConnector = _connectorContainer[(int)_stnadType];

            bool _isDetected = false;       // buildFinished가 검출 되었는지? 

            // 2. 커넥터 검사
            for (int i = 0; i < _myConnector.connectorList.Count; i++)
            {
                ConnectorType _typetype = _myConnector.connectorList[i].Item1;
                Vector3 _posiposi = _myConnector.connectorList[i].Item2 + _standPosi;

                Collider[] _coll = F_DetectOBject(_posiposi, BuildMaster.Instance._buildFinishedLayer);    // 기준위치에서, buildFInisehd 레이어 검사

                // 2-1. 검출이 되면? -> break로 for문 탈출
                if (_coll.Length > 0)
                {
                    _isDetected = true; //검출됨  
                    break;
                }
            }

            // 2-1. 검출되면? -> 패스
            // 2-2. 검출 안되면? -> 커넥터 지우기 
            if (!_isDetected)
            {
                Destroy(v_connObject[idx]);
            }
            idx++;
        }

        yield return new WaitForSeconds(0.02f);

        bool _isMyConnectorUsed = false;
        // 동작 다 끝난 후 삭제된 블럭 위치에 커넥터 설치 => 커넥터가 두개 설치될수도 ?
        for (int i = 0; i < v_myConn.connectorList.Count; i++)
        {
            ConnectorType _type = v_myConn.connectorList[i].Item1;
            Vector3 _posi = v_myConn.connectorList[i].Item2 + v_stanPosi;

            Collider[] _coll = F_DetectOBject(_posi, BuildMaster.Instance._buildFinishedLayer);

            // 1. 감지되면 -> 설치
            if (_coll.Length > 0)
            {
                _isMyConnectorUsed = true;
                break;
            }
        }

        // 감지되면? -> 설치 
        if (_isMyConnectorUsed)
            F_InstaceConnector(v_stanPosi, v_stanConType);
    }

    // Type에 따라 레이어 설정 
    private LayerMask F_returnLayerType(ConnectorType v_type, Vector3 v_genePosi)
    {
        if (v_type == ConnectorType.CellingConnector && v_genePosi.y == 0)
            return BuildMaster.Instance._connectorLayer[(int)ConnectorType.FloorConnector].Item1;
        else
            return BuildMaster.Instance._connectorLayer[(int)v_type].Item1;
    }

    // 해당 위치에서 Lay
    private Collider[] F_DetectOBject(Vector3 v_posi, LayerMask v_layer)
    {
        // 해당 위치에서 layermask 을 검사해서 return
        Collider[] _coll = Physics.OverlapSphere(v_posi, 1f, v_layer);

        return _coll;
    }

    // Select type에 따른 '커넥터' 설정 
    public Connector F_SettingConnector(SelectedBuildType v_type, Quaternion v_quteRotation)
    {
        switch (v_type)
        {
            case SelectedBuildType.Floor:
                return _connectorContainer[0];                                    // floor 커넥터  
            case SelectedBuildType.Celling:
                return _connectorContainer[1];                                    // celling 커넥터 
            case SelectedBuildType.Wall:                                    // window, door, window는 같은 wall 레이어 사용 
            case SelectedBuildType.Door:
            case SelectedBuildType.Window:
                if (v_quteRotation.eulerAngles.y != 0)                      // 회전값이 있으면?
                    return _connectorContainer[3];                                // 회전 0 wall 커넥터
                else
                    return _connectorContainer[2];                                // 회전 x wall 커넥터

            default:
                return default;
        }
    }

    // '커넥터타입'에 따라 회전 설정 
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

    // selectType과 회전에 따른 '커넥터타입' 반환 
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
