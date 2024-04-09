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
    [SerializeField] Button _loginButton;           // 로그인 버튼 ( 서버 저장 )
    [SerializeField] Button _guestLoginButton;      // 게스트 로그인 버튼 ( 로컬 저장 )
    [SerializeField] Button _registerButton;        // 회원가입 버튼
    [SerializeField] Button _onRegister;            // 회원가입 UI ON 버튼
    [SerializeField] Button _offRegister;           // 회원가입 UI OFF 버튼
    [SerializeField] Button _offPopup;              // 팝업 UI OFF버튼

    [Header("UI")]
    [SerializeField] GameObject _lobbyObjects;      // 로비 오브젝트
    [SerializeField] GameObject _accountUI;         // 로그인/회원가입 최상위 UI
    [SerializeField] GameObject _loginUI;           // 로그인 UI
    [SerializeField] GameObject _registerUI;        // 회원가입 UI

    [Header("popup")]
    [SerializeField] GameObject _popupUI;           // 팝업 UI
    [SerializeField] TextMeshProUGUI _popupMessage; // 팝업 UI 메세지

    private void Start()
    {
        // 로그인 버튼 함수 바인딩
        _loginButton.onClick.AddListener(F_Login);
        _guestLoginButton.onClick.AddListener(F_GuestLogin);

        // 회원가입 버튼 함수 바인딩
        _registerButton.onClick.AddListener(F_Register);

        // 회원가입UI On/OFF 버튼 함수 바인딩
        _onRegister.onClick.AddListener(() => F_OnRegisterUI(true));
        _offRegister.onClick.AddListener(() => F_OnRegisterUI(false));

        // 팝업 OFF 버튼
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

    #region 로그인
    private void F_Login()
    {
        string loginID = _inputFieldID_Login.text;
        string loginPW = _inputFieldPW_Login.text;
        F_InitLoginInputField();

        if(AccountManager.Instance.F_LoginAccount(_accountTable,loginID, loginPW))
            F_OnLoginUI(false);                 // 로그인 성공
        else
            F_OnPopup(true, "Login Fail");      // 로그인 실패
    }

    private void F_GuestLogin()
    {
        F_InitLoginInputField();
        F_OnLoginUI(false);
    }
    #endregion

    #region 회원가입
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
            // 회원가입 성공 -> UI OFF
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

    #region UI 관련
    /// <summary>
    /// 로그인 UI  OF/OFF
    /// </summary>
    /// <param name="v_state"> 로그인 성공 여부</param>
    public void F_OnLoginUI(bool v_state)
    {
        _accountUI.SetActive(v_state);

        F_InitLoginInputField();

        _lobbyObjects.SetActive(!v_state);
    }

    /// <summary>
    /// 로그인 UI 입력값 초기화
    /// </summary>
    private void F_InitLoginInputField()
    {
        _inputFieldID_Login.text = "";
        _inputFieldPW_Login.text = "";
    }

    /// <summary>
    /// 회원가입 UI ON/OFF
    /// </summary>
    /// <param name="v_state"> 회원가입 UI 상태</param>
    public void F_OnRegisterUI(bool v_state)
    {
        // 초기화
        F_InitRegisterInputField();

        _registerUI.SetActive(v_state);
        _loginUI.SetActive(!v_state);
    }

    /// <summary>
    /// 회원가입 UI 입력값 초기화
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
