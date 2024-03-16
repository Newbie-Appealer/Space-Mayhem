using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayer : MonoBehaviour
{
    [Header("플레이어 움직임")]
    private CharacterController _charCont;
    public float moveSpeed = 3f;
    public float jumpSpeed = 1f;
    public float gravity = -1f;
    public float yVelocity = 0f;

    [Header("카메라 움직임")]
    [SerializeField]
    private Transform _cameraTransform;
    [SerializeField]
    GameObject _camera;
    public float _rotationX;
    public float _rotationY;
    public float _mouseSensitivity = 800f;  //마우스감도

    void Start()
    {
        _charCont = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;        // 커서를 '화면 정중앙'에 고정시킴
    }

    void Update()
    {
        F_PlayerMove();
        F_CameraMove();
    }

    private void F_PlayerMove()
    {
        float _xinput = Input.GetAxis("Horizontal");
        //키보드 가로값(좌, 우)을 읽어온 결과를 _xinput에 넘김
        float _yinput = Input.GetAxis("Vertical");
        //키보드 세로값(상, 하)을 읽어온 결과를 _yinput에 넘김

        Vector3 _moveVec = new Vector3(_xinput, 0, _yinput).normalized * Time.deltaTime * moveSpeed;
        //x, y, z 축 값을 Vector3로 만들어 moveVec에 넘김

        _moveVec = _cameraTransform.TransformDirection(_moveVec);
        //카메라 기준 방향으로 움직임

        if (_charCont.isGrounded) //캐릭터가 땅에 있다면
        {
            yVelocity = 0f; //y값은 0
            if (Input.GetKeyDown(KeyCode.Space)) //Space키를 누르면
            {
                yVelocity = jumpSpeed; //y값에 jumpSpeed 대입
            }
        }

        yVelocity += (gravity * Time.deltaTime); //중력값 대입

        _moveVec.y = yVelocity; //_moveVec에 yVelocity값 대입

        _charCont.Move(_moveVec);
        //characterController 움직임에 정한 값(_moveVec)을 넣어줌

    }

    private void F_CameraMove()
    {
        float _mouseX = Input.GetAxisRaw("Mouse X");
        //마우스 X축(가로) 값을 받아 저장
        float _mouseY = Input.GetAxisRaw("Mouse Y");
        //마우스 Y축(세로) 값을 받아 저장

        _rotationX += _mouseY * _mouseSensitivity * Time.deltaTime;
        //가로값 * 마우스 감도를 float값으로 변환
        _rotationY += _mouseX * _mouseSensitivity * Time.deltaTime;
        //세로값 * 마우스 감도를 float값으로 변환

        if (_rotationX > 90)
        {
            _rotationX = 90f;
            //위로 90도 이상 넘어가지 못하게
        }
        if (_rotationX < -90)
        {
            _rotationX = -90f;
            //아래로 90도 이상 넘어가지 못하게
        }
        transform.eulerAngles = new Vector3(-_rotationX, _rotationY, 0);
        //transform.eulerAngles
        //= X축, Y축, Z축 3개의 축을 기준으로 0도~360도 회전하는 Euler좌표각을 사용하기 위한 함수
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
