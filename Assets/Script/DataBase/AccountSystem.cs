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
    [SerializeField] Button _loginButton;           // 로그인 버튼
    [SerializeField] Button _registerButton;        // 회원가입 버튼
    [SerializeField] Button _onRegister;            // 회원가입 UI ON 버튼
    [SerializeField] Button _offRegister;           // 회원가입 UI OFF 버튼

    [Header("UI")]
    [SerializeField] GameObject _lobbyObjects;      // 로비 오브젝트
    [SerializeField] GameObject _accountUI;         // 로그인/회원가입 최상위 UI
    [SerializeField] GameObject _loginUI;           // 로그인 UI
    [SerializeField] GameObject _registerUI;        // 회원가입 UI

    private void Start()
    {
        // 로그인 버튼 함수 바인딩
        _loginButton.onClick.AddListener(F_Login);

        // 회원가입 버튼 함수 바인딩
        _registerButton.onClick.AddListener(F_Register);

        // 회원가입UI On/OFF 버튼 함수 바인딩
        _onRegister.onClick.AddListener(() => F_OnRegisterUI(true));
        _offRegister.onClick.AddListener(() => F_OnRegisterUI(false));
    }

    private void F_Login()
    {

    }

    private void F_Register()
    {
        string registerID = _inputFieldID_Register.text;
        string registerPW = _inputFieldPW_Register.text;
        string registerConfirm = _inputFieldConfirmPW_Register.text;
        F_InitRegisterInputField();

        Debug.Log(registerID + "/" + registerPW + "/" + registerConfirm);
        Debug.Log(registerID.Length + "/" + registerPW.Length + "/" + registerConfirm.Length);

        if (registerID.Length < 1 || 15 < registerID.Length)
        {
            // 닉네임 오류
            Debug.Log("닉네임이 길이가 짧거나 김");
            return;
        }

        if(registerPW.Length < 4 || 25 < registerPW.Length)
        {
            // 비밀번호 오류
            Debug.Log("비밀번호 길이가 짧거나 김");
            return;
        }

        if(DBConnector.Instance.F_SearchAccountID(_accountTable, registerID))
        {
            // 닉네임 오류
            Debug.Log("아이디가 이미 있음");
            return;
        }

        
        if(registerPW != registerConfirm)
        {
            // 비밀번호 오류
            Debug.Log("비번이 다름");
            return;
        }

        if(AccountManager.Instance.F_RegisterAccount(_accountTable,registerID,registerPW))
        {
            // 회원가입 성공 -> UI OFF
            F_OnRegisterUI(false);  
            return;
        }
    }


    // 로그인 성공
    public void F_SuccessLogin()
    {
        _accountUI.SetActive(false);

        _inputFieldID_Login.text = "";
        _inputFieldPW_Login.text = "";
    }

    // 회원가입 UI ON/OFF
    public void F_OnRegisterUI(bool v_state)
    {
        // 초기화
        F_InitRegisterInputField();
        _registerUI.SetActive(v_state);
        _loginUI.SetActive(!v_state);
    }

    // 회원가입 UI 초기화
    private void F_InitRegisterInputField()
    {
        _inputFieldID_Register.text = "";
        _inputFieldPW_Register.text = "";
        _inputFieldConfirmPW_Register.text = "";
    }
}
