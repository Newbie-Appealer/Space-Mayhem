using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
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
    
    [Header("Player UI")]
    // 0 : 산소 , 1 : 물 , 2 : 배고픔
    [SerializeField] private Image[] _player_StatUI;
    
    protected override void InitManager() { }

    public void F_InventoryUI()
    {
        // 인벤토리 켜져있을때 추가해야할 기능.
        // 움직임 X
        // 마우스 커서 고정 해제 

        if (_inventoryUI.activeSelf) // 켜져있으면
        {
            _inventoryUI.SetActive(false);                              // 인벤토리 OFF

            _itemInfomation[0].text = "";
            _itemInfomation[1].text = "";
            _itemInfoImage.sprite = ResourceManager.Instance.emptySlotSprite;

            F_SlotFuntionUIOff();
        }
        else // 꺼져있으면
        {
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); // 인벤토리 업데이트
            _inventoryUI.SetActive(true);                               // 인벤토리 ON
        }
    }

    public void F_UpdateInventoryInformation(int v_Index)
    {
        ItemData data = ItemManager.Instance.F_GetData(v_Index);

        _itemInfomation[0].text = data._itemName;
        _itemInfomation[1].text = data._itemDescription;
        _itemInfoImage.sprite = ResourceManager.Instance.F_GetInventorySprite(data._itemCode);
    }


    public void F_SlotFunctionUI(int v_index)
    {
        if (_slotFunctionUI.activeSelf)
        {
            Debug.Log("hummm?");
            return;
        }

        ItemManager.Instance.inventorySystem._selectIndex = v_index;
        int itemCode = ItemManager.Instance.inventorySystem.inventory[v_index].itemCode;
        _selectItemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(itemCode);

        _slotFunctionUI.SetActive(true);
    }

    public void F_SlotFuntionUIOff()
    {
        ItemManager.Instance.inventorySystem._selectIndex = -1;
        _slotFunctionUI.SetActive(false);
    }


    public void F_PlayerStatUIUpdate()
    {
        _player_StatUI[0].fillAmount = PlayerManager.Instance.F_GetStat(0) / 100f ;
        _player_StatUI[1].fillAmount = PlayerManager.Instance.F_GetStat(1) / 100f;
        _player_StatUI[2].fillAmount = PlayerManager.Instance.F_GetStat(2) / 100f;
    }
}