using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Transactions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player_Controller : MonoBehaviour
{
    [Header("== Player Move ==")]
    [SerializeField] private Animator _player_ArmAniTest;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CapsuleCollider _cd;

    //0 : �ȴ� �ӵ�, 1 : �ٴ� �ӵ�, 2 : ������ �ȴ� �ӵ�, 3: ������ �ٴ� �ӵ�
    private float[] _speed_Array;
    private float _moveSpeed;
    private float _jumpSpeed = 5f;
    private bool _isGrounded = true;
    private bool _isCrouched = false;
    private bool _isOnLadder = false;

    [Header("== Camera Move ==")]
    [SerializeField] private Camera _main_Camera;
    [SerializeField] private float _mouseSensitivity = 500f; 
    private float _cameraPosY;                      //���� ī�޶� ������ y��
    private float _camera_Crouched_PosY;            //�ɾ��� �� ī�޶� ������ y��
    private Vector3 _rotationX;
    private float _rotationY;

    [Header("== ��ȣ�ۿ� ==")]
    [SerializeField] private LayerMask _item_LayerMask;
    private RaycastHit _hitInfo;
    private float _item_CanGetRange = 5f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _cd = GetComponent<CapsuleCollider>();

        _cameraPosY = _main_Camera.transform.localPosition.y;
        _speed_Array = new float[4];
        _speed_Array[0] = 4f; 
        _speed_Array[1] = 8f; 
        _speed_Array[2] = 2f;
        _speed_Array[3] = 3.5f;
    }

    void Update()
    {
        // Ŀ���� ������������ �����ϼ��ֵ��� �ϱ�
        if(!UnityEngine.Cursor.visible)
        {
            F_PlayerCrouch();
            F_PlayerRun();
            F_PlayerCameraHorizonMove();
            F_PlayerCameraVerticalMove();
            F_PlayerCheckScrap();
            F_PlayerMove();
        }
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
                    StartCoroutine(C_PlayerCrouch(true, -0.82f, 0.4f, 1));
                }
            }
            if (_isCrouched)
            {
                _moveSpeed = _speed_Array[2];
                if (Input.GetKeyUp(KeyCode.C))
                {
                    StartCoroutine(C_PlayerCrouch(false, 0f, 0.89f, 1.85f));
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
            _main_Camera.transform.localPosition = new Vector3(0, _cameraPosY, 0);
            if (_isCrouched)
            {
                if (_cameraPosY <= -0.821f)
                {
                    _cameraPosY = -0.82f;
                    break;
                }
            } 
            else
            {
                if (_cameraPosY >= -0.001f)
                {
                    _cameraPosY = 0f;
                    break;
                }
            }
            yield return null;
        }
    }

    private void F_PlayerMove()
    {
        float _input_x = Input.GetAxis("Horizontal");
        float _input_z = Input.GetAxis("Vertical");
        Vector3 _moveVector;
        if (!_isOnLadder)
        {
        _moveVector = (transform.right * _input_x + transform.forward * _input_z).normalized;
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);
        _rb.MovePosition(transform.position + _moveVector * _moveSpeed * Time.deltaTime);
            //�����̽��� ������ ����
            if (_isGrounded)
            {
                _player_ArmAniTest.SetBool("isGround", true);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    F_PlayerJump();
                }
            }
            else if (!_isGrounded)
            {
                _player_ArmAniTest.SetBool("isGround", false);
            }
        }
        else if (_isOnLadder)
        {
            F_PlayerLadderMove();
        }
        if (_input_x != 0 || _input_z != 0)
            _player_ArmAniTest.SetBool("Walk", true);
        else
            _player_ArmAniTest.SetBool("Walk", false);

    }

    private void F_PlayerJump()
    {
        if (!_isCrouched)
            _rb.AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);
        else
            _rb.AddForce(Vector3.up * _jumpSpeed / 2f, ForceMode.Impulse);
        _player_ArmAniTest.SetTrigger("Jump");
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

        _main_Camera.transform.localEulerAngles = new Vector3(_rotationY, 0, 0);
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
        float _cameraRorationX = _main_Camera.transform.localRotation.x;
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
            _rb.AddForce(-_main_Camera.transform.forward * _jumpSpeed / 4f, ForceMode.Impulse);
            _player_ArmAniTest.SetTrigger("Jump");
        }
    }
    #endregion

    #region ��ȣ�ۿ� ����
    private void F_PlayerCheckScrap()
    {
        if (Physics.Raycast(_main_Camera.transform.position, _main_Camera.transform.forward, out _hitInfo, _item_CanGetRange, _item_LayerMask))
        {
            UIManager.Instance.F_PlayerCheckScrap(true);
            if (Input.GetKeyDown(KeyCode.E))
                _hitInfo.transform.GetComponent<Scrap>().F_GetScrap();
        }
        else
            UIManager.Instance.F_PlayerCheckScrap(false);
    }
    #endregion
}
