using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PopupMode
{
    NONE,
    NewGame,
    Continue,
    Exit
}

public class ClickManager : MonoBehaviour
{
    #region Game Data Paths
    // Local Data
    private string _savePath => Application.persistentDataPath + "/saves/";      // ���̺� ���� ���� �ӽ� ����
    private string _inventorySaveFileName = "inventoryData";
    private string _buildSaveFileName = "buildingData";
    private string _furnitureSaveFileName = "furnitureData";
    private string _playerSaveFileName = "playerData";

    private string _dataTableName = "gamedata";
    #endregion

    [Header("Account UI")]
    [SerializeField] private GameObject _accountUI;

    [Header("Popup UI")]
    [SerializeField] private GameObject _popup;
    [SerializeField] private TextMeshProUGUI _popupTEXT;
    [SerializeField] private Button _popupButton_YES;
    [SerializeField] private Button _popupButton_NO;
    [SerializeField] private PopupMode _currentpopupMode;

    [Header("UI")]
    [SerializeField] private GameObject _loadingUI;

    [Header("AudioSource")]
    [SerializeField] private AudioSource _clickAudioSource;

    Ray ray;
    RaycastHit hit;
    private void Awake()
    {
        _popupButton_YES.onClick.AddListener(F_PopupButtonYES);
        _popupButton_NO.onClick.AddListener(F_PopupButtonNO);
    }
    private void Update()
    {
        // �α���â�� ���������� raycast ����
        if (_accountUI.activeSelf)
            return;

        // ���� �˾��� ���°� NONE�϶� ( �˾� OFF )
        if(_currentpopupMode == PopupMode.NONE)
            F_LobbyRaycast();
    }

    private void F_LobbyRaycast()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            
            if (Physics.Raycast(ray, out hit))
            {
                
                if (hit.transform.gameObject.name == "NewGame")
                    F_NewGame();
                
                if (hit.transform.gameObject.name == "Continue")
                    F_Continue();

                if (hit.transform.gameObject.name == "Exit")
                    F_Exit();
            }
        }
    }
    private void F_NewGame()
    {
        _clickAudioSource.Play();

        // 1. ���ӵ����Ͱ� �����ϴ��� Ȯ�� ( ���� : PlayeData )
        if (F_CheckPlayerData())
        {
            // 2. ���ӵ����Ͱ� �ִٸ� �˾� 
            F_OnPopup(PopupMode.NewGame);
        }
        else
        {
            // 3. ���ӵ����Ͱ� ���ٸ� �ٷ� ����  ( Main�� �Ѿ�� ������ ���� )
            StartCoroutine(F_ChangeScene());    // �ε� UI �� ���� ���� ( �� ����
        }
    }
    private void F_Continue()
    {
        _clickAudioSource.Play();

        // 1. ���ӵ����Ͱ� �����ϴ��� Ȯ�� ( ���� : PlayeData )
        if (F_CheckPlayerData())
        {
            // 2. ���ӵ����Ͱ� �ִٸ� �ٷ� ����
            StartCoroutine(F_ChangeScene());    // �ε� UI �� ���� ���� ( �� ����
        }
        else
        {
            // 3. ���ӵ����Ͱ� ���ٸ� �˾� 
            F_OnPopup(PopupMode.Continue);
        }
    }
    private void F_Exit()
    {
        _clickAudioSource.Play();

        // 1. ExitPopup ���
        F_OnPopup(PopupMode.Exit);
    }


    private void F_OnPopup(PopupMode v_mode)
    {
        // 1. �˾� ON
        _popup.SetActive(true);

        // 2. ���� �˾����¸� ������Ʈ
        _currentpopupMode = v_mode;
        switch(v_mode)
        {
            case PopupMode.NewGame:
                _popupTEXT.text = "���ӵ����Ͱ� �����մϴ�. \r\n���� �ʱ�ȭ?";               // ���� ������ ����
                break;

            case PopupMode.Continue:
                _popupTEXT.text = "���ӵ����Ͱ� �����ϴ�. \r\n�� ���� ����?";       // ������ ���� ������?
                break;

            case PopupMode.Exit:    
                _popupTEXT.text = "���� ����?";                     // ���� ���� ?
                break;
        }
    }
    private void F_PopupButtonYES()
    {
        _clickAudioSource.Play();
        switch (_currentpopupMode)
        {
            case PopupMode.NewGame:
                F_ResetPlayerData();                // ���̺� ���� �ʱ�ȭ
                StartCoroutine(F_ChangeScene());    // �ε� UI �� ���� ���� ( �� ����
                break;

            case PopupMode.Continue:
                StartCoroutine(F_ChangeScene());    // �ε� UI �� ���� ���� ( �� ����
                break;

            case PopupMode.Exit:
                // ���� ����
                Application.Quit();
                break;
        }
    }
    private void F_PopupButtonNO()
    {
        _clickAudioSource.Play();
        _currentpopupMode = PopupMode.NONE;
        _popup.SetActive(false);
    }

    /// <summary>
    /// �����Ͱ� ������ true / �����Ͱ� ������ false
    /// </summary>
    private bool F_CheckPlayerData()
    {
        int uid = AccountManager.Instance.uid;

        // Guest ( Local )
        if(uid == -1)
        {
            string saveFilePath = _savePath + _playerSaveFileName + ".json"; // ���̺� ���� ��ġ

            // 1. �÷��̾� ���̺� �����Ͱ� ������ false
            if (!File.Exists(saveFilePath))
            {
                return false;
            }

            // 2. �÷��̾� ���̺� �����Ͱ� ������ true
            return true;
        }

        // Login ( DB )
        else
        {
            string saveFile;

            // ������
            string query = string.Format("SELECT InventoryData From {0} where uid = {1}",
                _dataTableName, uid);

            DataSet data = DBConnector.Instance.F_Select(query, _dataTableName);

            foreach (DataRow row in data.Tables[0].Rows)
            {
                saveFile = row["inventoryData"].ToString();

                // ���̺� ������ ������ false
                if (saveFile == "NONE")
                {
                    return false;
                }

                // ���̺� ������ ������ true
                return true;
            }

            // DB�� ��������ü�� ������ false
            return false;
        }
    }

    /// <summary>
    /// ���̺� ���� �ʱ�ȭ
    /// </summary>
    private void F_ResetPlayerData()
    {
        int uid = AccountManager.Instance.uid;

        // LOCAL
        if (uid == -1)
        {
            // ���̺� ���� ����
            string inventory_saveFilePath = _savePath + _inventorySaveFileName + ".json";
            string build_saveFilePath = _savePath + _buildSaveFileName + ".json";
            string furniture_saveFilePath = _savePath + _furnitureSaveFileName + ".json";
            string player_saveFilePath = _savePath + _playerSaveFileName + ".json";

            File.Delete(inventory_saveFilePath);
            File.Delete(build_saveFilePath);
            File.Delete(furniture_saveFilePath);
            File.Delete(player_saveFilePath);
        }

        // DB
        else
        {
            string query1 = string.Format("UPDATE {0} SET InventoryData = '{1}' WHERE UID = {2}",
                _dataTableName, "NONE", uid);
            DBConnector.Instance.F_Update(query1);

            string query2 = string.Format("UPDATE {0} SET HousingData = '{1}' WHERE UID = {2}",
                _dataTableName, "NONE", uid);
            DBConnector.Instance.F_Update(query2);

            string query3 = string.Format("UPDATE {0} SET FurnitureData = '{1}' WHERE UID = {2}",
                _dataTableName, "NONE", uid);
            DBConnector.Instance.F_Update(query3);

            string query4 = string.Format("UPDATE {0} SET PlayerData = '{1}' WHERE UID = {2}",
                _dataTableName, "NONE", uid);
            DBConnector.Instance.F_Update(query4);
        }
    }

    private IEnumerator F_ChangeScene()
    {
        // �ε� UI ON
        _loadingUI.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        // ���� ����
        SceneManager.LoadScene("01_Main"); 
    }
}
