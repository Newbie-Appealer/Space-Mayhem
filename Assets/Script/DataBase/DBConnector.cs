using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBConnector : Singleton<DBConnector>
{
    private string _ip = "127.0.0.1";           // DB ���� �ּ�
    private int _port = 3306;                   // DB ���� ��Ʈ
    private string _id = "root";                // DB ���� ���̵�
    private string _pw = "0000";                // DB ���� ��й�ȣ
    private string _database = "spacemayhem";          // DB ���� �����ͺ��̽�

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
        //Debug.Log("Select Test : " + F_SearchAccountID("account", "kaffu1231") + " -> ���̵� ���翩��!");
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
    /// �÷��̾� ���� ������ �߰� ( ȸ������ )
    /// </summary>
    /// <param name="v_table"> Insert ���̺� ��</param>
    /// <param name="v_id"> Insert ������ : ID</param>
    /// <param name="v_pw"> Insert ������ : PW</param>
    /// <returns> insert ���� ���θ� �����մϴ�. </returns>
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
    /// �˻��� ���̵� �����ϴ��� Ȯ�� 
    /// </summary>
    /// <param name="v_table">Select Table</param>
    /// <param name="v_id">Select ����</param>
    /// <returns> �˻��� ���̵� ���翩�� ��ȯ </returns>
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

            // ������ ���� => ������
            while (reader.Read())
            {
                Debug.Log(reader["UID"] + " / " + reader["ID"] + " / "+ reader["PW"]);
                connection.Close();
                return true;
            }

            reader.Close();
            Debug.Log("����!");

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
