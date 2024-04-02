using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [Header("Storage Infomation")]
    [SerializeField] private int _storageSize;                  // 스토리지 크기
    [SerializeField] private List<ItemSlot> _slots;             // 슬롯

    [Header("Storage itemData")]
    [SerializeField] private Item[] _items;                     // 스토리지 아이템 정보
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


        // 해당 스토리지가 사용할 아이템 슬롯을 초기화
        for (int i = 0; i < _storageSize; i++)
            _slots.Add(_storageSlotTransform.GetChild(i).GetComponent<ItemSlot>());
    }

    /// <summary> 상자 열때 / 내용에 변화가 있을때 사용하기</summary>
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
        // 1. ItemManager에서 선택된 스토리지를 업데이트.
        ItemManager.Instance.F_SelectStorage(this);

        foreach (ItemSlot slot in _slots)
            slot.itemSlotRef = items;

        // 2. 창고 아이템 내용 최신화
        F_StorageUIUpdate();

        // 3. 창고/인벤토리 UI 활성화
        UIManager.Instance.OnInventoryUI();

        if (_storageSize == 8)
            UIManager.Instance.F_OnSmallStorageUI(true);
        else
            UIManager.Instance.F_OnBigStorageUI(true);

    }
}
