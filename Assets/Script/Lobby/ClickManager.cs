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
    private string _savePath => Application.persistentDataPath + "/saves/";      // 세이브 파일 저장 임시 폴더
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
        // 로그인창이 켜져있을때 raycast 방지
        if (_accountUI.activeSelf)
            return;

        // 현재 팝업의 상태가 NONE일때 ( 팝업 OFF )
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

        // 1. 게임데이터가 존재하는지 확인 ( 기준 : PlayeData )
        if (F_CheckPlayerData())
        {
            // 2. 게임데이터가 있다면 팝업 
            F_OnPopup(PopupMode.NewGame);
        }
        else
        {
            // 3. 게임데이터가 없다면 바로 시작  ( Main씬 넘어가서 데이터 생성 )
            StartCoroutine(F_ChangeScene());    // 로딩 UI 및 게임 시작 ( 씬 변경
        }
    }
    private void F_Continue()
    {
        _clickAudioSource.Play();

        // 1. 게임데이터가 존재하는지 확인 ( 기준 : PlayeData )
        if (F_CheckPlayerData())
        {
            // 2. 게임데이터가 있다면 바로 시작
            StartCoroutine(F_ChangeScene());    // 로딩 UI 및 게임 시작 ( 씬 변경
        }
        else
        {
            // 3. 게임데이터가 없다면 팝업 
            F_OnPopup(PopupMode.Continue);
        }
    }
    private void F_Exit()
    {
        _clickAudioSource.Play();

        // 1. ExitPopup 출력
        F_OnPopup(PopupMode.Exit);
    }


    private void F_OnPopup(PopupMode v_mode)
    {
        // 1. 팝업 ON
        _popup.SetActive(true);

        // 2. 현재 팝업상태를 업데이트
        _currentpopupMode = v_mode;
        switch(v_mode)
        {
            case PopupMode.NewGame:
                _popupTEXT.text = "게임데이터가 존재합니다. \r\n게임 초기화?";               // 게임 데이터 리셋
                break;

            case PopupMode.Continue:
                _popupTEXT.text = "게임데이터가 없습니다. \r\n새 게임 시작?";       // 데이터 없음 새게임?
                break;

            case PopupMode.Exit:    
                _popupTEXT.text = "게임 종료?";                     // 게임 종료 ?
                break;
        }
    }
    private void F_PopupButtonYES()
    {
        _clickAudioSource.Play();
        switch (_currentpopupMode)
        {
            case PopupMode.NewGame:
                F_ResetPlayerData();                // 세이브 파일 초기화
                StartCoroutine(F_ChangeScene());    // 로딩 UI 및 게임 시작 ( 씬 변경
                break;

            case PopupMode.Continue:
                StartCoroutine(F_ChangeScene());    // 로딩 UI 및 게임 시작 ( 씬 변경
                break;

            case PopupMode.Exit:
                // 게임 종료
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
    /// 데이터가 있으면 true / 데이터가 없으면 false
    /// </summary>
    private bool F_CheckPlayerData()
    {
        int uid = AccountManager.Instance.uid;

        // Guest ( Local )
        if(uid == -1)
        {
            string saveFilePath = _savePath + _playerSaveFileName + ".json"; // 세이브 파일 위치

            // 1. 플레이어 세이브 데이터가 없으면 false
            if (!File.Exists(saveFilePath))
            {
                return false;
            }

            // 2. 플레이어 세이브 데이터가 있으면 true
            return true;
        }

        // Login ( DB )
        else
        {
            string saveFile;

            // 쿼리문
            string query = string.Format("SELECT InventoryData From {0} where uid = {1}",
                _dataTableName, uid);

            DataSet data = DBConnector.Instance.F_Select(query, _dataTableName);

            foreach (DataRow row in data.Tables[0].Rows)
            {
                saveFile = row["inventoryData"].ToString();

                // 세이브 파일이 없으면 false
                if (saveFile == "NONE")
                {
                    return false;
                }

                // 세이브 파일이 있으면 true
                return true;
            }

            // DB에 데이터자체가 없으면 false
            return false;
        }
    }

    /// <summary>
    /// 세이브 파일 초기화
    /// </summary>
    private void F_ResetPlayerData()
    {
        int uid = AccountManager.Instance.uid;

        // LOCAL
        if (uid == -1)
        {
            // 세이브 파일 삭제
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
        // 로딩 UI ON
        _loadingUI.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        // 게임 시작
        SceneManager.LoadScene("01_Main"); 
    }
}
