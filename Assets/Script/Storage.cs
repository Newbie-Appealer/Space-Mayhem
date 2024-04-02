using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [Header("Storage Infomation")]
    [SerializeField] private int _storageSize;                  // ���丮�� ũ��
    [SerializeField] private List<ItemSlot> _slots;             // ����

    [Header("Storage itemData")]
    [SerializeField] private Item[] _items;                     // ���丮�� ������ ����
    public Item[] items => _items;

    [Space]
    [SerializeField] private Transform _storageSlotTransform;                // Slot Parent
    [SerializeField] private InventorySystem _inventorySystem;
    private void Start()
    {
        _items = new Item[_storageSize]; 
        _slots = new List<ItemSlot>();
        _inventorySystem = ItemManager.Instance.inventorySystem;

        if(_storageSize == 8)
            _storageSlotTransform = _inventorySystem.smallStorage.GetChild(0);
        else
            _storageSlotTransform = _inventorySystem.bigStorage.GetChild(0);


        // �ش� ���丮���� ����� ������ ������ �ʱ�ȭ
        for (int i = 0; i < _storageSize; i++)
            _slots.Add(_storageSlotTransform.GetChild(i).GetComponent<ItemSlot>());
    }

    /// <summary> ���� ���� / ���뿡 ��ȭ�� ������ ����ϱ�</summary>
    public void F_StorageUIUpdate()
    {
        for(int i = 0; i < _slots.Count; i++)
        {
            if (_items[i] == null)
                _slots[i].F_EmptySlot();
            else if (_items[i].F_IsEmpty())
            {
                _slots[i].F_EmptySlot();
                _items[i] = null;
            }
            else
            {
                _slots[i].F_UpdateSlot(_items[i].itemCode, _items[i].currentStack);
            }   
        }
    }

    public void F_OpenStorage()
    {
        // 1. ItemManager���� ���õ� ���丮���� ������Ʈ.
        ItemManager.Instance.F_SelectStorage(this);

        foreach (ItemSlot slot in _slots)
            slot.itemSlotRef = items;

        // 2. â�� ������ ���� �ֽ�ȭ
        F_StorageUIUpdate();

        // 3. â��/�κ��丮 UI Ȱ��ȭ
        UIManager.Instance.OnInventoryUI();

        if (_storageSize == 8)
            UIManager.Instance.F_OnSmallStorageUI(true);
        else
            UIManager.Instance.F_OnBigStorageUI(true);

    }
}
