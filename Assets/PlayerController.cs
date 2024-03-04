using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 움직임")]
    private Vector3 _moveVec;
    private float _xinput;
    private float _yinput;

    [Header("카메라 움직임")]
    [SerializeField]
    GameObject _camera;
    public float _mouseSensitivity = 800f; //마우스감도
    private float _mouseX;
    private float _mouseY;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        F_PlayerMove();
        F_Camera();
    }

    private void F_PlayerMove()
    {
        _xinput = Input.GetAxis ("Horizontal");
        _yinput = Input.GetAxis("Vertical");

        _moveVec = new Vector3(_xinput, 0, _yinput).normalized;

        transform.position += _moveVec * Time.deltaTime * 3f;
    }

    private void F_Camera()
    {
        Cursor.lockState = CursorLockMode.Locked;        // 커서를 '화면 정중앙'에 고정시킴
        Cursor.visible = false;                          // 커서 안 보이게

        _mouseX += Input.GetAxisRaw("Mouse X") * _mouseSensitivity * Time.deltaTime;
        _mouseY -= Input.GetAxisRaw("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        // 상하 회전 각도를 제한하여 너무 뒤로 회전하지 않도록 합니다.
        _mouseY = Mathf.Clamp(_mouseY, -90f, 90f);

        // 카메라의 로컬 회전을 설정합니다.
        _camera.transform.localRotation = Quaternion.Euler(_mouseY, _mouseX, 0f);
    }


}
