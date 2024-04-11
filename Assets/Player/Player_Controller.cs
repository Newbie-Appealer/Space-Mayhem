using System.Collections;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public delegate void playerMoveDelegate();
    public playerMoveDelegate playerController;

    [Header("== Player Move ==")]
    [SerializeField] private Animator _player_Arm_Ani;
    [SerializeField] private Animator _player_Arm_Weapon_Ani;
    [SerializeField] private GameObject _player_Arm;
    [SerializeField] private GameObject _player_Arm_Weapon;
    private Animator _player_Animation;
    
    //테스트용, 나중에 도구들 더 생기면 배열로 저장할 예정
    [SerializeField] private Animator _player_FarmingGun_Ani;
    private Rigidbody _rb;
    private CapsuleCollider _cd;

    //0 : 걷는 속도, 1 : 뛰는 속도, 2 : 앉으며 걷는 속도, 3: 앉으며 뛰는 속도
    private float[] _speed_Array;
    private float _moveSpeed;
    private float _jumpSpeed = 5f;
    private bool _isGrounded = true;
    private bool _isCrouched = false;
    private bool _isOnLadder = false;

    [Header("== Camera Move ==")]
    [SerializeField] private Camera _player_Camera;
    [SerializeField] private float _mouseSensitivity = 500f; 
    private float _cameraPosY;                      //현재 카메라 포지션 y축
    private float _camera_Crouched_PosY;            //앉았을 때 카메라 포지션 y축
    private Vector3 _rotationX;
    private float _rotationY;

    [Header("== 상호작용 LayerMask ==")]
    [SerializeField] private LayerMask _item_LayerMask;
    [SerializeField] private LayerMask _furniture_LayerMask;
    private LayerMask combLayerMask => _item_LayerMask | _furniture_LayerMask;

    [SerializeField] private Pistol _pistol;
    [SerializeField] private GameObject _repair_tool;
    private RaycastHit _hitInfo;
    private float _item_CanGetRange = 5f;

    public void F_initController()
    {
        _rb = GetComponent<Rigidbody>();
        _cd = GetComponent<CapsuleCollider>();
        _player_Animation = _player_Arm_Ani;

        _cameraPosY = _player_Camera.transform.localPosition.y;
        _speed_Array = new float[4];
        _speed_Array[0] = 4f;
        _speed_Array[1] = 8f;
        _speed_Array[2] = 2f;
        _speed_Array[3] = 3.5f;

        F_initDelegate();
    }

    #region Delegate
    private void F_initDelegate()
    {
        // 플레이어 기본 움직임 델리게이트 초기화
        playerController = F_PlayerCrouch;
        playerController += F_PlayerRun;
        playerController += F_PlayerCameraHorizonMove;
        playerController += F_PlayerCameraVerticalMove;
        playerController += F_PlayerMove;
        playerController += F_PlayerActionRayCast;
    }

    /// <summary> </summary>
    public void F_ChangeState(PlayerState v_state, int v_uniqueCode)
    {
        BuildMaster.Instance.myBuildManger.F_InitBuildngMode();            // 건설모드 초기화
        ItemManager.Instance.installSystem.F_InitInstall();     // 설치모드 초기화
        _pistol.F_InitSpear(); 

        switch (v_state)
        {
            case PlayerState.NONE:
                F_EmptyHand();
                playerController -= F_FarmingFunction;
                playerController -= F_InstallFunction;
                break;

            case PlayerState.FARMING:
                F_EquipTool(v_uniqueCode);
                playerController += F_FarmingFunction;
                playerController -= F_InstallFunction;
                break;
            case PlayerState.BUILDING:
                F_EquipTool(v_uniqueCode);
                playerController -= F_FarmingFunction;
                playerController -= F_InstallFunction;
                break;
            case PlayerState.INSTALL:
                F_EmptyHand();
                playerController -= F_FarmingFunction;
                playerController += F_InstallFunction;
                break;
        }
    }
    #endregion

    /// <summary> 도구 드는 함수임 </summary>
    public void F_EquipTool(int v_toolCode)
    {
        _player_Animation = _player_Arm_Weapon_Ani;
        if (_player_Arm.activeSelf)
        {
                _player_Arm.SetActive(false);
                _player_Arm_Weapon.SetActive(true);
        }
        // 파밍 도구 들기
        if (v_toolCode == 0)                         
        {
            _player_Animation.Rebind();
            _pistol.gameObject.SetActive(true);
            _repair_tool.SetActive(false);
            _pistol.F_InitSpear();
        }
        // 건설 도구 들기
        if (v_toolCode == 1)
        {
            _player_Animation.Rebind();
            _pistol.gameObject.SetActive(false);
            _repair_tool.SetActive(true);
        }
    }

    /// <summary> 맨손 겸 행동 초기화 함수 </summary>
    public void F_EmptyHand()
    {
        _player_Animation = _player_Arm_Ani;

        _player_Arm.SetActive(true);
        _player_Arm_Weapon.SetActive(false);
    }
    public void F_FarmingFunction()
    {
        F_PistolFire();
    }

    public void F_InstallFunction()
    {
    }

    public void F_CreateMotion()
    {
        _player_Animation.SetTrigger("Create");
    }

    #region 움직임 관련
    // 달리기 (Shift)
    private void F_PlayerRun()  
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //서있을 때 달리기
            if (!_isCrouched) 
                _moveSpeed = _speed_Array[1];

            //앉아있을 때 달리기
            else _moveSpeed = _speed_Array[3]; 
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            //앉은 채로 걷기
            if (_isCrouched)
                _moveSpeed = _speed_Array[2];

            //선 채로 걷기
            else _moveSpeed = _speed_Array[0];
        }
    }

    private void F_PistolFire()
    {
        if (!PlayerManager.Instance._isSpearFire)
        {
            if (Input.GetMouseButton(0))
                _pistol.F_SpearPowerCharge();
            if (Input.GetMouseButtonUp(0))
            {
                _player_Animation.SetTrigger("Fire");
                _player_FarmingGun_Ani.SetTrigger("Fire");
                PlayerManager.Instance._isSpearFire = true;
                _pistol.F_SpearFire();
            }
        }
        if (Input.GetMouseButton(1))
            _pistol.F_SpearComeBack();
    }

    //앉기 (C)
    private void F_PlayerCrouch()
    {
            if (!_isCrouched)
            {
                _moveSpeed = _speed_Array[0];
                if (Input.GetKeyDown(KeyCode.C))
                {
                    StartCoroutine(C_PlayerCrouch(true, 1.3f, 0.64f, 1.55f));
                }
            }
            if (_isCrouched)
            {
                _moveSpeed = _speed_Array[2];
                if (Input.GetKeyUp(KeyCode.C))
                {
                    StartCoroutine(C_PlayerCrouch(false, 2f, 1f, 2.3f));
                }
            }
    }

    //앉기 코루틴 
    //매개변수 순서 : (앉으면 true, 목표 카메라 Position Y축, CharacterController Y축, CharacterController 키)
    private IEnumerator C_PlayerCrouch(bool v_isCrouched, float v_cameraPosY, float v_chrCenter, float v_chrHeight)
    {
        _isCrouched = v_isCrouched;
        _camera_Crouched_PosY = v_cameraPosY;
        _cd.center  = new Vector3(0, v_chrCenter ,0);
        _cd.height = v_chrHeight;
        while(_cameraPosY != _camera_Crouched_PosY)
        {
            _cameraPosY = Mathf.Lerp(_cameraPosY, _camera_Crouched_PosY, 0.1f);
            _player_Camera.transform.localPosition = new Vector3(0, _cameraPosY, 0);
            if (_isCrouched)
            {
                if (_cameraPosY <= 1.302f)
                {
                    _cameraPosY = 1.3f;
                    break;
                }
            } 
            else
            {
                if (_cameraPosY >= 1.998f)
                {
                    _cameraPosY = 2f;
                    break;
                }
            }
            yield return null;
        }
    }

    private void F_PlayerMove()
    {
        float _input_x = Input.GetAxisRaw("Horizontal");
        float _input_z = Input.GetAxisRaw("Vertical");
        Vector3 _moveVector;
        if (!_isOnLadder)
        {
        _moveVector = (transform.right * _input_x + transform.forward * _input_z).normalized;
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.5f);
        _rb.MovePosition(transform.position + _moveVector * _moveSpeed * Time.deltaTime);
            //스페이스바 누르면 점프
            if (_isGrounded)
            {
                _player_Animation.SetBool("isGround", true);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    F_PlayerJump();
                }
            }
            else if (!_isGrounded)
            {
                _player_Animation.SetBool("isGround", false);
            }
        }
        else if (_isOnLadder)
        {
            F_PlayerLadderMove();
        }
        if (_input_x != 0 || _input_z != 0)
            _player_Animation.SetBool("Walk", true);
        else
            _player_Animation.SetBool("Walk", false);

    }

    private void F_PlayerJump()
    {
        if (!_isCrouched)
            _rb.AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);
        else
            _rb.AddForce(Vector3.up * _jumpSpeed / 2f, ForceMode.Impulse);
        _player_Animation.SetTrigger("Jump");
    }
    private void F_PlayerCameraHorizonMove()
    {
        float _mouseX = Input.GetAxisRaw("Mouse X");
        _rotationX = new Vector3(0, _mouseX, 0) * _mouseSensitivity * Time.deltaTime;
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(_rotationX));
    }
    private void F_PlayerCameraVerticalMove()
    {
        // 커서가 안보일때 !
        float _mouseY = Input.GetAxisRaw("Mouse Y");
        _rotationY -= _mouseY * _mouseSensitivity * Time.deltaTime;

        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);

        _player_Camera.transform.localEulerAngles = new Vector3(_rotationY, 0, 0);
    }
    #endregion

    #region 사다리 타기
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ladder"))
            _isOnLadder = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _rb.useGravity = true;
        _isOnLadder = false;
    }

    private void F_PlayerLadderMove()
    {
        _rb.useGravity = false;
        float _input_z = Input.GetAxis("Vertical");
        float _cameraRorationX = _player_Camera.transform.localRotation.x;
        if ((_input_z > 0 && _cameraRorationX < 0) || (_input_z < 0 && _cameraRorationX < 0))
        {
            _rb.MovePosition(transform.position + new Vector3(0, _input_z, 0) * _moveSpeed * Time.deltaTime);
        }
        else if ((_input_z > 0 && _cameraRorationX > 0) || (_input_z < 0 && _cameraRorationX > 0))
        {
            _rb.MovePosition(transform.position + new Vector3(0, -_input_z, 0) * _moveSpeed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rb.useGravity = true;
            _rb.AddForce(-_player_Camera.transform.forward * _jumpSpeed / 4f, ForceMode.Impulse);
            _player_Animation.SetTrigger("Jump");
        }
    }
    #endregion

    #region 상호작용 관련  
    private void F_PlayerActionRayCast()
    {
        // 인벤토리가 켜져있을때 상호작용 X
        if (UIManager.Instance.onInventory)
            return;

        if (Physics.Raycast(_player_Camera.transform.position, _player_Camera.transform.forward, out _hitInfo, _item_CanGetRange, combLayerMask))
        {
            if(_hitInfo.collider.CompareTag("Scrap"))
                F_ScrapInteraction();

            else if (_hitInfo.collider.CompareTag("InteractionObject"))
                F_FurnitureIntercation();

            return;
        }

        UIManager.Instance.F_IntercationPopup(false, "");
    }

   /// <summary> 우주쓰레기 상호작용 함수 </summary>
    private void F_ScrapInteraction()
    {
        UIManager.Instance.F_IntercationPopup(true, "[E]");

        if (Input.GetKeyDown(KeyCode.E))
        {
            Scrap _hitScrap = _hitInfo.transform.GetComponent<Scrap>();
            int _scrapNum = _hitScrap.scrapNumber;
            string _scrapName = ItemManager.Instance.ItemDatas[_scrapNum]._itemName;

            //작살에 맞은 채로 E로 아이템 획득할 때 예외 처리
            if (_hitScrap.transform.parent.name == _pistol.spear.name && ScrapManager.Instance._scrapHitedSpear.Count > 0)
            {
                if (ScrapManager.Instance._scrapHitedSpear.Count == 1)
                {
                    _pistol.F_InitSpear();
                    _pistol.pistolAni.SetTrigger("Get");
                }
                else
                    ScrapManager.Instance._scrapHitedSpear.Remove(_hitScrap);
            }

            StartCoroutine(UIManager.Instance.C_GetItemUIOn(ResourceManager.Instance.F_GetInventorySprite(_scrapNum), _scrapName));
            _hitScrap.F_GetScrap();
            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }
    /// <summary> 스토리지 상호작용 함수 </summary>
    private void F_FurnitureIntercation()
    {
        UIManager.Instance.F_IntercationPopup(true, "[E]");

        if (Input.GetKeyDown(KeyCode.E))
        {
            _hitInfo.transform.GetComponent<Furniture>().F_Interaction();

            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }
    #endregion
}
