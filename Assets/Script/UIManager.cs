using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Unity")]
    [SerializeField] private Canvas _canvas;
    public Canvas canvas => _canvas;

    [Header("Inventory UI")]
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private TextMeshProUGUI[] _itemInfomation;   // 0 title 1 description
    [SerializeField] private Image _itemInfoImage;
    [SerializeField] private GameObject _slotFunctionUI;
    [SerializeField] private Image _selectItemImage;

    public GameObject slotFunctionUI => _slotFunctionUI;

    [Header("Item")]
    [SerializeField] private TextMeshProUGUI _getItemName;
    [SerializeField] private Image _getItemImage;

    [Header("Player UI")]
    // 0 : 산소 , 1 : 물 , 2 : 배고픔
    [SerializeField] private Image[] _player_StatUI;
    
    protected override void InitManager() { }

    #region 인벤토리/제작 UI 관련
    public void F_InventoryUI()
    {
        if (_inventoryUI.activeSelf) // 켜져있으면
        {
            _inventoryUI.SetActive(false);                              // 인벤토리 OFF

            GameManager.Instance.F_SetCursor(false);
            F_UpdateItemInformation_Empty();
            F_SlotFuntionUIOff();
        }
        else // 꺼져있으면
        {
            _inventoryUI.SetActive(true);                               // 인벤토리 ON

            GameManager.Instance.F_SetCursor(true);
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); // 인벤토리 업데이트
        }
    }

    public void F_UpdateItemInformation_Empty()
    {
        _itemInfomation[0].text = "";
        _itemInfomation[1].text = "";
        _itemInfoImage.sprite = ResourceManager.Instance.emptySlotSprite;
    }

    public void F_UpdateItemInformation(int v_Index)
    {
        ItemData data = ItemManager.Instance.ItemDatas[v_Index];

        _itemInfomation[0].text = data._itemName;
        _itemInfomation[1].text = data._itemDescription;
        _itemInfoImage.sprite = ResourceManager.Instance.F_GetInventorySprite(data._itemCode);
    }

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

    public void F_SlotFuntionUIOff()
    {
        ItemManager.Instance.inventorySystem._selectIndex = -1;
        _slotFunctionUI.SetActive(false);
    }

    #region 제작 UI
    public void F_OnRecipe()
    {

    }
    #endregion

    #endregion

    #region 아이템 관련
    public void F_GetItemPopup(string v_name, Sprite v_sp)
    {
        _getItemName.text = v_name;
        _getItemImage.sprite = v_sp;
    }
    #endregion

    #region 플레이어 UI 관련
    public void F_PlayerStatUIUpdate()
    {
        //_player_StatUI[0].fillAmount = PlayerManager.Instance.F_GetStat(0) / 100f;
        //_player_StatUI[1].fillAmount = PlayerManager.Instance.F_GetStat(1) / 100f;
        //_player_StatUI[2].fillAmount = PlayerManager.Instance.F_GetStat(2) / 100f;
    }
    #endregion

}