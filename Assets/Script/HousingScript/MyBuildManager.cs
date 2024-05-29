using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public enum SelectedBuildType
{
    Floor,
    Celling,
    Wall,
    Door,
    Window,
    RepairTools
}

[System.Serializable]
public enum ConnectorType
{
    FloorConnector,
    CellingConnector,
    BasicWallConnector,
    RotatedWallConnector
}

public class MyBuildManager : MonoBehaviour
{
    public GameObject _testobject;
    
    [Header("===Player===")]
    public GameObject _player;

    [Header("===Block Parent===")]
    [SerializeField] Transform _parentTransform;

    [Header("===Type===")]
    [HideInInspector] private SelectedBuildType _SelectBuildType;
    [HideInInspector] private Connector[] _connectorArr;
    [SerializeField] private Connector _currConnector;
    public GameObject _connectorObject;

    #region prefab
    [SerializeField] public List<List<GameObject>> _bundleBulingPrefab;

    [Header("===Build Object===")]
    [SerializeField] private List<GameObject> _floorList;
    [SerializeField] private List<GameObject> _cellingList;
    [SerializeField] private List<GameObject> _wallList;
    [SerializeField] private List<GameObject> _doorList;
    [SerializeField] private List<GameObject> _windowList;
    #endregion

    #region LayerMask
    [Header("===LayerMask===")]
    [SerializeField] LayerMask _currTempLayer;              // ���� (temp ���� ������) ���̾� 
    private LayerMask _buildFinishedLayer;                  // �� ���� ���� layermask   
    private int _buildFinishedint;                          // �� ���� ���� layer int
    private List<Tuple<LayerMask, int>> _connectorLayer;    // Ŀ���� ���̾� 
    [HideInInspector] public LayerMask _ConnectorWholelayer;     // ��� Ŀ���� ���̾� �� ��ģ
    #endregion

    #region condition
    private int _buildTypeIdx;                              // ���� Ÿ������
    private int _buildDetailIdx;                            // �� Ÿ�Ծ� ���° ������Ʈ ����

    [Header("===Temp Object Setting===")]
    [SerializeField] bool _isTempValidPosition;             // �ӽ� ������Ʈ�� ������ �� �ִ���
    [SerializeField] bool _isEnoughResource;                // ��ġ�ϱ⿡ ��ᰡ �������?
    [SerializeField] bool _isntColliderOther;               // temp wall �� �ٸ� ������Ʈ�� �浹�� ��������? ( false�� �浹 )
    #endregion

    #region temp object Trs
    [HideInInspector] GameObject _tempObjectBuilding;        // �ӽ� ������Ʈ
    [HideInInspector] Transform _modelTransform;             // �� ������Ʈ 
    [HideInInspector] Transform _otherConnectorTr;           // �浹�� �ٸ� ������Ʈ 
    #endregion

    #region Material
    [Header("===Material===")]
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;
    [HideInInspector] Material _oriMaterial;
    [HideInInspector] Material _nowBuildMaterial;
    #endregion

    #region �� destroy
    private HashSet<Tuple<ConnectorType, Vector3>> _detectConnectorOnDestroyBlock;            // destory�� �� ��ġ���� ������ Connector

    #endregion

    // ������Ƽ
    public int BuildFinishedLayer { get => _buildFinishedint; }
    public bool IsntColliderOther { get => _isntColliderOther; set { _isntColliderOther = value; } }

    // =============================================
    private void Awake()
    {
        // 0. savemanager ��������Ʈ�� ���� 
        SaveManager.Instance.GameDataSave += () => SaveManager.Instance.F_SaveBuilding(_parentTransform.transform);

        // 1. �ʱ�ȭ
        _connectorArr = new Connector[System.Enum.GetValues(typeof(ConnectorType)).Length];       // Ŀ���� Ÿ�Ը�ŭ �迭 ����
        _detectConnectorOnDestroyBlock = new HashSet<Tuple<ConnectorType, Vector3>>();

        F_InitLayer();              // ���̾� �ʱ�ȭ                        
        _bundleBulingPrefab = new List<List<GameObject>> // �� block ������ List�� �ϳ��� List�� ����
        {
            _floorList,
            _cellingList,
            _wallList,
            _doorList,
            _windowList
        };

        // 2. ���� �ʱ�ȭ
        _isTempValidPosition = true;
        _isEnoughResource = false;
        _isntColliderOther = true;  // �ٸ� ������Ʈ�� �浹�Ǿ��ִ°�?

        // 
        BuildMaster.Instance.housingDataManager.F_InitConnectorInfo();

        // 3. SavaManager���� �ҷ����� 
        SaveManager.Instance.F_LoadBuilding(_parentTransform);
    }

    public void F_SetConnArr(Connector con1, Connector con2, Connector con3, Connector con4)
    {
        _connectorArr[0] = con1;
        _connectorArr[1] = con2;
        _connectorArr[2] = con3;
        _connectorArr[3] = con4;
    }

    private void F_InitLayer()
    {
        _buildFinishedint = LayerMask.NameToLayer("BuildFinishedBlock");
        _buildFinishedLayer = LayerMask.GetMask("BuildFinishedBlock");

        _connectorLayer = new List<Tuple<LayerMask, int>>
        {
            new Tuple <LayerMask , int>( LayerMask.GetMask("FloorConnectorLayer") , LayerMask.NameToLayer("FloorConnectorLayer") ),         // temp floor ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("CellingConnectorLayer") , LayerMask.NameToLayer("CellingConnectorLayer") ),     // temp celling ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("WallConnectorLayer") , LayerMask.NameToLayer("WallConnectorLayer") ),           // temp wall ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("WallConnectorLayer") , LayerMask.NameToLayer("WallConnectorLayer") ),           // temp wall ���̾� ( destory ���� ���ؼ� )
            
        };

        _ConnectorWholelayer = _connectorLayer[0].Item1 | _connectorLayer[1].Item1 | _connectorLayer[2].Item1;

    }


    public void F_GetbuildType(int v_type = 0, int v_detail = 1)
    {
        // 0. Ui�� �߸��� idx�� �Ѿ���� �� 
        if (v_type < 0 && v_detail < 0)
            return;

        // 1. index �ʱ�ȭ
        _buildTypeIdx = v_type;
        _buildDetailIdx = v_detail;

        // 2. �ӽ� ������Ʈ Ȯ��
        if (_tempObjectBuilding != null)
            Destroy(_tempObjectBuilding);
        _tempObjectBuilding = null;

        // 3. building check �ʱ�ȭ
        BuildMaster.Instance.mybuildCheck.F_BuildingStart();

        // 4. ���� ���� 
        StopAllCoroutines();
        StartCoroutine(F_TempBuild());
    }

    IEnumerator F_TempBuild()
    {
        // 0. index�� �ش��ϴ� ���� ������Ʈ return
        GameObject _currBuild = F_GetCurBuild(_buildTypeIdx, _buildDetailIdx);
        // 0.1. �� �� Ÿ�Կ� ���� �˻��� layer , Conneector ���ϱ� 
        F_SettingCurrLayer(_SelectBuildType);

        // 0.2. type�� ���� progress on off ui ����
        F_OnOffProgressUI();

        while (true)
        {
            // �������� type�� �ı����� �� ��
            if (_SelectBuildType == SelectedBuildType.RepairTools)
            {
                F_RepairAndDestroyTool();
            }
            // ��������, build type �� �� 
            else
            {
                F_OtherBuildBlockBuild(_currBuild);
            }

            // update ȿ�� 
            yield return null;
        }
    }


    #region ray , snap ����
    private void F_OtherBuildBlockBuild(GameObject v_build)
    {
        // 1. index�� �ش��ϴ� ���ӿ�����Ʈ ����
        F_CreateTempPrefab(v_build);

        // 2. �ش� ���̶� ���� Layer�� raycast
        F_Raycast(_currTempLayer);

        // 3. temp ������Ʈ�� �ݶ��̴� �˻�
        F_CheckCollision(_currTempLayer);

        // 4. ��ġ
        if (Input.GetMouseButtonDown(0))
            F_FinishBuild();
    }


    private void F_OnOffProgressUI()
    {
        // 0. repair type �� destroy���̸� progressUI ���� 
        if (_SelectBuildType == SelectedBuildType.RepairTools && _buildDetailIdx == 1)
            BuildMaster.Instance.housingUiManager.F_OnOFfBuildingProgressUi(false);
        // 1. �ƴϸ� �ѱ�
        else
            BuildMaster.Instance.housingUiManager.F_OnOFfBuildingProgressUi(true);

    }

    private void F_Raycast(LayerMask v_layer)
    {
        // 1. �Ѿ�� Layer'��' rayCast
        RaycastHit _hit;

        // ##TODO ���⼭ playermanager�� trasform�� �������°� ����? �ٵ� �׷��� �������� �����;��� 
        // 2. raycast �Ǹ� -> �ӽ� ������Ʈ�� �� ��ġ�� �ű�� 
        if (Physics.Raycast(_player.transform.position, _player.transform.forward * 10, out _hit, 5f, v_layer)) // Ÿ�� : LayerMask
        {
            _tempObjectBuilding.transform.position = _hit.point;
        }
    }

    private void F_CheckCollision(LayerMask v_layer)
    {
        // 1. �ݶ��̴� �˻� 
        Collider[] _coll = Physics.OverlapSphere(_tempObjectBuilding.transform.position, 1f, v_layer);    // Ÿ�� : LayerMask

        // 2. �˻�Ǹ� -> ������Ʈ Snap
        if (_coll.Length > 0)
            F_Snap(_coll);
        else
            _isTempValidPosition = false;

    }

    private void F_Snap(Collider[] v_coll)
    {
        // 0. �ٸ� Ŀ����? -> �迭�� ó������ ���� collider
        _otherConnectorTr = v_coll[0].transform;

        // 1. Ÿ���� wall �϶��� ȸ�� 
        if (_SelectBuildType == SelectedBuildType.Wall || _SelectBuildType == SelectedBuildType.Window
            || _SelectBuildType == SelectedBuildType.Door)
        {
            // �� temp �� ȸ�� += ������ Ŀ������ ȸ��
            Quaternion qu = _tempObjectBuilding.transform.rotation;
            qu.eulerAngles = new Vector3(qu.eulerAngles.x, _otherConnectorTr.eulerAngles.y, qu.eulerAngles.z);
            _tempObjectBuilding.transform.rotation = qu;
        }

        // 2. Snap!! 
        _tempObjectBuilding.transform.position
             = _otherConnectorTr.position;

        // 3. mesh �ѱ�
        F_MeshOnOff(_modelTransform, true);

        // 4. ��ġ���� 
        _isTempValidPosition = true;
    }

    public GameObject F_GetCurBuild(int v_type, int v_detail)
    {
        switch (v_type)
        {
            case 0:
                _SelectBuildType = SelectedBuildType.Floor;
                return _floorList[v_detail];
            case 1:
                _SelectBuildType = SelectedBuildType.Celling;
                return _cellingList[v_detail];
            case 2:
                _SelectBuildType = SelectedBuildType.Wall;
                return _wallList[v_detail];
            case 3:
                _SelectBuildType = SelectedBuildType.Door;
                return _doorList[v_detail];
            case 4:
                _SelectBuildType = SelectedBuildType.Window;
                return _windowList[v_detail];
            default:
                _SelectBuildType = SelectedBuildType.RepairTools;
                return null;
        }
    }

    private void F_SettingCurrLayer(SelectedBuildType v_type)
    {
        switch (v_type)
        {
            case SelectedBuildType.Floor:
                _currTempLayer = _connectorLayer[0].Item1;                          // floor ���̾�
                break;
            case SelectedBuildType.Celling:
                _currTempLayer = _connectorLayer[1].Item1;                          // celling ���̾�
                break;
            case SelectedBuildType.Wall:                                                 // window, door, window�� ���� wall ���̾� ��� 
            case SelectedBuildType.Door:
            case SelectedBuildType.Window:
                _currTempLayer = _connectorLayer[2].Item1;                          // wall ���̾�
                break;
            case SelectedBuildType.RepairTools:                                          // repair �� �� �� 
                _currTempLayer = _buildFinishedLayer;                                    // buildFinished
                break;
        }
    }

    private void F_CreateTempPrefab(GameObject v_temp)
    {
        // ��������
        // temp������Ʈ�� null�̵Ǹ� �ٷ� ������!
        if (_tempObjectBuilding == null)
        {
            // 1. ���� & 100f,100f,100f�� �ӽ���ġ 
            _tempObjectBuilding = Instantiate(v_temp, new Vector3(100f, 100f, 100f), Quaternion.identity);

            // 2. model Transform 
            _modelTransform = _tempObjectBuilding.transform.GetChild(0);

            // 2-1. moel�� is Trigger �ѱ� 
            F_ColliderTriggerOnOff(_modelTransform, true);

            // 3. ���� material ����
            _oriMaterial = _modelTransform.GetChild(0).GetComponent<MeshRenderer>().material;

            // 4. mybuilding check�� ���۽���
            F_BuldingInitCheckBuild();

            // 5. modeld�� Material �ٲٱ�
            F_ChangeMaterial(_modelTransform, _nowBuildMaterial);
        }
    }

    private void F_BuldingInitCheckBuild()
    {
        // 1. �ʱ�ȭ
        BuildMaster.Instance.mybuildCheck.F_BuildingStart();

        // 2. ��ᰡ �������? ����ϸ� true,  �ƴϸ� false
        _isEnoughResource = BuildMaster.Instance.mybuildCheck.F_WholeSourseIsEnough();

        // 3. �����е��� ���� material ��ȭ
        if (_isEnoughResource)
            _nowBuildMaterial = _greenMaterial;
        else
            _nowBuildMaterial = _redMaterial;
    }

    #endregion

    #region ���� & �ı�����

    // ���� & �ı����� ���� 
    private void F_RepairAndDestroyTool()
    {
        // 0. ��Ŭ�� ���� ��
        if (Input.GetMouseButtonDown(0))
        {
            // 1. ray ���� finished ���� ������
            RaycastHit _hit;
            if (Physics.Raycast(_player.transform.position, _player.transform.forward * 10, out _hit, 5f, _currTempLayer))   // Ÿ�� : LayerMask
            {
                // 1. myBlock �������� ( �浹�� buildFinished ������Ʈ�� �θ���, mybuildingBlock ��ũ��Ʈ )
                MyBuildingBlock my = _hit.collider.gameObject.transform.parent.GetComponent<MyBuildingBlock>();

                // 2. repair ����
                if (_buildDetailIdx == 0)
                    F_RepairTool(my);
                // 3. destroy ����
                else
                    F_DestroyTool(my);
            }
        }

    }

    private void F_DestroyTool(MyBuildingBlock v_mb)
    {
        // 1. Ŀ���� ������Ʈ ( ���� )
        F_DestroyConnetor((SelectedBuildType)v_mb.MyBlockTypeIdx, v_mb.gameObject.transform);

        // 2. ������Ʈ �ı� ���� ���
        SoundManager.Instance.F_PlaySFX(SFXClip.DESTORY);
    }

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
    #endregion

    #region building ���� �� (��ġ)

    private void F_FinishBuild()
    {
        // 1. �κ� �� ��ᰡ ����ϸ� , �ùٸ� ��ġ�� ������, �ٸ� ������Ʈ�� �浹�� ���°� �ƴϸ� 
        if (_isEnoughResource == true && _isTempValidPosition == true && _isntColliderOther == true)
        {
            // 2. ����
            F_BuildTemp();

            // 0. �÷��̾� �ִϸ��̼� ���� 
            PlayerManager.Instance.PlayerController.F_CreateMotion();

            // 3. �κ��丮 ������Ʈ
            BuildMaster.Instance.mybuildCheck.F_UpdateInvenToBuilding();

            // 4. ���� ���
            SoundManager.Instance.F_PlaySFX(SFXClip.INSTALL);
        }
        else
            return;
    }

    private void F_BuildTemp()
    {
        if (_tempObjectBuilding != null)
        {
            // 0. ����
            GameObject _nowbuild = Instantiate(F_GetCurBuild(_buildTypeIdx, _buildDetailIdx), _tempObjectBuilding.transform.position, _tempObjectBuilding.transform.rotation, _parentTransform);

            // 1. destory
            Destroy(_tempObjectBuilding);
            _tempObjectBuilding = null;

            // 3. model�� material & Layer(buildFinished) ����
            F_ChangeMaterial(_nowbuild.transform.GetChild(0), _oriMaterial);
            F_ChangeLayer(_nowbuild.transform.GetChild(0), _buildFinishedint);

            // 4-1. model�� �ݶ��̴��� is trigger üũ ���� ( Door�� Model �ݶ��̴��� trigger�̿����� )
            if (_SelectBuildType == SelectedBuildType.Door)
                F_ColliderTriggerOnOff(_nowbuild.transform.GetChild(0), true);
            else
                F_ColliderTriggerOnOff(_nowbuild.transform.GetChild(0), false);

            // 4-2. MyModelblock�� ��ġ �� ���� ( �� ������������ ���� x )
            // Destroy(_nowbuild.transform.GetChild(0).gameObject.GetComponent<MyModelBlock>());
            
            // 5. MyBuildingBlock �߰�
            if (_nowbuild.GetComponent<MyBuildingBlock>() == null)
                _nowbuild.AddComponent<MyBuildingBlock>();

            // 5-1. block�� �ʵ� �ʱ�ȭ 
            MyBuildingBlock _nowBuildBlock = _nowbuild.GetComponent<MyBuildingBlock>();
            _nowBuildBlock.F_SetBlockFeild(_buildTypeIdx, _buildDetailIdx % 10,
                BuildMaster.Instance.currBlockData.BlockHp,
                BuildMaster.Instance.currBlockData.BlockMaxHp);

            // 1. Ŀ���� ���� 
            F_CreateConnector( _SelectBuildType , _nowBuildBlock.transform);

            // 0. �� �ڸ��� �����ִ� Ŀ���� destory
            Destroy(_otherConnectorTr.gameObject);
        }
    }


    #endregion


    #region Connector Create

    public Connector F_SettingConnector(SelectedBuildType v_type, Quaternion v_quteRotation)
    {
        switch (v_type)
        {
            case SelectedBuildType.Floor:
                return _connectorArr[0];      // floor Ŀ����  
            case SelectedBuildType.Celling:
                return _connectorArr[1];     // celling Ŀ���� 
            case SelectedBuildType.Wall:                                                 // window, door, window�� ���� wall ���̾� ��� 
            case SelectedBuildType.Door:
            case SelectedBuildType.Window:
                if (v_quteRotation.eulerAngles.y != 0)  // ȸ������ ������?
                    return _connectorArr[3];                                      // ȸ�� 0 wall Ŀ����
                else
                    return _connectorArr[2];                                      // ȸ�� x wall Ŀ����

            default:
                return default;
        }
    }

    // Ÿ�Կ� ���� ȸ�� ���� 
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

    private ConnectorType F_SettingConnectorType(SelectedBuildType v_buildType , Quaternion v_rotation) 
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

    // Type�� ���� ���̾� ���� 
    private LayerMask F_returnLayerType(ConnectorType v_type, Vector3 v_genePosi)
    {
        if (v_type == ConnectorType.CellingConnector && v_genePosi.y == 0)
            return _connectorLayer[(int)ConnectorType.FloorConnector].Item1;
        else
            return _connectorLayer[(int)v_type].Item1;
    }

    // �ش� ��ġ���� Lay
    private Collider[] F_DetectOBject(Vector3 v_posi, LayerMask v_layer)
    {
        // �ش� ��ġ���� layermask �� �˻��ؼ� return
        Collider[] _coll = Physics.OverlapSphere(v_posi, 1f, v_layer);

        return _coll;
    }

    // Ŀ���� ���� 
    private void F_InstaceConnector(Vector3 v_genePosi, ConnectorType v_type)
    {
        // wall �ε�, ���Ͽ� (y<0) ��ü�Ǹ�?
        if ( (v_type == ConnectorType.RotatedWallConnector || v_type == ConnectorType.BasicWallConnector )
            &&v_genePosi.y <= 0)
            return;

        GameObject _connectorInstance = Instantiate(_connectorObject, v_genePosi, Quaternion.identity);
        _connectorInstance.transform.parent = _parentTransform;     // �θ��� 

        // 2. dir�� ���� ȸ���� ����, ����
        _connectorInstance.transform.rotation = F_SettingTypeToRatation(v_type);

        // 3. myBuildingBlock �߰�
        _connectorInstance.GetComponent<MyBuildingBlock>().F_SetBlockFeild( ((int)v_type + 1) * - 1 , -1, -100, -100);      // floor : -1 , celling : -2 , basic wall : -3 , rotated wall : -4 

        // 4. ���̾� , Ÿ���� celling�� ��, celling�� y�� 0 �̸�? -> floor���̾����
        if(v_type == ConnectorType.CellingConnector && v_genePosi.y == 0  )
            _connectorInstance.layer = _connectorLayer[(int)ConnectorType.FloorConnector].Item2;
        else
            _connectorInstance.layer = _connectorLayer[(int)v_type].Item2;

    }

    private void F_CreateConnector( SelectedBuildType v_type ,Transform v_stanardTrs) // ������ �Ǵ� ���� trs 
    {
        // 0. _currConnector Setting 
        _currConnector = F_SettingConnector(v_type, v_stanardTrs.rotation);

        // ���� connector���� List �� ���� 
        for (int i = 0; i < _currConnector.connectorList.Count; i++)
        {
            ConnectorType _conType = _currConnector.connectorList[i].Item1;
            Vector3 _posi = _currConnector.connectorList[i].Item2 + v_stanardTrs.position;

            // Layermask + buildFinished LayerMask 
            Collider[] coll = F_DetectOBject(_posi , F_returnLayerType( _conType, _posi) | _buildFinishedLayer);

            // 2. �˻��ؼ� ������? ��ġ x
            if (coll.Length > 0)
                continue;

            // 3. �˻��ؼ� �� ������? -> ��ġ0
            else
                F_InstaceConnector(_posi , _conType );
            
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
        ConnectorType _standartConnectorType = F_SettingConnectorType( v_buildType , _standartRota);

        // 0. ���� �� �������� Ŀ���� �˻� , wholeLayer �˻� -> hashSet�� ��Ƶα� (�ߺ�x) 
        // 1. Ŀ���� Ÿ�� ���� ���� ��
        // 2. 1�� ��ġ���� Ŀ���� �˻�
        // 2-1. buildFinished�� ������? -> Ŀ���ʹ� �����־����
        // 2-2. `` ������ -> Ŀ���� �����ؾ��� 

        // 0. Ŀ���� �����ϱ� 
        _currConnector = F_SettingConnector(v_buildType, _standartRota);

        for (int i = 0; i < _currConnector.connectorList.Count; i++)
        {
            ConnectorType _type = _currConnector.connectorList[i].Item1;
            Vector3 _position = _currConnector.connectorList[i].Item2 + _standartPosi;

            Collider[] coll = F_DetectOBject(_position , _ConnectorWholelayer);     // ������ġ����, ��üĿ���ͷ��̾�

            // buildConnector�� �����ϰ�, Connector�� �������Ǹ�
            if (coll.Length > 0) 
            {
                // 0. �ߺ�����, hashSet�� ��� 
                if (_detectConnectorOnDestroyBlock.Add(new Tuple<ConnectorType, Vector3>(_type, _position))) 
                {
                    // haset�� �ߺ��Ȱ� ��� �������?
                    _connectorList.Add( coll[0].gameObject );
                }
            }
        }

        StartCoroutine(F_Test(_connectorList , _standartPosi , _standartConnectorType));
    }

    IEnumerator F_Test(List<GameObject> v_connectorList , Vector3 v_stanPosi , ConnectorType v_stanConType) 
    {
        yield return new WaitForSeconds(0.1f);

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

                Collider[] _coll = F_DetectOBject(_posiposi, _buildFinishedLayer);    // ������ġ����, buildFInisehd ���̾� �˻�

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

        yield return new WaitForSeconds(0.1f);

        // ���� �� ���� �� ������ �� ��ġ�� Ŀ���� ��ġ => Ŀ���Ͱ� �ΰ� ��ġ�ɼ��� ?
        LayerMask _mask = F_returnLayerType( v_stanConType , v_stanPosi );
        Collider[] coll = F_DetectOBject( v_stanPosi , _mask );

        if (coll.Length <= 0)
            F_InstaceConnector(v_stanPosi, v_stanConType);

    }

    #endregion

    #region saveManager 

    // ������ ������ ���� ��, �ʱ⿡ ���� 
    public void F_FirstInitBaseFloor() 
    {
        _buildTypeIdx = 0;
        _buildDetailIdx = 0;
        Vector3 _buildVec = Vector3.zero;

        // 1. floor ���� 
        for (int i = -1; i <= 1; i++) 
        {
            for (int j = -1; j <= 1; j++) 
            {
                _buildVec = new Vector3( j * 5 , 0 , i * -5);

                // 1. �⺻�� ���� 
                GameObject _nowbuild = Instantiate( F_GetCurBuild( _buildTypeIdx , _buildDetailIdx ), _buildVec, Quaternion.identity , _parentTransform);
            }
        }

        // 2. Ŀ���� ������Ʈ 
        // 2-1. �ʱ� N�� ���� ���� Ŀ���� ������Ʈ ( parentTransform�� childCount�� �ϸ� ��Ӵþ�� ���ѷ��� )
        for (int i = 0; i < 9; i++)
        {
            // 2-1-2. Ŀ���� create 
            F_CreateConnector( (SelectedBuildType) 0 , _parentTransform.GetChild(i));
        }

    }

    // ���̺� �ҷ��� ��, MyBuldingBlock�� �ʵ� ä���ֱ�
    public void F_CreateBlockFromSave(int _t , int _d , Vector3 _trs , Vector3 _ro , int _h , int _maxhp) 
    {
        // Ÿ�� �ε���, �������ε���, ��ġ, ȸ��, hp , �ִ� hp

        // 1. ���� ,  �θ����� 
        GameObject _nowbuild;

        // �ε����� �����̸� ? -> Connector 
        if (_t < 0)
        { 
            _nowbuild = Instantiate(_connectorObject , _trs, Quaternion.identity , _parentTransform);
            _nowbuild.layer = _connectorLayer[ (_t * -1) - 1 ].Item2;
        }
        else
            _nowbuild = Instantiate(F_GetCurBuild(_t, _d), _trs, Quaternion.identity, _parentTransform);

        // 2. �� ȸ�� ����
        Quaternion _qu = Quaternion.Euler(_ro);
        _nowbuild.transform.rotation = _qu;

        // myBuildingBlock�� ��ũ��Ʈ�� ������������
        if( _nowbuild.GetComponent<MyBuildingBlock>() == null)
            _nowbuild.AddComponent<MyBuildingBlock>();
        
        // 3-5. block�� �ʵ�
        MyBuildingBlock _tmpBlock = _nowbuild.GetComponent<MyBuildingBlock>();
        // �ʵ� ����
        _tmpBlock.F_SetBlockFeild( _t, _d, _h , _maxhp );

    }


    #endregion

    #region chagneEct


    // Collider�� Trigger OnOff
    private void F_ColliderTriggerOnOff( Transform v_trs , bool v_flag) 
    {
        v_trs.GetComponent<Collider>().isTrigger = v_flag;
    }

    // Material change
    private void F_ChangeMaterial( Transform v_pa , Material material ) 
    {
        foreach ( MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>()) 
        {
            msr.material = material;
        }
    }

    // Mesh OnOff
    private void F_MeshOnOff( Transform v_pa , bool v_flag) 
    {
        foreach (MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>())
        {
            msr.enabled = v_flag;
        }
    }

    // ���̾� ����
    private void F_ChangeLayer( Transform v_pa , int v_layer  )    // ���ӿ�����Ʈ�� ���̾ �ٲ� ���� int ������
    {
        for (int i = 0; i < v_pa.childCount; i++)
        {
            v_pa.GetChild(i).gameObject.layer = v_layer;
        }
        
    }
    #endregion

    #region Player Controller
    
    // �Ǽ� ���� ������ �� �ʱ�ȭ �Լ� 
    public void F_InitBuildngMode() 
    {
        // 0. ���� �����ϰ� �ִ� building �ڷ�ƾ ���� 
        StopAllCoroutines();

        // 1. buildingProgressUi ����
        BuildMaster.Instance.housingUiManager._buildingProgressUi.gameObject.SetActive(false);

        // 2. Ȥ�ó� �������� housing ui�� ���� 
        BuildMaster.Instance.housingUiManager._buildingCanvas.gameObject.SetActive(false);

        // 3. preview ������Ʈ ����
        if (_tempObjectBuilding != null)
            Destroy(_tempObjectBuilding);
        _tempObjectBuilding = null;

    }

    #endregion

    #region MyModelBlock
    public void F_IsntCollChagneMaterail( int v_num ) 
    { 
        switch (v_num) 
        {
            // �ʷϻ����� model ��ȭ
            case 0:
                F_ChangeMaterial(_modelTransform, _greenMaterial );
                break;
            // ���������� model ��ȭ
            case 1:
                F_ChangeMaterial(_modelTransform, _redMaterial );
                break;
        }
    }

    #endregion
}
