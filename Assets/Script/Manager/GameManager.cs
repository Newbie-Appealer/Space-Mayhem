using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Transform _playerTransform;

    private bool _onDrag;               // 현재 드래그 상태인지 확인하는 변수
    private bool _onMap;                // 현재 맵(내부/외부)에 입장한 상태인지 확인하는 변수
    private int _storyStep;             // 스토리 진행도 ( 난이도 )
    private int _unlockRecipeStep;      // 레시피 해금 진해도

    public bool onDrag { get => _onDrag; set => _onDrag = value; }
    public bool onMap { get => _onMap; set => _onMap = value; }
    public int storyStep { get => _storyStep; set => _storyStep = value; }
    public int unlockRecipeStep { get => _unlockRecipeStep; set => _unlockRecipeStep = value; }

    [Header("journal")]
    [SerializeField] private JournalSystem _journalSystem;
    public JournalSystem journalSystem => _journalSystem;

    [Header("FPS")]
    private float _deltaTime = 0f;
    private Rect _rect;
    private GUIStyle _guiStyle;

    protected override void InitManager()
    {
        F_SetCursor(false);
        _onDrag = false;
        _onMap = false;
        _playerTransform = PlayerManager.Instance.playerTransform;

        //F_InitFpsUI();
        //StartCoroutine(C_RefreshFPS());
    }

    /// <summary> 매개변수 false : 커서끄기+고정 / true : 커서켜기+고정해제 </summary>
    public void F_SetCursor(bool v_mode)
    {
        Cursor.visible = v_mode;

        if(v_mode)
            Cursor.lockState = CursorLockMode.None;     // 컨서 고정 해제
        else
            Cursor.lockState = CursorLockMode.Locked;   // 커서 고정
    }

    public void F_ClearStoryDungeon()
    {
        storyStep++;
        MeteorManager.Instance.F_DifficultyUpdate();
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
    #region FPS
    //private void F_InitFpsUI()
    //{
    //    _guiStyle = new GUIStyle();
    //    _guiStyle.fontSize = 25;
    //    _guiStyle.normal.textColor = Color.white;

    //    _rect = new Rect(10, 10, Screen.width, Screen.height);
    //}

    //private void OnGUI()
    //{
    //    float _fps = 1.0f / _deltaTime;
    //    string _text = string.Format("{0:0.} FPS", _fps);

    //    GUI.Label(_rect, _text, _guiStyle);
    //}
    #endregion
    private void Update()
    {
        if (_playerTransform.position.y <= -500)
        {
            // 플레이어 위치 및 가속도 초기화
            _playerTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _playerTransform.position = new Vector3(0, 5, 0);
        }
    }
}
