using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    // 인벤토리 UI 델리게이트
    public delegate void UIDelegate();
    public UIDelegate OnInventoryUI;          // 인벤토리 UI ON/OFF 델리게이트 체인

    [Header("=== Unity ===")]
    [SerializeField] private Canvas _canvas;
    public Canvas canvas => _canvas;

    [Header("=== Inventory UI ===")]
    [SerializeField] private GameObject         _inventoryUI;
    [SerializeField] private TextMeshProUGUI[]  _itemInfomation;        // 0 title 1 description
    [SerializeField] private Image              _itemInfoImage;                      
    [SerializeField] private GameObject         _slotFunctionUI;        // 아이템 우클릭 팝업
    [SerializeField] private Image              _selectItemImage;                    
    [SerializeField] private GameObject[]       _quickSlotFocus;        // 현재 선택중인 슬롯
    public GameObject slotFunctionUI => _slotFunctionUI;

    [Header("=== Craft UI ===")]
    [SerializeField] private GameObject     _craftingUI;                // 제작 UI 최상위 오브젝트
    [SerializeField] private GameObject[]   _craftingScroll;            // 제작 UI 카테고리 스크롤 오브젝트

    [Header("=== Other UI ===")]
    [SerializeField] private GameObject _otherUI;                       // 창고, 정제기, 탱크류 등등의 오브젝트 상호작용 UI 최상위 오브젝트
    [SerializeField] private GameObject _smallStorageUI;                // 창고 UI 오브젝트 ( small )
    [SerializeField] private GameObject _bigStorageUI;                  // 창고 UI 오브젝트 ( big )
    [SerializeField] private GameObject _PurifierUI;                    // 정제기 UI 오브젝트
    [SerializeField] private GameObject _tankUI;                        // 탱크류 아이템 UI 오브젝트 

    [Header("=== Tank UI contents ===")]
    [SerializeField] TextMeshProUGUI    _tankUITitleTEXT;               // 탱크 UI 타이틀
    [SerializeField] TextMeshProUGUI    _statePowerTEXT;                // 탱크 UI 파워상태 텍스트
    [SerializeField] TextMeshProUGUI    _stateFilterTEXT;               // 탱크 UI 필터상태 텍스트   
    [SerializeField] TextMeshProUGUI    _chargingStateText;             // 탱크 게이지 충전 상태 텍스트
    [SerializeField] Image              _chargingGauge;                 // 게이지 이미지
    [SerializeField] TextMeshProUGUI    _GaugeTEXT;                     // 게이지 상태 텍스트
    [SerializeField] Button             _chargingButton;                // 게이지 회복 버튼 

    [Header("=== Item ===")]
    [SerializeField] private GameObject         _getItemTableUI;        
    [SerializeField] private GameObject         _getItemUI;             // 획득한 아이템 표시 UI
    [SerializeField] private TextMeshProUGUI    _getItemName;
    [SerializeField] private Image              _getItemImage;

    [Header("=== Player UI ===")]
    // 0 : 산소 , 1 : 물 , 2 : 배고픔
    [SerializeField] private Image[]            _player_StatUI;             // 플레이어 상태 게이지 배열
    [SerializeField] private Image[]             _player_StatDangerUI;
    [SerializeField] private TextMeshProUGUI    _player_intercation_Text;   // 상호작용 텍스트 
    [SerializeField] private TextMeshProUGUI    _player_Message_Text;       // 플레이어 메세지 텍스트 ( 경고 등등 일부 메세지 전달용 )
    [SerializeField] private GameObject         _player_CrossHair;          // CrossHair
    [SerializeField] private Image              _player_FireGauge;          // 파밍도구 게이지
    private bool[] _dangerUIIncrease;
    private IEnumerator[] _playerDangerCoroutine;

    [Header("=== Pause UI ===")]
    [SerializeField] private GameObject _pauseUI;                           // PauseUI 오브젝트
    [SerializeField] private Button     _pauseBackButton;                   // PauseUI 뒤로가기 버튼
    [SerializeField] private Button     _pauseQuitGameButton;               // PauseUI 게임종료 버튼

    [Header("=== Loding UI ===")]
    [SerializeField] private GameObject _loadingUI;                         // 로딩 오브젝트

    [Header("=== KeyGuide UI ===")]
    [SerializeField] private GameObject _KeyGuideUI;                        // KeyGuideUI 오브젝트

    [Header("=== KnockDown/Death UI ===")]
    [SerializeField] private GameObject _knockdownUI;  //기절 UI 오브젝트
    [SerializeField] private GameObject _deathUI;  //사망 UI 오브젝트
    [SerializeField] private TextMeshProUGUI _deathUI_Text; //사망 UI 중앙 텍스트
    [SerializeField] private Button _deathUI_Btn; //사망 UI 하단 버튼
    private string _deathUI_Text_input = "Unfortunately, you died of oxygen deficiency.\r\nYou have been floating in space forever.";

    #region Get/Set
    public bool onInventory => _inventoryUI.activeSelf;
    public bool onRecipe => _craftingUI.activeSelf;
    public bool onPurifier => _PurifierUI.activeSelf && _otherUI.activeSelf;
    public bool onTank => _tankUI.activeSelf && _otherUI.activeSelf;
    public bool onPause => _pauseUI.activeSelf;
    public bool onLoading => _loadingUI.activeSelf;
    #endregion

    private Coroutine _messageFaceOutCoroutine;

    protected override void InitManager()
    {
        F_QuickSlotFocus(-1);

        OnInventoryUI = F_OnInventoryUI;                        // 인벤토리 열기
        OnInventoryUI += F_OnRecipe;                            // 제작 UI 열기
        OnInventoryUI += F_UpdateItemInformation_Empty;         // 인벤토리 UI 관련 초기화
        OnInventoryUI += F_SlotFuntionUIOff;                    // 인벤토리 UI 관련 초기화
        OnInventoryUI += () => F_QuickSlotFocus(-1);            // 퀵슬롯 포커스 해제
        OnInventoryUI += ItemManager.Instance.inventorySystem.F_InventoryUIUpdate;  // 인벤토리 아이템 정보 최신화
        OnInventoryUI += () => F_initRecipeCategory(-1);        // 제작 UI 초기화 ( 선택된 카테고리 )

        _pauseBackButton.onClick.AddListener(F_PauseUIBack);
        _pauseQuitGameButton.onClick.AddListener(F_QuitGame);
        _dangerUIIncrease = new bool[3];
        _playerDangerCoroutine = new IEnumerator[3];
        for(int l = 0; l < _dangerUIIncrease.Length; l++)
        {
            _dangerUIIncrease[l] = false;
            _playerDangerCoroutine[l] = C_PlayerInDangerUI(l);
        }
    }   

    #region 인벤토리/제작 UI 관련

    // 인벤토리 UI  On/Off 함수
    public void F_OnInventoryUI()
    {
        _inventoryUI.SetActive(!onInventory);           // UI ON/OFF 상태 반전      true <-> false

        GameManager.Instance.F_SetCursor(onInventory);
        _player_CrossHair.SetActive(!onInventory);

        // other UI 전부 끄기.
        for(int i = 0; i < _otherUI.transform.childCount; i++)
            _otherUI.transform.GetChild(i).gameObject.SetActive(false);
        _otherUI.SetActive(false);
    }

    // 아이템 설명 UI 초기화
    public void F_UpdateItemInformation_Empty()
    {
        _itemInfomation[0].text = "";
        _itemInfomation[1].text = "";
        _itemInfoImage.sprite = ResourceManager.Instance.emptySlotSprite;
    }

    // 아이템 설명 UI 초기화
    public void F_UpdateItemInformation(int v_Index)
    {
        ItemData data = ItemManager.Instance.ItemDatas[v_Index];

        _itemInfomation[0].text = data._itemName;
        _itemInfomation[1].text = data._itemDescription;
        _itemInfoImage.sprite = ResourceManager.Instance.F_GetInventorySprite(data._itemCode);
    }

    // 아이템 슬롯 기능 UI 켜졌을때 
    public void F_SlotFunctionUI(int v_index, int v_itemCode, SlotType v_slotType)
    {
        if (!_slotFunctionUI.activeSelf)
        {
            ItemManager.Instance.inventorySystem._selectIndex = v_index;
            ItemManager.Instance.inventorySystem._selectSlotType = v_slotType;
            _selectItemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(v_itemCode);

            _slotFunctionUI.SetActive(true);
        }
    }

    // 아이템 슬롯 기능 UI 꺼졌을때
    public void F_SlotFuntionUIOff()
    {
        SoundManager.Instance.F_PlaySFX(SFXClip.CLICK4);

        ItemManager.Instance.inventorySystem._selectIndex = -1;
        ItemManager.Instance.inventorySystem._selectSlotType = SlotType.NONE;
        _slotFunctionUI.SetActive(false);
    }

    // 아이템 퀵슬롯 포커스 함수
    public void F_QuickSlotFocus(int v_index)
    {
        for(int i = 0; i < _quickSlotFocus.Length; i++)
        {
            if (i == v_index)
                _quickSlotFocus[i].SetActive(true);
            else
                _quickSlotFocus[i].SetActive(false);
        }
    }
    #endregion

    #region 제작 UI
    public void F_OnRecipe()
    {
        _craftingUI.SetActive(!onRecipe);
    }

    public void F_initRecipeCategory(int v_category)
    {
        for(int i = 0; i < _craftingScroll.Length; i++)
        {
            _craftingScroll[i].SetActive(false);
            if(v_category == i)
            {
                SoundManager.Instance.F_PlaySFX(SFXClip.CLICK4);
                _craftingScroll[i].SetActive(true);
            }
        }
    }
    #endregion

    #region Other UI
    #region Storage
    public void F_OnSmallStorageUI(bool v_bValue)
    {
        _otherUI.SetActive(v_bValue);
        _smallStorageUI.gameObject.SetActive(v_bValue);
    }
    public void F_OnBigStorageUI(bool v_bValue)
    {
        _otherUI.SetActive(v_bValue);
        _bigStorageUI.gameObject.SetActive(v_bValue);
    }
    #endregion

    #region 정제기
    public void F_OnPurifierUI(bool v_bValue)
    {
        _otherUI.SetActive(v_bValue);
        _PurifierUI.SetActive(v_bValue);
    }
    #endregion

    #region Tank ( water / oxygen )
    public void F_OnTankUI(TankType v_type, bool v_statePower, bool v_stateFilter, bool v_bValue)
    {
        _otherUI.SetActive(v_bValue);
        _tankUI.SetActive(v_bValue);

        _statePowerTEXT.gameObject.SetActive(!v_statePower);                    // 전원이 연결되어 있으면
        _stateFilterTEXT.gameObject.SetActive(!v_stateFilter);                  // 필터가 주위에 있으면
        _chargingStateText.gameObject.SetActive(v_statePower && v_stateFilter); // 전원+필터가 연결되어있으면
        switch (v_type)
        {
            case TankType.WATER:
                _tankUITitleTEXT.text = "WATER TANK";
                break;

            case TankType.OXYGEN:
                _tankUITitleTEXT.text = "OXYGEN TANK";
                break;
        }
    }

    public void F_UpdateTankUITEXT(bool v_statePower, bool v_stateFilter)
    {
        _statePowerTEXT.gameObject.SetActive(!v_statePower);                    // 전원이 연결되어 있으면
        _stateFilterTEXT.gameObject.SetActive(!v_stateFilter);                  // 필터가 주위에 있으면
    }

    public void F_BindingTankUIEvent(Action v_event)
    {
        _chargingButton.onClick.RemoveAllListeners();                           // 버튼 클릭 이벤트 초기화 
        _chargingButton.onClick.AddListener(() => v_event());
    }

    public void F_UpdateTankGauge(float v_value, string v_text)
    {
        _chargingGauge.fillAmount = v_value;
        _GaugeTEXT.text = v_text;
    }
    #endregion
    #endregion

    #region 아이템 획득 관련
    public IEnumerator C_GetItemUIOn(Sprite v_spr, string v_name)
    {
        _getItemUI.GetComponent<GetScrap>().F_GetScrapUIUpdate(v_spr, v_name);
        GameObject _item_Get = Instantiate(_getItemUI);
        _item_Get.transform.SetParent(_getItemTableUI.transform);
        _item_Get.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        Destroy(_item_Get);
    }

    #endregion

    #region 플레이어 UI 관련
    public void F_PlayerStatUIUpdate(PlayerStatType v_type)
    {
        int idx = (int)v_type;
        _player_StatUI[idx].fillAmount = PlayerManager.Instance.F_GetStat(idx) / 100f;
    }

    public void F_PlayerStatUIDanger(PlayerStatType v_type)
    {
        int idx = (int)v_type;
        _player_StatUI[idx].color = new Color(255 / 255f, 67 / 255f, 67 / 255f, 255/ 255f);
        StartCoroutine(_playerDangerCoroutine[idx]);
    }

    public void F_PlayerStatUIRecover(PlayerStatType v_type)
    {
        int idx = (int)v_type;
        _player_StatUI[idx].color = Color.white;
        F_InitPlayerDangerUI(idx);
    }

    private void F_InitPlayerDangerUI(int v_idx)
    {
        StopCoroutine(_playerDangerCoroutine[v_idx]);
        float _colorAlpha = 0f;
        _player_StatDangerUI[v_idx].color = new Color(_player_StatDangerUI[v_idx].color.r, _player_StatDangerUI[v_idx].color.g, _player_StatDangerUI[v_idx].color.b, _colorAlpha);
    }

    private IEnumerator C_PlayerInDangerUI(int v_idx)
    {
        float _colorAlpha = _player_StatDangerUI[v_idx].color.a;
        while (true)
        {
            if (!_dangerUIIncrease[v_idx])
            {
                _colorAlpha++;
                if (_colorAlpha >= 100f)
                    _dangerUIIncrease[v_idx] = true;
            }
            else
            {
                _colorAlpha--;
                if (_colorAlpha <= 0.01f)
                    _dangerUIIncrease[v_idx] = false;
            }
           _player_StatDangerUI[v_idx].color = new Color(255 / 255f, 23 / 255f, 23 / 255f, _colorAlpha / 255f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void F_IntercationPopup(bool v_bValue, string v_text)
    {
        // 인벤토리 켜져있을때 상호작용 팝업 끄기
        _player_intercation_Text.gameObject.SetActive(v_bValue);

        if (v_bValue)
            _player_intercation_Text.text = v_text.Replace("\\n", "\n"); // 줄바꿈 추가.
    }

    public void F_PlayerMessagePopupTEXT(string v_text,float v_time = 1f)
    {
        // 1. FadeOut 코루틴이 실행중일때
        if(_player_Message_Text.color.a > 0)
        {
            StopCoroutine(_messageFaceOutCoroutine);            // 코루틴 중지
            _messageFaceOutCoroutine = null;                    // 초기화
            _player_Message_Text.color = new Color(0, 0, 0, 0); // 색 초기화
        }

        // 2. 팝업의 텍스트 변경 및 FadeOutMessage 코루틴을 실행
        _player_Message_Text.text = v_text;
        _messageFaceOutCoroutine = StartCoroutine(C_FadeOutMessage(v_time));
    }

    IEnumerator C_FadeOutMessage(float v_time)
    {
        // [0.02씩 감소 , 0.03초에 한번씩]
        _player_Message_Text.color = Color.yellow;  // 색 초기화
        float colorAlpha = 1;                       // alpha 값
        yield return new WaitForSeconds(v_time);
        while(colorAlpha > 0.05f)                   // alpha값이 0.05이하가 될때까지
        {
            colorAlpha -= 0.02f;
            Color newColor = _player_Message_Text.color;
            newColor.a = colorAlpha;
            _player_Message_Text.color = newColor;
            yield return new WaitForSeconds(0.03f);
        }
        _player_Message_Text.color = new Color(0, 0, 0, 0);
    }

    public Image F_GetPlayerFireGauge()
    {
        return _player_FireGauge;
    }
    
    public IEnumerator C_FireGaugeFadeOut()
    {
        float _colorAlpha = 1;
        while (_colorAlpha > 0)
        {
            _colorAlpha -= 0.015f;
            Color _alpha = _player_FireGauge.color;
            _alpha.a = _colorAlpha;
            _player_FireGauge.color = _alpha;
            yield return new WaitForSeconds(0.0001f);
        }
        PlayerManager.Instance._canShootPistol = true;
        _player_FireGauge.fillAmount = 0;
        _player_FireGauge.color = new Color(0, 0, 0, 0);
        yield return null;
    }

    #endregion

    #region Pause UI
    public void F_OnPauseUI(bool v_state)
    {
        _pauseUI.SetActive(v_state);
        GameManager.Instance.F_SetCursor(v_state);

        // 옵션UI 추가되면 이곳에서 옵션UI 닫아주는 기능 추가해야함.
    }

    private void F_QuitGame()
    {
        SoundManager.Instance.F_PlaySFX(SFXClip.CLICK4);
        SaveManager.Instance.GameDataSave();
        Application.Quit();
    }

    private void F_PauseUIBack()
    {
        SoundManager.Instance.F_PlaySFX(SFXClip.CLICK4);
        F_OnPauseUI(false);
    }
    #endregion

    #region Loading UI
    public void F_OnLoading(bool v_state)
    {
        // 이건 임시임!
        _loadingUI.SetActive(v_state);
    }
    #endregion

    #region KeyGuide UI
    public void F_OnClickKeyGuide()
    {
        if (_KeyGuideUI.activeSelf)
            _KeyGuideUI.SetActive(false);
        else
            _KeyGuideUI.SetActive(true);
    }
    #endregion

    #region 사망/기절 UI
    public void F_KnockDownUI(bool v_state)
    {
        _knockdownUI.SetActive(v_state);
        PlayerManager.Instance.PlayerController.F_PlayerDead();
    }
    
    public void F_DeathUI()
    {
        int _childCount = _canvas.transform.childCount;
        for (int l = 0; l <  _childCount; l++)
        {
            _canvas.transform.GetChild(l).gameObject.SetActive(false);
        }
        _deathUI_Text.text = string.Empty;
        StartCoroutine(C_OnDeathUI(_deathUI_Text_input));
    }

    private IEnumerator C_OnDeathUI(string v_str)
    {
        _deathUI.SetActive(true);
        for (int l = 0; l < v_str.Length; l++)
        {
            _deathUI_Text.text += v_str[l];
            yield return new WaitForSeconds(0.05f);
        }
        if (_deathUI_Text.text == v_str)
        {
            _deathUI_Btn.gameObject.SetActive(true);
            GameManager.Instance.F_SetCursor(true);
        }
    }

    public void F_ClickRestartBtn()
    {
        SaveManager.Instance.F_ResetLocalData();
        SceneManager.LoadScene("00_Lobby");
    }

    public void F_ClickReturnBtn()
    {
        _knockdownUI.SetActive(false);
        PlayerManager.Instance.F_PlayerReturnToSpaceShip();
        //TODO: 맵 삭제 및 플레이어 위치 우주선으로 초기화
    }
    #endregion
}