using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    // 인벤토리 UI 델리게이트
    public delegate void UIDelegate();
    public UIDelegate OnInventoryUI;          // 인벤토리 UI ON/OFF 델리게이트 체인

    [Header("Unity")]
    [SerializeField] private Canvas _canvas;
    public Canvas canvas => _canvas;

    [Header("Inventory UI")]
    [SerializeField] private GameObject         _inventoryUI;
    [SerializeField] private TextMeshProUGUI[]  _itemInfomation;        // 0 title 1 description
    [SerializeField] private Image              _itemInfoImage;                      
    [SerializeField] private GameObject         _slotFunctionUI;        // 아이템 우클릭 팝업
    [SerializeField] private Image              _selectItemImage;                    
    [SerializeField] private GameObject[]       _quickSlotFocus;        // 현재 선택중인 슬롯
    public GameObject slotFunctionUI => _slotFunctionUI;

    [Header("Craft UI")]
    [SerializeField] private GameObject     _craftingUI;
    [SerializeField] private GameObject[]   _craftingScroll;

    [Header("Other UI")]
    [SerializeField] private GameObject _otherUI;
    [SerializeField] private GameObject _smallStorageUI;
    [SerializeField] private GameObject _bigStorageUI;
    [SerializeField] private GameObject _PurifierUI;
    [SerializeField] private GameObject _tankUI;

    [Header("Tank UI contents")]
    [SerializeField] TextMeshProUGUI    _tankUITitleTEXT;
    [SerializeField] TextMeshProUGUI    _statePowerTEXT;
    [SerializeField] TextMeshProUGUI    _stateFilterTEXT;
    [SerializeField] TextMeshProUGUI    _chargingSpeedTEXT;
    [SerializeField] Image              _chargingGauge;
    [SerializeField] TextMeshProUGUI    _GaugeTEXT;
    [SerializeField] Button             _chargingButton;  

    [Header("Item")]
    [SerializeField] private GameObject         _getItemTableUI;
    [SerializeField] private GameObject         _getItemUI;                     // 획득한 아이템 표시 UI
    [SerializeField] private TextMeshProUGUI    _getItemName;
    [SerializeField] private Image              _getItemImage;

    [Header("Player UI")]
    // 0 : 산소 , 1 : 물 , 2 : 배고픔
    [SerializeField] private Image[]            _player_StatUI;
    [SerializeField] private TextMeshProUGUI    _player_intercation_Text;
    [SerializeField] private GameObject         _player_CrossHair;
    [SerializeField] private Image              _player_FireGauge;

    [Header("Pause UI")]
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private Button     _pauseBackButton;
    [SerializeField] private Button     _pauseQuitGameButton;

    public bool onInventory => _inventoryUI.activeSelf;
    public bool onRecipe => _craftingUI.activeSelf;
    public bool onPurifier => _PurifierUI.activeSelf && _otherUI.activeSelf;
    public bool onTank => _tankUI.activeSelf && _otherUI.activeSelf;
    public bool onPause => _pauseUI.activeSelf;

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
                _craftingScroll[i].SetActive(true);
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
        _chargingSpeedTEXT.gameObject.SetActive(v_statePower && v_stateFilter); // 전원+필터가 연결되어있으면
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

    public void F_IntercationPopup(bool v_bValue, string v_text)
    {
        // 인벤토리 켜져있을때 상호작용 팝업 끄기
        _player_intercation_Text.gameObject.SetActive(v_bValue);

        if (v_bValue && v_text != _player_intercation_Text.text)
            _player_intercation_Text.text = v_text;
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
        SaveManager.Instance.GameDataSave();
        Application.Quit();
    }

    private void F_PauseUIBack()
    {
        F_OnPauseUI(false);
    }
    #endregion
}