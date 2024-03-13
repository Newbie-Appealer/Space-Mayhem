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
        // �κ��丮 ���������� �߰��ؾ��� ���.
        // ������ X
        // ���콺 Ŀ�� ���� ���� 

        if (_inventoryUI.activeSelf) // ����������
        {
            _inventoryUI.SetActive(false);                              // �κ��丮 OFF

            _itemInfomation[0].text = "";
            _itemInfomation[1].text = "";
            _itemInfoImage.sprite = ResourceManager.Instance.emptySlotSprite;
        }
        else // ����������
        {
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); // �κ��丮 ������Ʈ
            _inventoryUI.SetActive(true);                               // �κ��丮 ON
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
