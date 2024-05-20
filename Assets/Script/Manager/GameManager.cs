using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Transform _playerTransform;

    private bool _onDrag;   // 현재 드래그 상태인지 확인하는 변수
    public bool onDrag { get => _onDrag; set => _onDrag = value; }

    private bool _onMap;    // 현재 맵(내부/외부)에 입장한 상태인지 확인하는 변수
    public bool onMap { get => _onMap; set => _onMap = value; }
    protected override void InitManager()
    {
        F_SetCursor(false);
        _onDrag = false;
        _onMap = false;
        _playerTransform = PlayerManager.Instance.playerTransform;
    }

    /// <summary> 매개변수 false : 커서끄기+고정 / true : 커서켜기+고정해제 </summary>
    public void F_SetCursor(bool v_mode)
    {
        Cursor.visible = v_mode;

        if(v_mode)
            Cursor.lockState = CursorLockMode.None;     // 코서 고정 해제
        else
            Cursor.lockState = CursorLockMode.Locked;   // 커서 고정
    }

    #region base 64
    // base 64 인코딩
    public string F_EncodeBase64(string v_source)
    {
        System.Text.Encoding encoding = new System.Text.UTF8Encoding();
        byte[] bytes = encoding.GetBytes(v_source);
        return System.Convert.ToBase64String(bytes);
    }

    // base64 디코딩
    public string F_DecodeBase64(string v_base64)
    {
        System.Text.Encoding encoding = new System.Text.UTF8Encoding();
        byte[] bytes = System.Convert.FromBase64String(v_base64);
        return encoding.GetString(bytes);
    }
    #endregion

    private void Update()
    {
        if (_playerTransform.position.y <= -500)
        {
            // 플레이어 위치 및 가속도 초기화
            _playerTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _playerTransform.position = new Vector3(0, 5, 0);

            // 0 0 0 위치에 블럭이 없으면 계속 떨어지는 현상 고쳐야함..
            // 0 0 0 위치의 블럭은 부숴지지않게끔 수정하거나, 블럭이 존재하는곳 위로 가도록 구현해야할듯

            // 모든 오브젝트가 부숴졌을때 ( 발판 벽 등등 ) 게임을 초기화시키거나 , 마지막 저장위치로 옮기는 기능 추가해야할듯
        }
    }

    public void QualityTEST()
    {
        // QualitySettings.names  --> 퀄리티 세팅명 배열
        // QualitySettings.GetQualityLevel()    --> 현재 퀄리티 가져옴
        // QualitySettings.SetQualityLevel();    --> 퀼리티 변경 ( names의 index를 매개변수로 )
    }
}
