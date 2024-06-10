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
    [SerializeField] bool _isntColliderPlacedItem;          // 설치완료된 오브젝트와 겹치는지 ?

    // MyBuildManager에서 받아온 변수 
    private int _snapObjectTypeIdx;                         
    private int _snapObjectDetailIdx;
    private Material _snapOrimaterial;

    // 프로퍼티 
    public bool isntColliderPlacedItem { get => _isntColliderPlacedItem; set { _isntColliderPlacedItem = value; } } 

    private void Start()
    {
        // 2. 상태 초기화
        _isTempValidPosition    = true;
        _isntColliderPlacedItem = true;
    }

    public void F_OtherBuildBlockBuild(SelectedBuildType v_snapType , GameObject v_snapObject , LayerMask v_layermask)
    {
        // 임시로 담아놓기
        _snapTempObject = v_snapObject;
        _snapSelectBuildType = v_snapType;
        _snapObjectTypeIdx = BuildMaster.Instance._buildTypeIdx;
        _snapObjectDetailIdx = BuildMaster.Instance._buildDetailIdx;
        _snapOrimaterial = BuildMaster.Instance.myBuildManger._oriMaterial;

        // 초기화
        _isTempValidPosition = true;
        _isntColliderPlacedItem = true;

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

        // 3. 회전 
        F_BlockRotationInputR();

        // 4. mesh 켜기
        F_MeshOnOff(BuildMaster.Instance.myBuildManger._modelTransform, true);
        
        // 5. 설치가능 
        _isTempValidPosition = true;
    }

    // r 누를 때 회전  
    private void F_BlockRotationInputR() 
    {
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            Vector3 _blockRoatate = BuildMaster.Instance.currBlockData.blockRotation;

            Quaternion _qu = _snapTempObject.transform.rotation;
            _qu.eulerAngles += new Vector3( _blockRoatate.x , _blockRoatate.y , _blockRoatate.z );
            _snapTempObject.transform.rotation = _qu;
        }
    }

    private void F_FinishBuild()
    {
        // 0. 재료 변수 가져오기 
        bool _snapIsEnoughResource = BuildMaster.Instance.myBuildManger._isEnoughResource;

        // 1. 인벤 내 재료가 충분하면 , 올바른 위치에 있으면, 다른 오브젝트랑 충돌한 상태가 아니면 
        if (_snapIsEnoughResource == true && _isTempValidPosition == true && _isntColliderPlacedItem == true )
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

            // 3. model의 material 변경 
            Transform _nowBuildObjModel = _nowbuild.transform.GetChild(0);
            BuildMaster.Instance.F_ChangeMaterial(_nowBuildObjModel, _snapOrimaterial);         // material 바꾸기

            // 4-1. collider group 오브젝트의 하위 콜라이더를 trigger Off
            BuildMaster.Instance.F_ColliderTriggerOnOff(_nowbuild.transform.GetChild(1), false);

            // 4-2. MyModelblock은 설치 후 삭제 ( 기본 : 설치되어있는 상태 )
            BuildMaster.Instance.F_DestoryMyModelBlockUnderParent( _nowBuildObjModel );

            // 5.myBuildingBlock & outline 추가 
            BuildMaster.Instance.F_AddNecessaryComponent(_nowbuild);

            // 5-1. block에 필드 초기화 
            MyBuildingBlock _nowBuildBlock = _nowbuild.GetComponent<MyBuildingBlock>();
            _nowBuildBlock.F_SetBlockFeild(_snapObjectTypeIdx, _snapObjectDetailIdx,
                BuildMaster.Instance.currBlockData.blockHp,
                BuildMaster.Instance.currBlockData.blockMaxHp);

            // 1. 커넥터 지정 
            BuildMaster.Instance.housingRepairDestroy.F_CreateConnector( _nowBuildBlock.transform );

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
