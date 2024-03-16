using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayer : MonoBehaviour
{
    [Header("�÷��̾� ������")]
    private CharacterController _charCont;
    public float moveSpeed = 3f;
    public float jumpSpeed = 1f;
    public float gravity = -1f;
    public float yVelocity = 0f;

    [Header("ī�޶� ������")]
    [SerializeField]
    private Transform _cameraTransform;
    [SerializeField]
    GameObject _camera;
    public float _rotationX;
    public float _rotationY;
    public float _mouseSensitivity = 800f;  //���콺����

    void Start()
    {
        _charCont = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;        // Ŀ���� 'ȭ�� ���߾�'�� ������Ŵ
    }

    void Update()
    {
        F_PlayerMove();
        F_CameraMove();
    }

    private void F_PlayerMove()
    {
        float _xinput = Input.GetAxis("Horizontal");
        //Ű���� ���ΰ�(��, ��)�� �о�� ����� _xinput�� �ѱ�
        float _yinput = Input.GetAxis("Vertical");
        //Ű���� ���ΰ�(��, ��)�� �о�� ����� _yinput�� �ѱ�

        Vector3 _moveVec = new Vector3(_xinput, 0, _yinput).normalized * Time.deltaTime * moveSpeed;
        //x, y, z �� ���� Vector3�� ����� moveVec�� �ѱ�

        _moveVec = _cameraTransform.TransformDirection(_moveVec);
        //ī�޶� ���� �������� ������

        if (_charCont.isGrounded) //ĳ���Ͱ� ���� �ִٸ�
        {
            yVelocity = 0f; //y���� 0
            if (Input.GetKeyDown(KeyCode.Space)) //SpaceŰ�� ������
            {
                yVelocity = jumpSpeed; //y���� jumpSpeed ����
            }
        }

        yVelocity += (gravity * Time.deltaTime); //�߷°� ����

        _moveVec.y = yVelocity; //_moveVec�� yVelocity�� ����

        _charCont.Move(_moveVec);
        //characterController �����ӿ� ���� ��(_moveVec)�� �־���

    }

    private void F_CameraMove()
    {
        float _mouseX = Input.GetAxisRaw("Mouse X");
        //���콺 X��(����) ���� �޾� ����
        float _mouseY = Input.GetAxisRaw("Mouse Y");
        //���콺 Y��(����) ���� �޾� ����

        _rotationX += _mouseY * _mouseSensitivity * Time.deltaTime;
        //���ΰ� * ���콺 ������ float������ ��ȯ
        _rotationY += _mouseX * _mouseSensitivity * Time.deltaTime;
        //���ΰ� * ���콺 ������ float������ ��ȯ

        if (_rotationX > 90)
        {
            _rotationX = 90f;
            //���� 90�� �̻� �Ѿ�� ���ϰ�
        }
        if (_rotationX < -90)
        {
            _rotationX = -90f;
            //�Ʒ��� 90�� �̻� �Ѿ�� ���ϰ�
        }
        transform.eulerAngles = new Vector3(-_rotationX, _rotationY, 0);
        //transform.eulerAngles
        //= X��, Y��, Z�� 3���� ���� �������� 0��~360�� ȸ���ϴ� Euler��ǥ���� ����ϱ� ���� �Լ�
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
    */

}
