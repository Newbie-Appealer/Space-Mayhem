using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private TextMeshProUGUI[] _itemInfomation;         // 0 title 1 description
    [SerializeField] private Image _itemInfoImage;                      
    [SerializeField] private GameObject _slotFunctionUI;                // 아이템 우클릭 팝업
    [SerializeField] private Image _selectItemImage;                    
    [SerializeField] private GameObject _getItemUI;                     // 획득한 아이템 표시 UI
    [SerializeField] private GameObject[] _quickSlotFocus;              // 현재 선택중인 슬롯
    public GameObject slotFunctionUI => _slotFunctionUI;

    [Header("Craft UI")]
    [SerializeField] private GameObject _craftingUI;
    [SerializeField] private GameObject[] _craftingScroll;

    [Header("Other UI")]
    [SerializeField] private GameObject _otherUI;
    [SerializeField] private GameObject _smallStorageUI;
    [SerializeField] private GameObject _bigStorageUI;

    [Header("Item")]
    [SerializeField] private TextMeshProUGUI _getItemName;
    [SerializeField] private Image _getItemImage;

    [Header("Player UI")]
    // 0 : 산소 , 1 : 물 , 2 : 배고픔
    [SerializeField] private Image[] _player_StatUI;
    [SerializeField] private TextMeshProUGUI _player_intercation_Text;
    [SerializeField] private Image _player_FireGauge;


    public bool onInventory => _inventoryUI.activeSelf;
    public bool onRecipe => _craftingUI.activeSelf;
    protected override void InitManager()
    {
        F_QuickSlotFocus(-1);

        OnInventoryUI = F_OnInventoryUI;                        // 인벤토리 열기
        OnInventoryUI += F_OnRecipe;                            // 제작 UI 열기
        OnInventoryUI += F_UpdateItemInformation_Empty;         // 인벤토리 UI 관련 초기화
        OnInventoryUI += F_SlotFuntionUIOff;                    // 인벤토리 UI 관련 초기화
        OnInventoryUI += () => F_QuickSlotFocus(-1);            // 퀵슬롯 포커스 해제
        OnInventoryUI += ItemManager.Instance.inventorySystem.F_InventoryUIUpdate;  // 인벤토리 아이템 정보 최신화
        OnInventoryUI += () => F_initRecipeCategory(-1);        // 제작 UI 초기화 ( 카테고리 )
    }   

    #region 인벤토리/제작 UI 관련

    // 인벤토리 UI  On/Off 함수
    public void F_OnInventoryUI()
    {
        _inventoryUI.SetActive(!onInventory);           // UI ON/OFF 상태 반전      true <-> false

        GameManager.Instance.F_SetCursor(onInventory);
        _player_FireGauge.gameObject.SetActive(!onInventory);

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
    public void F_SlotFunctionUI(int v_index)
    {
        if (!_slotFunctionUI.activeSelf)
        {
            ItemManager.Instance.inventorySystem._selectIndex = v_index;
            int itemCode = ItemManager.Instance.inventorySystem.inventory[v_index].itemCode;
            _selectItemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(itemCode);

            _slotFunctionUI.SetActive(true);
        }
    }

    // 아이템 슬롯 기능 UI 꺼졌을때
    public void F_SlotFuntionUIOff()
    {
        ItemManager.Instance.inventorySystem._selectIndex = -1;
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

    #endregion

    #region 아이템 획득 관련
    public void F_GetItemPopup(string v_name, Sprite v_sp)
    {
        _getItemName.text = v_name;
        _getItemImage.sprite = v_sp;
    }
    public IEnumerator C_GetItemUIOn(Sprite v_spr, string v_name)
    {
        _getItemName.text = v_name;
        _getItemImage.sprite = v_spr;
        _getItemUI.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _getItemUI.SetActive(false);
    }

    #endregion

    #region 플레이어 UI 관련
    public void F_PlayerStatUIUpdate()
    {
        //_player_StatUI[0].fillAmount = PlayerManager.Instance.F_GetStat(0) / 100f;
        //_player_StatUI[1].fillAmount = PlayerManager.Instance.F_GetStat(1) / 100f;
        //_player_StatUI[2].fillAmount = PlayerManager.Instance.F_GetStat(2) / 100f;
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

    #endregion

}