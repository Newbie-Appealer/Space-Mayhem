using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Housing_SnapBuild : MonoBehaviour
{
    // MyBuildManager���� �޾ƿ� ������Ʈ�� snap , Build 
    [SerializeField] GameObject _snapTempObject;
    [SerializeField] SelectedBuildType _snapSelectBuildType;
    [HideInInspector] Transform _otherConnectorTr;           // �浹�� �ٸ� ������Ʈ 

    [Header("===Temp Object Setting===")]
    [SerializeField] bool _isTempValidPosition;             // �ӽ� ������Ʈ�� ������ �� �ִ���

    private int _snapObjectTypeIdx;
    private int _snapObjectDetailIdx;
    private Material _snapOrimaterial;

    private void Start()
    {
        // 2. ���� �ʱ�ȭ
        _isTempValidPosition = true;
    }

    public void F_OtherBuildBlockBuild(SelectedBuildType v_snapType , GameObject v_snapObject , LayerMask v_layermask)
    {
        // �ӽ÷� ��Ƴ���
        _snapTempObject = v_snapObject;
        _snapSelectBuildType = v_snapType;
        _snapObjectTypeIdx = BuildMaster.Instance._buildTypeIdx;
        _snapObjectDetailIdx = BuildMaster.Instance._buildDetailIdx;
        _snapOrimaterial = BuildMaster.Instance.myBuildManger._oriMaterial;

        // 2. �ش� �����̶� ���� Layer�� raycast
        F_Raycast(v_layermask);

        // 3. temp ������Ʈ�� �ݶ��̴� �˻�
        F_CheckCollision(v_layermask);

        // 4. ��ġ
        if (Input.GetMouseButtonDown(0))
            F_FinishBuild();
    }

    private void F_Raycast(LayerMask v_layer)
    {
        // 1. �Ѿ�� Layer'��' rayCast
        RaycastHit _hit;

        // ##TODO ���⼭ playermanager�� trasform�� �������°� ����? �ٵ� �׷��� �������� �����;��� 
        // 2. raycast �Ǹ� -> �ӽ� ������Ʈ�� �� ��ġ�� �ű�� 
        if (Physics.Raycast
            (BuildMaster.Instance._playerCamera.transform.position, 
                BuildMaster.Instance._playerCamera.transform.forward * 10, out _hit, 5f, v_layer)) // Ÿ�� : LayerMask
        {
            _snapTempObject.transform.position = _hit.point;
        }
    }

    private void F_CheckCollision(LayerMask v_layer)
    {
        // 1. �ݶ��̴� �˻� 
        Collider[] _coll = Physics.OverlapSphere(_snapTempObject.transform.position, 1f, v_layer);    // Ÿ�� : LayerMask

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
        if (_snapSelectBuildType == SelectedBuildType.Wall || _snapSelectBuildType == SelectedBuildType.Window
            || _snapSelectBuildType == SelectedBuildType.Door)
        {
            // �� temp ���� ȸ�� += ������ Ŀ������ ȸ��
            Quaternion qu = _snapTempObject.transform.rotation;
            qu.eulerAngles = new Vector3(qu.eulerAngles.x, _otherConnectorTr.eulerAngles.y, qu.eulerAngles.z);
            _snapTempObject.transform.rotation = qu;
        }

        // 2. Snap!! 
        _snapTempObject.transform.position
             = _otherConnectorTr.position;

        // 3. mesh �ѱ�
        F_MeshOnOff(BuildMaster.Instance.myBuildManger._modelTransform, true);
        
        // 4. ��ġ���� 
        _isTempValidPosition = true;
    }

    private void F_FinishBuild()
    {
        bool _snapIsEnoughResource = BuildMaster.Instance.myBuildManger._isEnoughResource;
        // 1. �κ� �� ��ᰡ ����ϸ� , �ùٸ� ��ġ�� ������, �ٸ� ������Ʈ�� �浹�� ���°� �ƴϸ� 
        if (_snapIsEnoughResource == true && _isTempValidPosition == true )
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
        if (_snapTempObject != null)
        {
            // 0. ����
            GameObject _nowbuild = Instantiate( BuildMaster.Instance.myBuildManger.F_GetCurBuild(_snapObjectTypeIdx, _snapObjectDetailIdx),
                _snapTempObject.transform.position, _snapTempObject.transform.rotation, 
                BuildMaster.Instance._parentTransform);

            // 1. destory
            Destroy(BuildMaster.Instance.myBuildManger._tempObject);
            BuildMaster.Instance.myBuildManger._tempObject = null;

            // 3. model�� material & Layer(buildFinished) ����
            BuildMaster.Instance.F_ChangeMaterial(_nowbuild.transform.GetChild(0), _snapOrimaterial);
            _nowbuild.transform.GetChild(0).gameObject.layer = BuildMaster.Instance._buildFinishedint;

            // 4-1. model�� �ݶ��̴��� is trigger üũ ���� ( Door�� Model �ݶ��̴��� trigger�̿����� )
            if (_snapSelectBuildType == SelectedBuildType.Door)
                BuildMaster.Instance.F_ColliderTriggerOnOff(_nowbuild.transform.GetChild(0), true);
            else
                BuildMaster.Instance.F_ColliderTriggerOnOff(_nowbuild.transform.GetChild(0), false);

            // 4-2. MyModelblock�� ��ġ �� ���� ( �� �������������� ���� x )
            // Destroy(_nowbuild.transform.GetChild(0).gameObject.GetComponent<MyModelBlock>());

            // 5. MyBuildingBlock �߰�
            if (_nowbuild.GetComponent<MyBuildingBlock>() == null)
                _nowbuild.AddComponent<MyBuildingBlock>();

            // 5-1. block�� �ʵ� �ʱ�ȭ 
            MyBuildingBlock _nowBuildBlock = _nowbuild.GetComponent<MyBuildingBlock>();
            _nowBuildBlock.F_SetBlockFeild(_snapObjectTypeIdx, _snapObjectDetailIdx % 10,
                BuildMaster.Instance.currBlockData.blockHp,
                BuildMaster.Instance.currBlockData.blockMaxHp);

            // 1. Ŀ���� ���� 
            BuildMaster.Instance.housingRepairDestroy.F_CreateConnector(_snapSelectBuildType, _nowBuildBlock.transform);

            // 0. �� �ڸ��� �����ִ� Ŀ���� destory
            Destroy(_otherConnectorTr.gameObject);
        }
    }

    private void F_MeshOnOff(Transform v_pa, bool v_flag)
    {
        foreach (MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>())
        {
            msr.enabled = v_flag;
        }
    }


}