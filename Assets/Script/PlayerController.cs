using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�÷��̾� ������")]
    private Vector3 _moveVec;
    private float _xinput;
    private float _yinput;

    [Header("ī�޶� ������")]
    [SerializeField]
    GameObject _camera;
    public float _mouseSensitivity = 800f; //���콺����
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
        Cursor.lockState = CursorLockMode.Locked;        // Ŀ���� 'ȭ�� ���߾�'�� ������Ŵ
        Cursor.visible = false;                          // Ŀ�� �� ���̰�

        _mouseX += Input.GetAxisRaw("Mouse X") * _mouseSensitivity * Time.deltaTime;
        _mouseY -= Input.GetAxisRaw("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        // ���� ȸ�� ������ �����Ͽ� �ʹ� �ڷ� ȸ������ �ʵ��� �մϴ�.
        _mouseY = Mathf.Clamp(_mouseY, -90f, 90f);

        // ī�޶��� ���� ȸ���� �����մϴ�.
        _camera.transform.localRotation = Quaternion.Euler(_mouseY, _mouseX, 0f);
    }


}
