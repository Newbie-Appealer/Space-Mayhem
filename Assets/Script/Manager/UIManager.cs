using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    // 인벤토리 델리게이트
    public delegate void inventoryDelegate();
    public inventoryDelegate inventoryUI;                                  // 인벤토리 UI가 켜질때 실행되어야할 델리게이트 체인

    [Header("Unity")]
    [SerializeField] private Canvas _canvas;
    public Canvas canvas => _canvas;

    [Header("Inventory UI")]
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private TextMeshProUGUI[] _itemInfomation;         // 0 title 1 description
    [SerializeField] private Image _itemInfoImage;
    [SerializeField] private GameObject _slotFunctionUI;
    [SerializeField] private Image _selectItemImage;
    [SerializeField] private GameObject _getItemUI;
    [SerializeField] private GameObject[] _quickSlotFocus;              // 현재 선택중인 슬롯
    public GameObject slotFunctionUI => _slotFunctionUI;

    [Header("Craft UI")]
    [SerializeField] private GameObject[] _craftingUI;

    [Header("Item")]
    [SerializeField] private TextMeshProUGUI _getItemName;
    [SerializeField] private Image _getItemImage;

    [Header("Player UI")]
    // 0 : 산소 , 1 : 물 , 2 : 배고픔
    [SerializeField] private Image[] _player_StatUI;
    [SerializeField] private TextMeshProUGUI _player_GetUI_Text;
    [SerializeField] private Image _player_FireGauge;
    
    protected override void InitManager()
    {
        inventoryUI = F_OnInventory;
        F_QuickSlotFocus(-1);
    }   

    #region 인벤토리/제작 UI 관련
    public void F_AddInventoryFunction(inventoryDelegate v_func)
    {
        inventoryUI += v_func;
    }

    // 인벤토리 UI  On/Off 함수
    public void F_OnInventory()
    {
        if (_inventoryUI.activeSelf) // 켜져있으면
        {
            _inventoryUI.SetActive(false);                              // 인벤토리 OFF

            GameManager.Instance.F_SetCursor(false);
            _player_FireGauge.gameObject.SetActive(true);
            F_UpdateItemInformation_Empty();
            F_SlotFuntionUIOff();
            F_OnRecipe(-1);                                             // 제작 카테고리 UI 다 꺼버리기
        }
        else // 꺼져있으면
        {
            _inventoryUI.SetActive(true);                               // 인벤토리 ON

            GameManager.Instance.F_SetCursor(true);
            _player_FireGauge.gameObject.SetActive(false);
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); // 인벤토리 업데이트
        }
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
    public void F_OnRecipe(int v_category)
    {
        for(int i = 0; i < _craftingUI.Length; i++)
        {
            _craftingUI[i].SetActive(false);
            if(v_category == i)
                _craftingUI[i].SetActive(true);
        }
    }
    #endregion

    #endregion

    #region 아이템 관련
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

    public void F_PlayerCheckScrap(bool v_bValue)
    {
        _player_GetUI_Text.gameObject.SetActive(v_bValue);
        if(v_bValue )
            _player_GetUI_Text.text = "Press E to Get Item";
    }

    public Image F_GetPlayerFireGauge()
    {
        return _player_FireGauge;
    }

    #endregion

}