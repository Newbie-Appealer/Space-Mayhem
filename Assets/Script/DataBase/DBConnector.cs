using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class DBConnector : Singleton<DBConnector>
{
    private string _ip = "127.0.0.1";           // DB 서버 주소
    //private int _port = 3306;                   // DB 서버 포트
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
        Debug.Log("DB connention : " + F_ConnectionTest());
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

    #region DB 쿼리 사용
    public bool F_Insert(string v_query)
    {
        try
        {
            connection.Open();

            MySqlCommand queryCommand = new MySqlCommand(v_query, connection);

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

    public DataSet F_Select(string v_query, string v_tableName)
    {
        DataSet dataSet= null;
        try
        {
            connection.Open();

            MySqlCommand queryCommand = new MySqlCommand(v_query, connection);
            MySqlDataAdapter data = new MySqlDataAdapter(queryCommand);
            dataSet = new DataSet();
            data.Fill(dataSet, v_tableName);

        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }

        connection.Close();
        return dataSet;
    }

    public bool F_Update(string v_query)
    {
        try
        {
            connection.Open();

            MySqlCommand queryCommand = new MySqlCommand(v_query, connection);

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
    #endregion
}
