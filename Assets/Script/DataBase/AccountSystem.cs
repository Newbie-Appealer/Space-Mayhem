using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountSystem : MonoBehaviour
{
    private string _accountTable = "account";

    [Header("InputFields")]
    [SerializeField] TMP_InputField _inputFieldID_Login;
    [SerializeField] TMP_InputField _inputFieldPW_Login;

    [SerializeField] TMP_InputField _inputFieldID_Register;
    [SerializeField] TMP_InputField _inputFieldPW_Register;
    [SerializeField] TMP_InputField _inputFieldConfirmPW_Register;

    [Header("Event Buttons")]
    [SerializeField] Button _loginButton;           // �α��� ��ư ( ���� ���� )
    [SerializeField] Button _guestLoginButton;      // �Խ�Ʈ �α��� ��ư ( ���� ���� )
    [SerializeField] Button _registerButton;        // ȸ������ ��ư
    [SerializeField] Button _onRegister;            // ȸ������ UI ON ��ư
    [SerializeField] Button _offRegister;           // ȸ������ UI OFF ��ư
    [SerializeField] Button _offPopup;              // �˾� UI OFF��ư

    [Header("UI")]
    [SerializeField] GameObject _lobbyObjects;      // �κ� ������Ʈ
    [SerializeField] GameObject _accountUI;         // �α���/ȸ������ �ֻ��� UI
    [SerializeField] GameObject _loginUI;           // �α��� UI
    [SerializeField] GameObject _registerUI;        // ȸ������ UI

    [Header("popup")]
    [SerializeField] GameObject _popupUI;           // �˾� UI
    [SerializeField] TextMeshProUGUI _popupMessage; // �˾� UI �޼���

    private void Start()
    {
        // �α��� ��ư �Լ� ���ε�
        _loginButton.onClick.AddListener(F_Login);
        _guestLoginButton.onClick.AddListener(F_GuestLogin);

        // ȸ������ ��ư �Լ� ���ε�
        _registerButton.onClick.AddListener(F_Register);

        // ȸ������UI On/OFF ��ư �Լ� ���ε�
        _onRegister.onClick.AddListener(() => F_OnRegisterUI(true));
        _offRegister.onClick.AddListener(() => F_OnRegisterUI(false));

        // �˾� OFF ��ư
        _offPopup.onClick.AddListener(() => F_OnPopup(false));

        #region DataSet Example
        //string query = string.Format("SELECT * FROM {0}"
        //        , _accountTable);

        //DataSet data = DBConnector.Instance.F_Select(query, _accountTable);

        //foreach(DataColumn c in data.Tables[0].Columns)
        //{
        //    Debug.Log("Column : " + c.ColumnName);
        //}
        //foreach(DataRow d in data.Tables[0].Rows)
        //{
        //    Debug.Log(d["UID"] + " / "+ d["ID"] + " / " + d["PW"]);
        //}
        #endregion
    }

    #region �α���
    private void F_Login()
    {
        string loginID = _inputFieldID_Login.text;
        string loginPW = _inputFieldPW_Login.text;
        F_InitLoginInputField();

        if(AccountManager.Instance.F_LoginAccount(_accountTable,loginID, loginPW))
            F_OnLoginUI(false);                 // �α��� ����
        else
            F_OnPopup(true, "Login Fail");      // �α��� ����
    }

    private void F_GuestLogin()
    {
        F_InitLoginInputField();
        F_OnLoginUI(false);
    }
    #endregion

    #region ȸ������
    private void F_Register()
    {
        string registerID = _inputFieldID_Register.text;
        string registerPW = _inputFieldPW_Register.text;
        string registerConfirm = _inputFieldConfirmPW_Register.text;
        F_InitRegisterInputField();

        if (!F_CheckRegister(registerID, registerPW, registerConfirm))
            return;

        if (AccountManager.Instance.F_RegisterAccount(_accountTable, registerID, registerPW))
        {
            // ȸ������ ���� -> UI OFF
            F_OnRegisterUI(false);
            F_OnPopup(true, "SUCCESS");
        }
    }

    private bool F_CheckRegister(string v_id, string v_pw, string v_comfirm)
    {
        if (v_id.Length < 1 || 15 < v_id.Length)
        {
            if (v_id.Length == 0)
                F_OnPopup(true, "ID Empty");
            else
                F_OnPopup(true, "ID Length Error ( 1 ~ 15 )");
            return false;
        }
        if (v_pw.Length < 4 || 25 < v_pw.Length)
        {
            if(v_pw.Length == 0 || v_pw.Length == 0)
                F_OnPopup(true, "PW Empty");
            else
                F_OnPopup(true, "PW Length Error ( 4 ~ 25 )");
            return false;
        }
        if (v_pw != v_comfirm)
        {
            F_OnPopup(true, "Password Error");
            return false;
        }
        if(AccountManager.Instance.F_SearchAccount(_accountTable,v_id))
        {
            F_OnPopup(true, "ID already exists");
            return false;
        }

        return true;
    }
    #endregion

    #region UI ����
    /// <summary>
    /// �α��� UI  OF/OFF
    /// </summary>
    /// <param name="v_state"> �α��� ���� ����</param>
    public void F_OnLoginUI(bool v_state)
    {
        _accountUI.SetActive(v_state);

        F_InitLoginInputField();

        _lobbyObjects.SetActive(!v_state);
    }

    /// <summary>
    /// �α��� UI �Է°� �ʱ�ȭ
    /// </summary>
    private void F_InitLoginInputField()
    {
        _inputFieldID_Login.text = "";
        _inputFieldPW_Login.text = "";
    }

    /// <summary>
    /// ȸ������ UI ON/OFF
    /// </summary>
    /// <param name="v_state"> ȸ������ UI ����</param>
    public void F_OnRegisterUI(bool v_state)
    {
        // �ʱ�ȭ
        F_InitRegisterInputField();

        _registerUI.SetActive(v_state);
        _loginUI.SetActive(!v_state);
    }

    /// <summary>
    /// ȸ������ UI �Է°� �ʱ�ȭ
    /// </summary>
    private void F_InitRegisterInputField()
    {
        _inputFieldID_Register.text = "";
        _inputFieldPW_Register.text = "";
        _inputFieldConfirmPW_Register.text = "";
    }

    private void F_OnPopup(bool v_state, string v_message = "")
    {
        _popupUI.SetActive(v_state);
        if(v_state)
        {
            _popupMessage.text = v_message;
        }
    }
    #endregion
}
