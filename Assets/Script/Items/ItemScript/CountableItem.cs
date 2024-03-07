using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableItem : Item
{
    protected CountableData _countableData
    { get; private set; }
    public CountableItem(CountableData v_data) : base(v_data)
    {
        _countableData = v_data;
    }

    /// <summary> 현재스택이 최대스택 미만일때 true를 반환합니다. (아이템 개수를 추가할수있는지) </summary>
    /// 
    public bool F_CheckItemStack()
    {
        if(_countableData.itemStack < _countableData.maxStack) 
            return true;
        return false;
    }
}
