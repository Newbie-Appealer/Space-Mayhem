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
    public inventoryDelegate inventoryUI;                               // 인벤토리 UI가 켜질때 실행되어야할 델리게이트 체인

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

    [Header("Storage UI")]
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
        if (_inventoryUI.activeSelf) // 인벤이 이미 켜져있을때 -> 인벤 끄기
        {
            _inventoryUI.SetActive(false);                              // 1. 인벤토리 OFF

            GameManager.Instance.F_SetCursor(false);                    // 2. 커서 가리기+고정
            _player_FireGauge.gameObject.SetActive(true);               // 3. 마우스포인트? ON
            F_UpdateItemInformation_Empty();                            // 4. 아이템 설명칸 초기화
            F_SlotFuntionUIOff();                                       // 5. 아이템 슬롯 기능 UI OFF
            F_OnRecipe(-1);                                             // 6. 제작 카테고리 UI 다 꺼버리기

            F_OnStorageUI(false);                                       // 7. 상자 UI 끄기
        }
        else // 인벤이 꺼져있을때 -> 인벤 켜기
        {
            _inventoryUI.SetActive(true);                               // 1. 인벤토리 ON

            GameManager.Instance.F_SetCursor(true);                     // 2. 커서 보이기+고정해제
            _player_FireGauge.gameObject.SetActive(false);              // 3. 마우스포인트? Off
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); // 4. 인벤토리 업데이트
        }
    }

    public bool F_CheckInventoryActive()
    {
        return _inventoryUI.activeSelf;
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

    public void F_OnRecipe(int v_category)
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
    public void F_OnStorageUI(bool v_bValue)
    {
        _smallStorageUI.gameObject.SetActive(v_bValue);
        _craftingUI.SetActive(!v_bValue);
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

    public void F_IntercationPopup(bool v_bValue, string v_text)
    {
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