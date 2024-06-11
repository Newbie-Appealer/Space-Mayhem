using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Transform _playerTransform;

    private bool _onDrag;               // ���� �巡�� �������� Ȯ���ϴ� ����
    private bool _onMap;                // ���� ��(����/�ܺ�)�� ������ �������� Ȯ���ϴ� ����
    private int _storyStep;             // ���丮 ���൵ ( ���̵� )
    private int _unlockRecipeStep;      // ������ �ر� ���ص�

    public bool onDrag { get => _onDrag; set => _onDrag = value; }
    public bool onMap { get => _onMap; set => _onMap = value; }
    public int storyStep { get => _storyStep; set => _storyStep = value; }
    public int unlockRecipeStep { get => _unlockRecipeStep; set => _unlockRecipeStep = value; }

    [Header("journal")]
    [SerializeField] private JournalSystem _journalSystem;
    public JournalSystem journalSystem => _journalSystem;
    
    protected override void InitManager()
    {
        F_SetCursor(false);
        _onDrag = false;
        _onMap = false;
        _playerTransform = PlayerManager.Instance.playerTransform;
    }

    /// <summary> �Ű����� false : Ŀ������+���� / true : Ŀ���ѱ�+�������� </summary>
    public void F_SetCursor(bool v_mode)
    {
        Cursor.visible = v_mode;

        if(v_mode)
            Cursor.lockState = CursorLockMode.None;     // ���� ���� ����
        else
            Cursor.lockState = CursorLockMode.Locked;   // Ŀ�� ����
    }

    public void F_ClearStoryDungeon()
    {
        storyStep++;
        MeteorManager.Instance.F_DifficultyUpdate();
    }

    #region base 64
    // base 64 ���ڵ�
    public string F_EncodeBase64(string v_source)
    {
        System.Text.Encoding encoding = new System.Text.UTF8Encoding();
        byte[] bytes = encoding.GetBytes(v_source);
        return System.Convert.ToBase64String(bytes);
    }

    // base64 ���ڵ�
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
            // �÷��̾� ��ġ �� ���ӵ� �ʱ�ȭ
            _playerTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _playerTransform.position = new Vector3(0, 5, 0);
        }
    }
}
