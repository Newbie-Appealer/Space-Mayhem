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
    
    //�׽�Ʈ��, ���߿� ������ �� ����� �迭�� ������ ����
    [SerializeField] private Animator _player_FarmingGun_Ani;
    private Rigidbody _rb;
    private CapsuleCollider _cd;

    //0 : �ȴ� �ӵ�, 1 : �ٴ� �ӵ�, 2 : ������ �ȴ� �ӵ�, 3: ������ �ٴ� �ӵ�
    private float[] _speed_Array;
    private float _moveSpeed;
    private float _jumpSpeed = 5f;
    private bool _isGrounded = true;
    private bool _isCrouched = false;
    private bool _isOnLadder = false;

    [Header("== Camera Move ==")]
    [SerializeField] private Camera _player_Camera;
    [SerializeField] private float _mouseSensitivity = 500f; 
    private float _cameraPosY;                      //���� ī�޶� ������ y��
    private float _camera_Crouched_PosY;            //�ɾ��� �� ī�޶� ������ y��
    private Vector3 _rotationX;
    private float _rotationY;

    [Header("== ��ȣ�ۿ� ==")]
    [SerializeField] private LayerMask _item_LayerMask;
    [SerializeField] private Pistol _pistol;

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
        // �÷��̾� �⺻ ������ ��������Ʈ �ʱ�ȭ
        playerController = F_PlayerCrouch;
        playerController += F_PlayerRun;
        playerController += F_PlayerCameraHorizonMove;
        playerController += F_PlayerCameraVerticalMove;
        playerController += F_PlayerMove;
        playerController += F_PlayerCheckScrap;
    }

    /// <summary> </summary>
    public void F_ChangeState(PlayerState v_state, int v_uniqueCode)
    {
        switch(v_state)
        {
            case PlayerState.NONE:
                F_EmptyHand();
                playerController -= F_FarmingFunction;
                playerController -= F_BuildigFunction;
                playerController -= F_InstallFunction;
                break;

            case PlayerState.FARMING:
                F_EquipTool(v_uniqueCode);
                playerController += F_FarmingFunction;
                playerController -= F_BuildigFunction;
                playerController -= F_InstallFunction;
                break;
            case PlayerState.BUILDING:
                F_EquipTool(v_uniqueCode);
                playerController -= F_FarmingFunction;
                playerController += F_BuildigFunction;
                playerController -= F_InstallFunction;
                break;
            case PlayerState.INSTALL:
                F_EmptyHand();
                playerController -= F_FarmingFunction;
                playerController -= F_BuildigFunction;
                playerController += F_InstallFunction;
                break;
        }
    }
    #endregion

    /// <summary> ���� ��� �Լ��� </summary>
    public void F_EquipTool(int v_toolCode)
    {
        _player_Animation = _player_Arm_Weapon_Ani;

        _player_Arm.SetActive(false);
        _player_Arm_Weapon.SetActive(true);
    }

    /// <summary> �Ǽ� �� �ൿ �ʱ�ȭ �Լ� </summary>
    public void F_EmptyHand()
    {
        _player_Animation = _player_Arm_Ani;

        _player_Arm.SetActive(true);
        _player_Arm_Weapon.SetActive(false);
    }
    public void F_FarmingFunction()
    {
        if (Input.GetMouseButton(0))
            _pistol.F_SpearPowerCharge();
        if (Input.GetMouseButtonUp(0)) 
        {
            _player_Animation.SetTrigger("Fire");
            _player_FarmingGun_Ani.SetTrigger("Fire");
            _pistol.F_SpearFire();
        }
        if(Input.GetMouseButton(1))
        {
            _pistol.F_SpearComeBack();
        }
    }
    public void F_BuildigFunction()
    {

    }
    public void F_InstallFunction()
    {

    }

    #region ������ ����
    // �޸��� (Shift)
    private void F_PlayerRun()  
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //������ �� �޸���
            if (!_isCrouched) 
                _moveSpeed = _speed_Array[1];

            //�ɾ����� �� �޸���
            else _moveSpeed = _speed_Array[3]; 
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            //���� ä�� �ȱ�
            if (_isCrouched)
                _moveSpeed = _speed_Array[2];

            //�� ä�� �ȱ�
            else _moveSpeed = _speed_Array[0];
        }
    }

    //�ɱ� (C)
    private void F_PlayerCrouch()
    {
            if (!_isCrouched)
            {
                _moveSpeed = _speed_Array[0];
                if (Input.GetKeyDown(KeyCode.C))
                {
                    StartCoroutine(C_PlayerCrouch(true, 0.82f, 0.4f, 1));
                }
            }
            if (_isCrouched)
            {
                _moveSpeed = _speed_Array[2];
                if (Input.GetKeyUp(KeyCode.C))
                {
                    StartCoroutine(C_PlayerCrouch(false, 1.5f, 0.83f, 1.85f));
                }
            }
    }

    //�ɱ� �ڷ�ƾ 
    //�Ű����� ���� : (������ true, ��ǥ ī�޶� Position Y��, CharacterController Y��, CharacterController Ű)
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
                if (_cameraPosY <= 0.82f)
                {
                    _cameraPosY = 0.82f;
                    break;
                }
            } 
            else
            {
                if (_cameraPosY >= 1.499f)
                {
                    _cameraPosY = 1.5f;
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
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);
        _rb.MovePosition(transform.position + _moveVector * _moveSpeed * Time.deltaTime);
            //�����̽��� ������ ����
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
        // Ŀ���� �Ⱥ��϶� !
        float _mouseY = Input.GetAxisRaw("Mouse Y");
        _rotationY -= _mouseY * _mouseSensitivity * Time.deltaTime;

        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);

        _player_Camera.transform.localEulerAngles = new Vector3(_rotationY, 0, 0);
    }
    #endregion

    #region ��ٸ� Ÿ��
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

    #region ��ȣ�ۿ� ����  
    private void F_PlayerCheckScrap()
    {
        if (Physics.Raycast(_player_Camera.transform.position, _player_Camera.transform.forward, out _hitInfo, _item_CanGetRange, _item_LayerMask))
        {
            UIManager.Instance.F_PlayerCheckScrap(true);
            if (Input.GetKeyDown(KeyCode.E))
                F_PlayerGetScrap(_hitInfo);
        }
        else
            UIManager.Instance.F_PlayerCheckScrap(false);
    }

    private void F_PlayerGetScrap(RaycastHit v_hit)
    {
        Scrap _hitScrap = v_hit.transform.GetComponent<Scrap>();
        int _scrapNum = _hitScrap.scrapNumber;
        string _scrapName = ItemManager.Instance.ItemDatas[_scrapNum]._itemName;
        StartCoroutine(UIManager.Instance.C_GetItemUIOn(ResourceManager.Instance.F_GetInventorySprite(_scrapNum), _scrapName));
        v_hit.transform.GetComponent<Scrap>().F_GetScrap();

    }
    #endregion
}
