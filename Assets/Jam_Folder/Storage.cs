using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [Header("Storage Infomation")]
    [SerializeField] protected int _storageSize;                // 스토리지 크기
    [SerializeField] protected Item[] _items;                   // 스토리지 아이템 정보

    [Space]
    [SerializeField] protected GameObject _storage;             // UI
    [SerializeField] protected List<StorageSlot> _slots;        // 슬롯\

    protected void F_InitSlot()
    {
        Transform slotTransform = _storage.transform.GetChild(0);

        for (int i = 0; i < _storageSize; i++)
            _slots.Add(slotTransform.GetChild(i).GetComponent<StorageSlot>());
    }

    /// <summary> 상자 열때 / 내용에 변화가 있을때 사용하기</summary>
    private void F_StorageUIUpdate()
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
        F_StorageUIUpdate();
    }
}
