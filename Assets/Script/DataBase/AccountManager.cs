using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountManager : Singleton<AccountManager>
{
    private string _playerID = string.Empty;
    private string _playerPW = string.Empty;
    private int _playerUID = -1;

    protected override void InitManager()
    {
   
    }

    public bool F_RegisterAccount(string v_table, string v_id, string v_pw)
    {
        if (DBConnector.Instance.F_InsertAccount(v_table, v_id, v_pw))
        {
            Debug.Log("등록 성공 : ID[" + v_id + "] / PW[" + v_pw + "]");
            return true;
        }
        return false;
    }
}
