using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBConnector : Singleton<DBConnector>
{
    private string _ip = "127.0.0.1";           // DB 서버 주소
    private int _port = 3306;                   // DB 서버 포트
    private string _id = "root";                // DB 서버 아이디
    private string _pw = "0000";                // DB 서버 비밀번호
    private string _database = "spacemayhem";          // DB 서버 데이터베이스

    private MySqlConnection _connection = null;
    private MySqlConnection connection
    {
        get
        {
            if (_connection == null)
            {
                try
                {
                    string formatSql = string.Format(
                        "Server={0};Database={1};Uid={2};Pwd={3}"
                        , _ip, _database, _id, _pw);
                    _connection = new MySqlConnection(formatSql);
                }
                catch (MySqlException e)
                {
                    Debug.LogError(e);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

            return _connection;

        }
    }

    protected override void InitManager()
    {
        Debug.Log("connention : " + F_ConnectionTest());

        //Debug.Log("Insert Test : " + F_InsertAccount("account","kaffu123","012asdf34"));
        //Debug.Log("Select Test : " + F_SearchAccountID("account", "kaffu1231") + " -> 아이디 존재여부!");
    }

    private bool F_ConnectionTest()
    {
        try
        {
            connection.Open();
            Debug.Log("DB Connection State : " + connection.State);
            connection.Close();
            return true;
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return false;
        }
    }

    /// <summary>
    /// 플레이어 계정 데이터 추가 ( 회원가입 )
    /// </summary>
    /// <param name="v_table"> Insert 테이블 명</param>
    /// <param name="v_id"> Insert 데이터 : ID</param>
    /// <param name="v_pw"> Insert 데이터 : PW</param>
    /// <returns> insert 성공 여부를 리턴합니다. </returns>
    public bool F_InsertAccount(string v_table, string v_id, string v_pw)
    {
        string query = string.Empty;
        try
        {
            query = string.Format("INSERT INTO {0}(ID,PW) VALUES('{1}','{2}')"
                , v_table, v_id, v_pw);
            connection.Open();

            MySqlCommand queryCommand = new MySqlCommand(query,connection); 

            queryCommand.ExecuteNonQuery();
            connection.Close();

            return true;
        }
        catch (Exception ex)
        {
            connection.Close();
            Debug.LogError(ex);
            return false;
        }
    }
    /// <summary>
    /// 검색한 아이디가 존재하는지 확인 
    /// </summary>
    /// <param name="v_table">Select Table</param>
    /// <param name="v_id">Select 조건</param>
    /// <returns> 검색한 아이디가 존재여부 반환 </returns>
    public bool F_SearchAccountID(string v_table, string v_id)
    {
        string query = string.Empty;
        try
        {
            query = string.Format("SELECT * FROM {0} WHERE ID = '{1}'"
                , v_table, v_id);

            connection.Open();

            MySqlCommand queryCommand = new MySqlCommand(query, connection);

            MySqlDataReader reader = queryCommand.ExecuteReader();

            // 읽을게 있음 => 존재함
            while (reader.Read())
            {
                Debug.Log(reader["UID"] + " / " + reader["ID"] + " / "+ reader["PW"]);
                connection.Close();
                return true;
            }

            reader.Close();
            Debug.Log("없음!");

            connection.Close();
            return false;
        }
        catch (Exception ex)
        {
            connection.Close();
            Debug.LogError(ex);
            return true; 
        }
    }
}
