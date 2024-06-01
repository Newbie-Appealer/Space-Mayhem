using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Housing_SnapBuild : MonoBehaviour
{
    // MyBuildManager에서 받아온 오브젝트를 snap , Build 
    [SerializeField] GameObject _snapTempObject;
    [SerializeField] SelectedBuildType _snapSelectBuildType;
    [HideInInspector] Transform _otherConnectorTr;           // 충돌한 다른 오브젝트 

    [Header("===Temp Object Setting===")]
    [SerializeField] bool _isTempValidPosition;             // 임시 오브젝트가 지어질 수 있는지

    private int _snapObjectTypeIdx;
    private int _snapObjectDetailIdx;
    private Material _snapOrimaterial;

    private void Start()
    {
        // 2. 상태 초기화
        _isTempValidPosition = true;
    }

    public void F_OtherBuildBlockBuild(SelectedBuildType v_snapType , GameObject v_snapObject , LayerMask v_layermask)
    {
        // 임시로 담아놓기
        _snapTempObject = v_snapObject;
        _snapSelectBuildType = v_snapType;
        _snapObjectTypeIdx = BuildMaster.Instance._buildTypeIdx;
        _snapObjectDetailIdx = BuildMaster.Instance._buildDetailIdx;
        _snapOrimaterial = BuildMaster.Instance.myBuildManger._oriMaterial;

        // 2. 해당 블럭이랑 같은 Layer만 raycast
        F_Raycast(v_layermask);

        // 3. temp 오브젝트의 콜라이더 검사
        F_CheckCollision(v_layermask);

        // 4. 설치
        if (Input.GetMouseButtonDown(0))
            F_FinishBuild();
    }

    private void F_Raycast(LayerMask v_layer)
    {
        // 1. 넘어온 Layer'만' rayCast
        RaycastHit _hit;

        // ##TODO 여기서 playermanager의 trasform을 가져오는게 낫나? 근데 그러면 매프레임 가져와야함 
        // 2. raycast 되면 -> 임시 오브젝트를 그 위치로 옮기기 
        if (Physics.Raycast
            (BuildMaster.Instance._playerCamera.transform.position, 
                BuildMaster.Instance._playerCamera.transform.forward * 10, out _hit, 5f, v_layer)) // 타입 : LayerMask
        {
            _snapTempObject.transform.position = _hit.point;
        }
    }

    private void F_CheckCollision(LayerMask v_layer)
    {
        // 1. 콜라이더 검사 
        Collider[] _coll = Physics.OverlapSphere(_snapTempObject.transform.position, 1f, v_layer);    // 타입 : LayerMask

        // 2. 검사되면 -> 오브젝트 Snap
        if (_coll.Length > 0)
            F_Snap(_coll);
        else
            _isTempValidPosition = false;

    }

    private void F_Snap(Collider[] v_coll)
    {
        // 0. 다른 커넥터? -> 배열에 처음으로 들어온 collider
        _otherConnectorTr = v_coll[0].transform;

        // 1. 타입이 wall 일때는 회전 
        if (_snapSelectBuildType == SelectedBuildType.Wall || _snapSelectBuildType == SelectedBuildType.Window
            || _snapSelectBuildType == SelectedBuildType.Door)
        {
            // 내 temp 블럭 회전 += 접촉한 커넥터의 회전
            Quaternion qu = _snapTempObject.transform.rotation;
            qu.eulerAngles = new Vector3(qu.eulerAngles.x, _otherConnectorTr.eulerAngles.y, qu.eulerAngles.z);
            _snapTempObject.transform.rotation = qu;
        }

        // 2. Snap!! 
        _snapTempObject.transform.position
             = _otherConnectorTr.position;

        // 3. mesh 켜기
        F_MeshOnOff(BuildMaster.Instance.myBuildManger._modelTransform, true);
        
        // 4. 설치가능 
        _isTempValidPosition = true;
    }

    private void F_FinishBuild()
    {
        bool _snapIsEnoughResource = BuildMaster.Instance.myBuildManger._isEnoughResource;
        // 1. 인벤 내 재료가 충분하면 , 올바른 위치에 있으면, 다른 오브젝트랑 충돌한 상태가 아니면 
        if (_snapIsEnoughResource == true && _isTempValidPosition == true )
        {
            // 2. 짓기
            F_BuildTemp();

            // 0. 플레이어 애니메이션 실행 
            PlayerManager.Instance.PlayerController.F_CreateMotion();

            // 3. 인벤토리 업데이트
            BuildMaster.Instance.mybuildCheck.F_UpdateInvenToBuilding();

            // 4. 사운드 재생
            SoundManager.Instance.F_PlaySFX(SFXClip.INSTALL);
        }
        else
            return;
    }

    private void F_BuildTemp()
    {
        if (_snapTempObject != null)
        {
            // 0. 생성
            GameObject _nowbuild = Instantiate( BuildMaster.Instance.myBuildManger.F_GetCurBuild(_snapObjectTypeIdx, _snapObjectDetailIdx),
                _snapTempObject.transform.position, _snapTempObject.transform.rotation, 
                BuildMaster.Instance._parentTransform);

            // 1. destory
            Destroy(BuildMaster.Instance.myBuildManger._tempObject);
            BuildMaster.Instance.myBuildManger._tempObject = null;

            // 3. model의 material & Layer(buildFinished) 변겅
            BuildMaster.Instance.F_ChangeMaterial(_nowbuild.transform.GetChild(0), _snapOrimaterial);
            _nowbuild.transform.GetChild(0).gameObject.layer = BuildMaster.Instance._buildFinishedint;

            // 4-1. model의 콜라이더를 is trigger 체크 해제 ( Door은 Model 콜라이더가 trigger이여야함 )
            if (_snapSelectBuildType == SelectedBuildType.Door)
                BuildMaster.Instance.F_ColliderTriggerOnOff(_nowbuild.transform.GetChild(0), true);
            else
                BuildMaster.Instance.F_ColliderTriggerOnOff(_nowbuild.transform.GetChild(0), false);

            // 4-2. MyModelblock은 설치 후 삭제 ( 다 지어지고나서는 쓸모가 x )
            // Destroy(_nowbuild.transform.GetChild(0).gameObject.GetComponent<MyModelBlock>());

            // 5. MyBuildingBlock 추가
            if (_nowbuild.GetComponent<MyBuildingBlock>() == null)
                _nowbuild.AddComponent<MyBuildingBlock>();

            // 5-1. block에 필드 초기화 
            MyBuildingBlock _nowBuildBlock = _nowbuild.GetComponent<MyBuildingBlock>();
            _nowBuildBlock.F_SetBlockFeild(_snapObjectTypeIdx, _snapObjectDetailIdx % 10,
                BuildMaster.Instance.currBlockData.BlockHp,
                BuildMaster.Instance.currBlockData.BlockMaxHp);

            // 1. 커넥터 지정 
            BuildMaster.Instance.housingRepairDestroy.F_CreateConnector(_snapSelectBuildType, _nowBuildBlock.transform);

            // 0. 그 자리에 원래있던 커넥터 destory
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
