using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallStorage : Storage
{
    int _size = 8;
    private void Start()
    {
        _storageSize = _size;                       // �ʱ�ȭ
        _items = new Item[_size];                   // �ʱ�ȭ
        _slots = new List<StorageSlot>();           // �ʱ�ȭ

        _storage = ItemManager.Instance.inventorySystem.smallStorage;

        F_InitSlot();
    }
}
