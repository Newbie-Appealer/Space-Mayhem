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
}
