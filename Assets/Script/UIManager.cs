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

    [Header("UI")]
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private TextMeshProUGUI[] _itemInfomation;   // 0 title 1 description
    [SerializeField] private Image _itemInfoImage;
    [SerializeField] private GameObject _slotUI;
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
}
