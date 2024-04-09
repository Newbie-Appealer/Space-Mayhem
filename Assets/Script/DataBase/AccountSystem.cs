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
    [SerializeField] Button _loginButton;           // �α��� ��ư
    [SerializeField] Button _registerButton;        // ȸ������ ��ư
    [SerializeField] Button _onRegister;            // ȸ������ UI ON ��ư
    [SerializeField] Button _offRegister;           // ȸ������ UI OFF ��ư

    [Header("UI")]
    [SerializeField] GameObject _lobbyObjects;      // �κ� ������Ʈ
    [SerializeField] GameObject _accountUI;         // �α���/ȸ������ �ֻ��� UI
    [SerializeField] GameObject _loginUI;           // �α��� UI
    [SerializeField] GameObject _registerUI;        // ȸ������ UI

    private void Start()
    {
        // �α��� ��ư �Լ� ���ε�
        _loginButton.onClick.AddListener(F_Login);

        // ȸ������ ��ư �Լ� ���ε�
        _registerButton.onClick.AddListener(F_Register);

        // ȸ������UI On/OFF ��ư �Լ� ���ε�
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
            // �г��� ����
            Debug.Log("�г����� ���̰� ª�ų� ��");
            return;
        }

        if(registerPW.Length < 4 || 25 < registerPW.Length)
        {
            // ��й�ȣ ����
            Debug.Log("��й�ȣ ���̰� ª�ų� ��");
            return;
        }

        if(DBConnector.Instance.F_SearchAccountID(_accountTable, registerID))
        {
            // �г��� ����
            Debug.Log("���̵� �̹� ����");
            return;
        }

        
        if(registerPW != registerConfirm)
        {
            // ��й�ȣ ����
            Debug.Log("����� �ٸ�");
            return;
        }

        if(AccountManager.Instance.F_RegisterAccount(_accountTable,registerID,registerPW))
        {
            // ȸ������ ���� -> UI OFF
            F_OnRegisterUI(false);  
            return;
        }
    }


    // �α��� ����
    public void F_SuccessLogin()
    {
        _accountUI.SetActive(false);

        _inputFieldID_Login.text = "";
        _inputFieldPW_Login.text = "";
    }

    // ȸ������ UI ON/OFF
    public void F_OnRegisterUI(bool v_state)
    {
        // �ʱ�ȭ
        F_InitRegisterInputField();
        _registerUI.SetActive(v_state);
        _loginUI.SetActive(!v_state);
    }

    // ȸ������ UI �ʱ�ȭ
    private void F_InitRegisterInputField()
    {
        _inputFieldID_Register.text = "";
        _inputFieldPW_Register.text = "";
        _inputFieldConfirmPW_Register.text = "";
    }
}
