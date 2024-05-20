using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Transform _playerTransform;

    private bool _onDrag;   // ���� �巡�� �������� Ȯ���ϴ� ����
    public bool onDrag { get => _onDrag; set => _onDrag = value; }

    private bool _onMap;    // ���� ��(����/�ܺ�)�� ������ �������� Ȯ���ϴ� ����
    public bool onMap { get => _onMap; set => _onMap = value; }
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
            Cursor.lockState = CursorLockMode.None;     // �ڼ� ���� ����
        else
            Cursor.lockState = CursorLockMode.Locked;   // Ŀ�� ����
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

            // 0 0 0 ��ġ�� ���� ������ ��� �������� ���� ���ľ���..
            // 0 0 0 ��ġ�� ���� �ν������ʰԲ� �����ϰų�, ���� �����ϴ°� ���� ������ �����ؾ��ҵ�

            // ��� ������Ʈ�� �ν������� ( ���� �� ��� ) ������ �ʱ�ȭ��Ű�ų� , ������ ������ġ�� �ű�� ��� �߰��ؾ��ҵ�
        }
    }

    public void QualityTEST()
    {
        // QualitySettings.names  --> ����Ƽ ���ø� �迭
        // QualitySettings.GetQualityLevel()    --> ���� ����Ƽ ������
        // QualitySettings.SetQualityLevel();    --> ����Ƽ ���� ( names�� index�� �Ű������� )
    }
}
