using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallStorage : Storage
{
    int _size = 8;
    private void Start()
    {
        _storageSize = _size;                       // 초기화
        _items = new Item[_size];                   // 초기화
        _slots = new List<StorageSlot>();           // 초기화

        _storage = ItemManager.Instance.inventorySystem.smallStorage;

        F_InitSlot();
    }
}
