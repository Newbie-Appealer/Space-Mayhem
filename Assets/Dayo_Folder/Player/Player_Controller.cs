using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("Player Move")]
    private CharacterController _chrCtr;
    private float _moveSpeed = 3f;
    private float _jumpSpeed = 0.05f;
    private float _gravity = -0.1f;
    private float _velocity_y = 0f;

    [Header("Camera Move")]
    [SerializeField] private Camera _main_Camera;
    //마우스 민감도
    [SerializeField]private float _mouseSensitivity = 500f; 
    private float _rotationX;
    private float _rotationY;
    void Start()
    {
        _chrCtr = GetComponent<CharacterController>();
    }

    void Update()
    {
        F_PlayerMove();
        F_PlayerCameraMove();
    }

    private void F_PlayerMove()
    {
        float _jnput_x = Input.GetAxis("Horizontal");
        float _jnput_z = Input.GetAxis("Vertical");

        Vector3 _moveVector = new Vector3(_jnput_x, 0, _jnput_z).normalized * _moveSpeed * Time.deltaTime;
        _moveVector = _main_Camera.transform.TransformDirection(_moveVector);

        if(_chrCtr.isGrounded)
        {
            _velocity_y = 0f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _velocity_y = _jumpSpeed;
            }
        }
        _velocity_y += _gravity * Time.deltaTime;
        _moveVector.y = _velocity_y;
        _chrCtr.Move(_moveVector);
    }

    private void F_PlayerCameraMove()
    {
        //Rotation의 x축 : 상/하, y축 : 좌/우
        //Mouse Y : 좌/우 움직임, Mouse X : 상/하 움직임.
        float _mouseX = Input.GetAxisRaw("Mouse Y");
        float _mouseY = Input.GetAxisRaw("Mouse X");
        _rotationY += _mouseY * _mouseSensitivity * Time.deltaTime;
        _rotationX -= _mouseX * _mouseSensitivity * Time.deltaTime;

        if (_rotationX > 90) _rotationX = 90f;
        if (_rotationX< -90) _rotationX = -90f;

        transform.eulerAngles = new Vector3(_rotationX, _rotationY, 0);
    }
}
