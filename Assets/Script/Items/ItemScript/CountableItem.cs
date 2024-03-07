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

    /// <summary> ���罺���� �ִ뽺�� �̸��϶� true�� ��ȯ�մϴ�. (������ ������ �߰��Ҽ��ִ���) </summary>
    /// 
    public bool F_CheckItemStack()
    {
        if(_countableData.itemStack < _countableData.maxStack) 
            return true;
        return false;
    }
}
