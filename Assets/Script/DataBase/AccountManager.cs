using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountManager : Singleton<AccountManager>
{
    [SerializeField] private string _playerID = string.Empty;
    [SerializeField] private string _playerPW = string.Empty;
    [SerializeField] private int _playerUID = -1;

    public bool isLocal => _playerUID == -1;
    public int uid => _playerUID;

    protected override void InitManager()
    {
   
    }

    public bool F_LoginAccount(string v_table, string v_id, string v_pw)
    {
        string query = string.Format("SELECT * FROM {0} WHERE ID = '{1}'"
            , v_table,v_id);

        DataSet data = DBConnector.Instance.F_Select(query, v_table);

        if (data == null)
            return false;

        foreach(DataRow row in data.Tables[0].Rows)
        {
            string uid = row["UID"].ToString();
            string id = row["ID"].ToString();
            string pw = row["PW"].ToString();

            if(id == v_id && pw == v_pw)
            {
                _playerUID = int.Parse(uid);
                _playerID = id;
                _playerPW = pw;

                Debug.Log("로그인 성공!");
                return true;
            }
        }

        return false;
    }

    public bool F_RegisterAccount(string v_table, string v_id, string v_pw)
    {
        string query = string.Format("INSERT INTO {0}(ID,PW) VALUES('{1}','{2}')"
                , v_table, v_id, v_pw);
        if (DBConnector.Instance.F_Insert(query))
        {
            Debug.Log("등록 성공 : ID[" + v_id + "] / PW[" + v_pw + "]");
            F_CreateNewGameData(v_table, v_id);
            return true;
        }
        return false;
    }

    public bool F_SearchAccount(string v_table, string v_id)
    {
        string query = string.Format("SELECT * FROM {0} WHERE ID = '{1}'"
                , v_table, v_id);

        DataSet data = DBConnector.Instance.F_Select(query, v_table);
        if(data ==  null)
            return false;

        // 값이 있으면 true 바로 반환됨.
        foreach (DataRow row in data.Tables[0].Rows)            
            return true;

        return false;
    }

    /// <summary>
    ///  새로운 계정의 게임 데이터를 생성.
    /// </summary>
    /// <param name="v_table"></param>
    /// <param name="v_id"></param>
    private void F_CreateNewGameData(string v_table, string v_id)
    {
        string query = string.Format("SELECT * FROM {0} WHERE ID = '{1}'"
        , v_table, v_id);

        DataSet data = DBConnector.Instance.F_Select(query, v_table);
        foreach(DataRow row in data.Tables[0].Rows)
        {
            int uid = int.Parse(row["UID"].ToString());

            query = "INSERT INTO gamedata(UID) VALUES(" + uid + ")";
            if(DBConnector.Instance.F_Insert(query))
            {
                Debug.Log("게임 데이터 생성 성공 UID : " + uid);
            }
            Debug.Log("게임 데이터 생성 실패");
        }
    }
}
