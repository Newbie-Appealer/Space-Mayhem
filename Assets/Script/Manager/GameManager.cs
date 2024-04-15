using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool _onDrag;
    protected override void InitManager()
    {
        F_SetCursor(false);
        _onDrag = false;
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
    // TODO:ESC �������� ���峪�� ���� ���ľ���!


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
}
