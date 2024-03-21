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
    [SerializeField] protected PlayerState _playerState;
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
        // 아이템 타입이 none 이거나 현재 스택이 0 인 아이템은 빈 공간
        if (itemType == ItemType.NONE || currentStack <= 0)
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
    { PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1); }
}