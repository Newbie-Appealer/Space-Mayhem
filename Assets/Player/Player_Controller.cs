using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player_Controller : MonoBehaviour
{
    [Header("== Player Move ==")]
    private CharacterController _chrCtr;
    //0 : �ȴ� �ӵ�, 1 : �ٴ� �ӵ�, 2 : ������ �ȴ� �ӵ�, 3: ������ �ٴ� �ӵ�
    private float[] _speed_Array;
    private float _moveSpeed;
    private float _jumpSpeed = 0.04f;
    private float _gravity = -0.1f;
    private float _velocity_y = 0f;
    private bool _isCrouched = false;

    [Header("== Camera Move ==")]
    [SerializeField] private Camera _main_Camera;
    [SerializeField] private float _mouseSensitivity = 500f; 
    private float _cameraPosY;                    //���� ī�޶� ������ y��
    private float _camera_Crouched_PosY; //�ɾ��� �� ī�޶� ������ y��
    private float _rotationX;
    private float _rotationY;

    [Header("== Item Check ==")]
    [SerializeField] private LayerMask _item_LayerMask;
    [SerializeField] private GameObject _item_GetUI;
    private RaycastHit _hitInfo;
    private float _item_CanGetRange = 5f;

    void Start()
    {
        _chrCtr = GetComponent<CharacterController>();
        _cameraPosY = _main_Camera.transform.position.y;
        _speed_Array = new float[4];
        _speed_Array[0] = 4f; 
        _speed_Array[1] = 8f; 
        _speed_Array[2] = 2f;
        _speed_Array[3] = 3.5f;
    }

    void Update()
    {
        F_PlayerCrouch();
        F_PlayerRun();
        F_PlayerCameraMove();
        F_PlayerCheckItem();
        F_PlayerMove();
    }

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
                    StartCoroutine(C_PlayerCrouch(false, 1.65f, 0.8f, 2));
                }
            }
    }

    //�ɱ� �ڷ�ƾ 
    //�Ű����� ���� : (������ true, ��ǥ ī�޶� Position Y��, CharacterController Y��, CharacterController Ű)
    private IEnumerator C_PlayerCrouch(bool v_isCrouched, float v_cameraPosY, float v_chrCenter, float v_chrHeight)
    {
        _isCrouched = v_isCrouched;
        _camera_Crouched_PosY = v_cameraPosY;
        _chrCtr.center  = new Vector3(0, v_chrCenter ,0);
        _chrCtr.height = v_chrHeight;
        while(_cameraPosY != _camera_Crouched_PosY)
        {
            _cameraPosY = Mathf.Lerp(_cameraPosY, _camera_Crouched_PosY, 0.1f);
            _main_Camera.transform.localPosition = new Vector3(0, _cameraPosY, 0);
            if (_isCrouched)
            {
                if (_cameraPosY <= 0.8205f) break;
            } 
            else
            {
                if (_cameraPosY >= 1.6495f) break;
            }
            yield return null;
        }
    }

    private void F_PlayerMove()
    {
        float _jnput_x = Input.GetAxis("Horizontal");
        float _jnput_z = Input.GetAxis("Vertical");

        Vector3 _moveVector = new Vector3(_jnput_x, 0, _jnput_z).normalized * _moveSpeed * Time.deltaTime;
        _moveVector = _main_Camera.transform.TransformDirection(_moveVector);

        //�����̽��� ������ ����
        if(_chrCtr.isGrounded)
        {
            _velocity_y = 0f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!_isCrouched)
                    _velocity_y = _jumpSpeed;
                else
                    _velocity_y = 0.03f;
            }
        }

        _velocity_y += _gravity * Time.deltaTime;
        _moveVector.y = _velocity_y;
        _chrCtr.Move(_moveVector);
    }

    private void F_PlayerCameraMove()
    {
        //Rotation�� x�� : ��/��, y�� : ��/��
        //Mouse Y : ��/�� ������, Mouse X : ��/�� ������.
        float _mouseX = Input.GetAxisRaw("Mouse Y");
        float _mouseY = Input.GetAxisRaw("Mouse X");
        _rotationY += _mouseY * _mouseSensitivity * Time.deltaTime;
        _rotationX -= _mouseX * _mouseSensitivity * Time.deltaTime;

        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

        transform.eulerAngles = new Vector3(_rotationX, _rotationY, 0);
    }

    private void F_PlayerCheckItem()
    {
        if (Physics.Raycast(_main_Camera.transform.position, transform.forward, out _hitInfo, _item_CanGetRange, _item_LayerMask))
        {
            _item_GetUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                F_PlayerGetItem();
        }
        else 
            _item_GetUI.SetActive(false);
    }

    private void F_PlayerGetItem()
    {
        // ���߿� F_GetScrap() �Լ� �߰� ����
        Debug.Log("������ ȹ��");
    }

    private void F_PlayerMouseClick()
    {
      
    }
}
