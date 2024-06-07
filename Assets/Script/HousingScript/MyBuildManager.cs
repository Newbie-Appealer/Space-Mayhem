using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MyBuildManager : MonoBehaviour
{
    [Header("===Block Transform===")]
    [HideInInspector] public GameObject _tempObject;        // �ӽ� ������Ʈ

    [HideInInspector] public Transform _modelTransform;             // �� ������Ʈ 
    [HideInInspector] public Transform _colliderGroupTrasform;      // �ݶ��̴� �θ� ������Ʈ 

    [Header("===Type===")]
    [HideInInspector] private SelectedBuildType _SelectBuildType;

    #region prefab
    [SerializeField] public List<List<GameObject>> _bundleBulingPrefab;

    [Header("===Build Object===")]
    [SerializeField] private List<GameObject> _floorList;
    [SerializeField] private List<GameObject> _cellingList;
    [SerializeField] private List<GameObject> _wallList;
    [SerializeField] private List<GameObject> _doorList;
    [SerializeField] private List<GameObject> _windowList;
    #endregion

    [Header("===LayerMask===")]
    [SerializeField] LayerMask _currTempLayer;              // ���� (temp ���� ������) ���̾� 

    #region Material
    [Header("===Material===")]
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;
    [HideInInspector] public Material _oriMaterial;
    [HideInInspector] Material _nowBuildMaterial;
    #endregion

    public bool _isEnoughResource;                          // ��ġ�ϱ⿡ ��ᰡ �������
    private int _housingManager_Typeidx;                    // myBuildManager���� ����� type �ε���
    private int _housingManager_Detailidx;                  // myBuildingManager���� ����� detail 

    // =============================================
    private void Awake()
    {
        // 1. �ʱ�ȭ                      
        _bundleBulingPrefab = new List<List<GameObject>> // �� block ������ List�� �ϳ��� List�� ����
        {
            _floorList,
            _cellingList,
            _wallList,
            _doorList,
            _windowList
        };

        // 1. ��� ���� �ʱ�ȭ -> buildManager���� ���� 
        _isEnoughResource = false;

    }

    public void F_GetbuildType(int v_type = 0, int v_detail = 1)
    {
        // 0. Ui�� �߸��� idx�� �Ѿ���� �� 
        if (v_type < 0 && v_detail < 0)
            return;

        // 1. buildMater�� index �ֱ� 
        BuildMaster.Instance.F_SetHousnigIdx(v_type , v_detail);

        // 1-1. �� ��ũ��Ʈ���� ����� idx �������� 
        _housingManager_Typeidx = BuildMaster.Instance._buildTypeIdx;
        _housingManager_Detailidx = BuildMaster.Instance._buildDetailIdx;

        // 2. �ӽ� ������Ʈ Ȯ��
        if (_tempObject != null)
            Destroy(_tempObject);
        _tempObject = null;

        // 3. building check �ʱ�ȭ
        BuildMaster.Instance.mybuildCheck.F_BuildingStart();

        // 4. ���� ���� 
        StopAllCoroutines();
        StartCoroutine(F_TempBuild());
    }

    IEnumerator F_TempBuild()
    {
        // 0. index�� �ش��ϴ� ���� ������Ʈ return
        GameObject _currBuild = F_GetCurBuild(_housingManager_Typeidx, _housingManager_Detailidx);
        // 0.1. �� �� Ÿ�Կ� ���� �˻��� layer , Conneector ���ϱ� 
        F_SettingCurrLayer(_SelectBuildType);

        // 0.2. type�� ���� progress on off ui ����
        F_OnOffProgressUI();

        while (true)
        {
            // �������� type�� �ı����� �� ��
            if (_SelectBuildType == SelectedBuildType.RepairTools)
            {
                BuildMaster.Instance.housingRepairDestroy.F_RepairAndDestroyTool( _currTempLayer );
            }
            // ��������, build type �� �� 
            else
            {
                // 1. index�� �ش��ϴ� ���ӿ�����Ʈ ���� , tempObjectbuilding ������Ʈ ���� 
                F_CreateTempPrefab(_currBuild);

                // 2. ������ ������Ʈ�� snap , tempObjectBuilding ������Ʈ �ѱ�� 
                BuildMaster.Instance.housingSnapBuild.F_OtherBuildBlockBuild( _SelectBuildType ,_tempObject , _currTempLayer );
            }

            // update ȿ�� 
            yield return null;
        }
    }

    private void F_OnOffProgressUI()
    {
        // 0. repair type �� destroy���̸� progressUI ���� 
        if (_SelectBuildType == SelectedBuildType.RepairTools && _housingManager_Detailidx == 1)
            BuildMaster.Instance.housingUiManager.F_OnOFfBuildingProgressUi(false);
        // 1. �ƴϸ� �ѱ�
        else
            BuildMaster.Instance.housingUiManager.F_OnOFfBuildingProgressUi(true);

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
                _currTempLayer = BuildMaster.Instance._connectorLayer[0].Item1;                          // floor ���̾�
                break;
            case SelectedBuildType.Celling:
                _currTempLayer = BuildMaster.Instance._connectorLayer[1].Item1;                          // celling ���̾�
                break;
            case SelectedBuildType.Wall:                                                                 // window, door, window�� ���� wall ���̾� ��� 
            case SelectedBuildType.Door:
            case SelectedBuildType.Window:
                _currTempLayer = BuildMaster.Instance._connectorLayer[2].Item1;                          // wall ���̾�
                break;
            case SelectedBuildType.RepairTools:                                                          // repair �� �� �� 
                _currTempLayer = BuildMaster.Instance._buildFinishedLayer;                               // buildFinished
                break;
        }
    }

    private void F_CreateTempPrefab(GameObject v_temp)
    {
        // ��������
        // temp������Ʈ�� null�̵Ǹ� �ٷ� ������!
        if (_tempObject == null)
        {
            // 1. ���� & 100f,100f,100f�� �ӽ���ġ 
            _tempObject = Instantiate(v_temp, new Vector3(100f, 100f, 100f), Quaternion.identity);

            // 2. model Transform & collider group Transform 
            _modelTransform         = _tempObject.transform.GetChild(0);
            _colliderGroupTrasform  = _tempObject.transform.GetChild(1);

            // 2-1. ������Ʈ ���� collider ������Ʈ�� ���� �ݶ��̴��� trigger 
            BuildMaster.Instance.F_ColliderTriggerOnOff(_colliderGroupTrasform, true);

            // 3. ���� material ����
            _oriMaterial = _modelTransform.GetChild(0).GetComponent<MeshRenderer>().material;

            // 4. mybuilding check�� ���۽���
            F_BuldingInitCheckBuild();

            // 5. modeld�� Material �ٲٱ�
            BuildMaster.Instance.F_ChangeMaterial(_modelTransform, _nowBuildMaterial);
        }
    }

    public void F_BuldingInitCheckBuild()
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


    #region Player Controller
    
    // �Ǽ� ���� ������ �� �ʱ�ȭ �Լ� 
    public void F_InitBuildngMode() 
    {
        // 0. ���� �����ϰ� �ִ� building �ڷ�ƾ ���� 
        StopAllCoroutines();

        // 1. buildingProgressUi ����
        BuildMaster.Instance.housingUiManager._buildingProgressUi.gameObject.SetActive(false);

        // 2. Ȥ�ó� �������� housing ui�� ���� 
        BuildMaster.Instance.housingUiManager._buildingBlockSelectUi.gameObject.SetActive(false);

        // 3. preview ������Ʈ ����
        if (_tempObject != null)
            Destroy(_tempObject);
        _tempObject = null;

    }

    #endregion

    #region MyModelBlock / ������� x 
    
    public void F_IsntCollChagneMaterail( int v_num ) 
    {
        // model�� �������� Material�� �ٲ� ( ���� : save ���� �ҷ��� �� , modelTrs�� null �ε� �Լ�ȣ��� (�ν��Ͻ�ȭ -> �ݶ��̴� ���� -> ����)
        if (_modelTransform == null)
            return;

        switch (v_num) 
        {
            // �ʷϻ����� model ��ȭ
            case 0:
                BuildMaster.Instance.F_ChangeMaterial(_modelTransform, _greenMaterial );
                break;
            // ���������� model ��ȭ
            case 1:
                BuildMaster.Instance.F_ChangeMaterial(_modelTransform, _redMaterial );
                break;
        }
    }
    
    #endregion
}
