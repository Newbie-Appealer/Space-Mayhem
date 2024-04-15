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
    /// <summary> 매개변수 false : 커서끄기+고정 / true : 커서켜기+고정해제 </summary>
    public void F_SetCursor(bool v_mode)
    {
        Cursor.visible = v_mode;

        if(v_mode)
            Cursor.lockState = CursorLockMode.None;     // 코서 고정 해제
        else
            Cursor.lockState = CursorLockMode.Locked;   // 커서 고정
    }
    // TODO:ESC 눌렀을때 고장나는 현상 고쳐야함!


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
}
