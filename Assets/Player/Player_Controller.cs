using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
    
    [SerializeField] private Animator _player_FarmingGun_Ani;
    private CapsuleCollider _cd;
    private Rigidbody _rb;
    private RaycastHit _check_Ray_Scrap;

    public Rigidbody playerRigidbody => _rb;

    //0 : 걷는 속도, 1 : 뛰는 속도, 2 : 앉으며 걷는 속도, 3: 앉으며 뛰는 속도
    private float[] _speed_Array;
    private float _moveSpeed;
    private float _jumpSpeed = 5f;
    private bool _isCrouched = false;
    [SerializeField] private bool _isGrounded = true;
    [SerializeField] private bool _isOnLadder = false;
    public bool _isPlayerDead = false;

    [Header("== Camera Move ==")]
    [SerializeField] private Camera _player_Camera;
    [SerializeField] private float _mouseSensitivity = 500f;
    public float mouseSensitivity { get => _mouseSensitivity; set => _mouseSensitivity = value; }
    private float _cameraPosY;                      //현재 카메라 포지션 y축
    private float _camera_Crouched_PosY;            //앉았을 때 카메라 포지션 y축
    private Vector3 _rotationX;
    private float _rotationY;

    [Header("== 상호작용 LayerMask ==")]
    [SerializeField] private LayerMask _item_LayerMask;
    [SerializeField] private LayerMask _furniture_LayerMask;
    [SerializeField] private LayerMask _teleport_LayerMask;

    private LayerMask combLayerMask => _item_LayerMask | _furniture_LayerMask | _teleport_LayerMask;

    private RaycastHit _hitInfo;
    private float _item_CanGetRange = 5f;

    [Header("=== Pistol ===")]
    [SerializeField] private Pistol _pistol;
    [SerializeField] private GameObject _repair_tool;

    [Header("=== teleport ===")]
    [SerializeField] private TeleportController _teleport;

    [Header("=== Light ===")]
    [SerializeField] private GameObject _DungeonLight;
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
        _moveSpeed = _speed_Array[0];

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
                PlayerManager.Instance._canShootPistol = false;
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
                PlayerManager.Instance._canShootPistol = false;
                break;

            case PlayerState.INSTALL:
                F_EmptyHand();
                playerController -= F_FarmingFunction;
                playerController += F_InstallFunction;
                PlayerManager.Instance._canShootPistol = false;
                break;
        }
    }
    #endregion

    /// <summary> 도구 드는 함수임 </summary>
    public void F_EquipTool(int v_toolCode)
    {
        _player_Animation = _player_Arm_Weapon_Ani;

        if (UIManager.Instance.F_GetPlayerFireGauge().fillAmount > 0)
        {
            StartCoroutine(UIManager.Instance.C_FireGaugeFadeOut());
        }

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

        if (UIManager.Instance.F_GetPlayerFireGauge().fillAmount > 0)
        {
            StartCoroutine(UIManager.Instance.C_FireGaugeFadeOut());
        }
    }
    public void F_FarmingFunction()
    {
        F_PistolFire();
    }

    public void F_InstallFunction()
    {
    }

    //사망 or 기절 시 델리게이트 제거
    public void F_PlayerDead()
    {
        _player_Animation.SetBool("Walk", false);
        GameManager.Instance.F_SetCursor(true);
    }

    #region 플레이어 애니메이션 (모션)
    public void F_CreateMotion()
    {
        _player_Animation.SetTrigger("Create");
    }

    public void F_PickupMotion()
    {
        _player_Animation.SetTrigger("PickUp");
    }
    #endregion

    #region 움직임 관련
    // 달리기 (Shift)
    private void F_PlayerRun()  
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //서있을 때 달리기
            if (!_isCrouched)
            {
                _moveSpeed = _speed_Array[1];
                _player_Animation.SetFloat("WalkSpeed", 2.0f);
            }
            //앉아있을 때 달리기
            else
            {
                _moveSpeed = _speed_Array[3];
                _player_Animation.SetFloat("WalkSpeed", 1.5f);
            }

        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            //앉은 채로 걷기
            if (_isCrouched)
            {
                _moveSpeed = _speed_Array[2];
            }

            //선 채로 걷기
            else 
            {
                _moveSpeed = _speed_Array[0];
                _player_Animation.SetFloat("WalkSpeed", 1.0f);
            }
        }
    }

    private void F_PistolFire()
    {
        if (PlayerManager.Instance._canShootPistol)
        {
            if (Input.GetMouseButton(0))
                _pistol.F_SpearPowerCharge();
            if (Input.GetMouseButtonUp(0))
            {
                _player_Animation.SetTrigger("Fire");
                _player_FarmingGun_Ani.SetTrigger("Fire");
                PlayerManager.Instance._canShootPistol = false;
                _pistol.F_SpearFire();

                SoundManager.Instance.F_PlaySFX(SFXClip.CROSSBOW);   // 발싸 사운드 실행

                // 도구 내구도 줄이기
                int idx = ItemManager.Instance.inventorySystem.selectQuickSlotNumber;               // 선택된 슬롯 ( 도구 )
                (ItemManager.Instance.inventorySystem.inventory[idx] as Tool).F_UseDurability();    // 도구 내구도 사용
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
                if (Input.GetKeyDown(KeyCode.C) && _isGrounded)
                {
                    StartCoroutine(C_PlayerCrouch(true, 1.3f, 0.64f, 1.55f));
                    _moveSpeed = _speed_Array[2];
                }
            }
            if (_isCrouched)
            {
                _player_Animation.SetFloat("WalkSpeed", 0.5f);
                if (Input.GetKeyUp(KeyCode.C))
                {
                    StartCoroutine(C_PlayerCrouch(false, 2f, 1f, 2.3f));
                    _moveSpeed = _speed_Array[0];
                    _player_Animation.SetFloat("WalkSpeed", 1.0f);
                }
            }
    }

    //앉기 코루틴 매개변수 순서 : (앉으면 true, 목표 카메라 Position Y축, CharacterController Y축, CharacterController 키)
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
        if (Physics.Raycast(transform.position, Vector3.down, out _check_Ray_Scrap, 0.7f))
        {
            if (_check_Ray_Scrap.collider.CompareTag("Scrap") || _check_Ray_Scrap.collider.CompareTag("Undefined"))
                _isGrounded = false;
            else
                _isGrounded = true;
        }
        else
            _isGrounded = false;
        
        if (!_isOnLadder)
        {
        _moveVector = (transform.right * _input_x + transform.forward * _input_z).normalized;
        _rb.MovePosition(transform.position + _moveVector * _moveSpeed * Time.deltaTime);
            if (_isGrounded)
                _player_Animation.SetBool("isGround", true);
            else
                _player_Animation.SetBool("isGround", false);
            //스페이스바 누르면 점프
            if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
                    F_PlayerJump();
        }
        else if (_isOnLadder)
        {
            F_PlayerLadderMove();
            _input_x = 0;
        }
        if (_input_x != 0 || _input_z != 0)
        {
            _player_Animation.SetBool("Walk", true);
        }
        else if (_isOnLadder && _input_x == 0)
        {
            _player_Animation.SetBool("Walk", false);
        }
        else
            _player_Animation.SetBool("Walk", false);

    }

    private void F_PlayerJump()
    {
        if (_isGrounded )
        {
            if (!_isCrouched)
                _rb.AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);
            else
                _rb.AddForce(Vector3.up * _jumpSpeed / 2f, ForceMode.Impulse);
            _player_Animation.SetTrigger("Jump");
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder") && !_isOnLadder)
        {
            _isOnLadder = true;
            _rb.useGravity = false;
            _rb.velocity = Vector3.zero;
            float _yTrans = transform.position.y + 0.2f;
            transform.position = new Vector3(transform.position.x, _yTrans, transform.position.z);
        }

        else if (other.CompareTag("Undefined") && _isOnLadder)
        {
            _isOnLadder = false;
            _rb.useGravity = true;
            _rb.velocity = Vector3.zero;
        }
    }
   
    private void F_PlayerLadderMove()
    {
        float _input_z = Input.GetAxisRaw("Vertical");
        Vector3 _moveOnLadder = (transform.up * _input_z).normalized;
        _rb.MovePosition(transform.position + _moveOnLadder * _moveSpeed * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rb.useGravity = true;
            _rb.AddForce(-_player_Camera.transform.forward * _jumpSpeed / 4f, ForceMode.Impulse);
            _isOnLadder = false;
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
            // 쓰레기 ( 파밍템 ) 상호작용
            if (_hitInfo.collider.CompareTag("Scrap"))
                F_ScrapInteraction();

            // 상호작용 O, 회수 O ( 설치물 )
            else if (_hitInfo.collider.CompareTag("InteractionObject"))
                F_FurnitureIntercation();

            // 상호작용 X , 회수 O, 뒤에 or문은 사다리  ( 설치물 )
            else if (_hitInfo.collider.CompareTag("unInteractionObject") || _hitInfo.collider.transform.parent.CompareTag("unInteractionObject"))
                F_ReturnFurniture();

            // 운석 상호작용
            else if (_hitInfo.collider.CompareTag("Meteor"))
                F_MeteorInteraction();

            // 우주선 - 외부맵간 텔레포트 상호작용
            else if (_hitInfo.collider.CompareTag("Teleport"))
                F_TeleportInteraction();

            // 외부맵 탈출구 상호작용
            else if (_hitInfo.collider.CompareTag("EnterDungeon"))
                F_EnterDungeonInteraction();

            // 내부맵 탈출구 상호작용
            else if (_hitInfo.collider.CompareTag("ExitDungeon"))
                F_ExitDungeonInteraction();

            // 드랍 아이템 ( 내부 외부맵 아이템 및 레시피)
            else if (_hitInfo.collider.CompareTag("DropItem"))
                F_DropItemInteraction();

            return;
        }

        UIManager.Instance.F_IntercationPopup(false, "");
    }

   /// <summary> 우주쓰레기 상호작용 함수 </summary>
    private void F_ScrapInteraction()
    {
        UIManager.Instance.F_IntercationPopup(true, "GET [E]");

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

            // 애니메이션 + 사운드        
            F_PickupMotion();
            SoundManager.Instance.F_PlaySFX(SFXClip.USEHAND);

            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }

    /// <summary> 설치물(Furniture) 상호작용 함수 </summary>
    private void F_FurnitureIntercation()
    {
        UIManager.Instance.F_IntercationPopup(
            true, ResourceManager.Instance.F_GetIntercationTEXT(_hitInfo.collider.gameObject.name));

        if (Input.GetKeyDown(KeyCode.E))
        {
            _hitInfo.transform.GetComponent<Furniture>().F_Interaction();
            UIManager.Instance.F_IntercationPopup(false, "");
        }

        // TODO:설치된 아이템 회수 기능 ( 임시키 : H )
        if (Input.GetKeyDown(KeyCode.H))
        {
            _hitInfo.transform.GetComponent<Furniture>().F_TakeFurniture();
            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }

    /// <summary> 설치물(Furniture) 상호작용 함수 ( 회수 )</summary>
    private void F_ReturnFurniture()
    {
        UIManager.Instance.F_IntercationPopup(true, "Return [H]");
        // TODO:설치된 아이템 회수 기능 ( 임시키 : H )
        if (Input.GetKeyDown(KeyCode.H))
        {
            _hitInfo.transform.GetComponent<Furniture>().F_TakeFurniture();
            if (_isOnLadder)
            { 
                _isOnLadder = false;
                _rb.useGravity = true;
                _rb.velocity = Vector3.zero;
            }
            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }
    
    /// <summary> 운석 상호작용 함수 </summary>
    private void F_MeteorInteraction()
    {
        UIManager.Instance.F_IntercationPopup(true, "GET [E]");

        if (Input.GetKeyDown(KeyCode.E))
        {
            Meteor _hitedMeteor = _hitInfo.transform.GetComponent<Meteor>();
            int _meteorDropItemCode = _hitedMeteor.F_SettingItemCode();
            string _meteorDropItemName = ItemManager.Instance.ItemDatas[_meteorDropItemCode]._itemName;

            StartCoroutine(UIManager.Instance.C_GetItemUIOn(ResourceManager.Instance.F_GetInventorySprite(_meteorDropItemCode), _meteorDropItemName));
            _hitedMeteor.F_GetMeteor(_meteorDropItemCode);

            // 애니메이션 + 사운드        
            F_PickupMotion();       
            SoundManager.Instance.F_PlaySFX(SFXClip.USEHAND);

            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }

    private void F_TeleportInteraction()
    {
        UIManager.Instance.F_IntercationPopup(true, "Teleport [E]");
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(_teleport.F_TeleportPlayer());
            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }

    private void F_EnterDungeonInteraction()
    {
        UIManager.Instance.F_IntercationPopup(true, "Enter Dungeon [E]");
        if (Input.GetKeyDown(KeyCode.E))
        {
            InsideMapManager.Instance.mapLight.SetActive(false);
            StartCoroutine(F_TeleportPlayer(InsideMapManager.Instance._startRoom.transform.position));
            UIManager.Instance.F_IntercationPopup(false, "");
            _DungeonLight.SetActive(true);
        }
    }

    private void F_ExitDungeonInteraction()
    {
        UIManager.Instance.F_IntercationPopup(true, "Exit Dungeon [E]");
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.F_ClearStoryDungeon();

            InsideMapManager.Instance.mapLight.SetActive(true);
            StartCoroutine(F_TeleportPlayer(OutsideMapManager.Instance.playerTeleportPosition));
            UIManager.Instance.F_IntercationPopup(false, "");
            _DungeonLight.SetActive(false);
        }
    }

    private void F_DropItemInteraction()
    {
        UIManager.Instance.F_IntercationPopup(true, "GET [E]");

        if (Input.GetKeyDown(KeyCode.E))
        {
            // 아이템/레시피 획득
            _hitInfo.collider.GetComponent<DropObject>().F_GetObject();

            // 애니메이션 + 사운드        
            F_PickupMotion();
            SoundManager.Instance.F_PlaySFX(SFXClip.USEHAND);

            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }

    IEnumerator F_TeleportPlayer(Vector3 target)
    {
        F_OnKinematic(true);                            // 물리 충돌 X
        UIManager.Instance.F_OnLoading(true);           // 로딩 ON
        yield return new WaitForSeconds(0.5f);          // 0.5초 대기

        transform.position = target;                    // Target으로 이동
        yield return new WaitForSeconds(0.5f);          // 0.5초 대기

        F_OnKinematic(false);                            // 물리 충돌 X
        yield return new WaitForSeconds(0.1f);          // 0.5초 대기
        UIManager.Instance.F_OnLoading(false);           // 로딩 ON
    }
    #endregion

    public void F_OnKinematic(bool v_bValue)
    {
        _rb.isKinematic = v_bValue;
    }
}
