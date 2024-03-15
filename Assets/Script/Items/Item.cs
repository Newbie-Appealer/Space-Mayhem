using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int itemCode => _itemCode;
    public string itemName => _itemName;
    public string itemDescription => _itemDescription;
    public int currentStack => _currentStack;
    public int maxStack => _maxStack;

    public ItemType itemType => _itemType;

    [SerializeField] protected int _itemCode;
    [SerializeField] protected string _itemName;
    [SerializeField] protected string _itemDescription;

    [SerializeField] protected int _currentStack;
    [SerializeField] protected int _maxStack;

    [SerializeField] protected ItemType _itemType;
    public Item(ItemData data)
    {
        _itemCode = data._itemCode;
        _itemName = data._itemName;
        _itemDescription = data._itemDescription;

        _currentStack = 1;
    }

    /// <summary> 빈 아이템 이면 true를 반환합니다.</summary>
    public bool F_IsEmpty()
    {
        if (itemCode == 0 || itemType == ItemType.NONE)
            return true;
        return false;
    }

    /// <summary> 비교대상의 아이템 번호가 동일하면 true를 반환 </summary>
    public bool F_CheckItemCode(int v_code)
    {
        return _itemCode == v_code;
    }
    
    /// <summary> 스택을 더 쌓을수 있을때 true를 반환 </summary>
    public bool F_CheckStack()
    {
        if (_currentStack < _maxStack)
            return true;
        return false;
    }

    public void F_AddStack(int value)
    {
        _currentStack += value;
        if (_currentStack > maxStack)
            _currentStack = maxStack;
    }

    public virtual void F_UseItem()
    {
        Debug.Log("재정의 해주세요");
    }
}