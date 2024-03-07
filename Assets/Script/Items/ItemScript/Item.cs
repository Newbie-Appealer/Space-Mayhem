using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public ItemData itemdata => _itemdata;
    [SerializeField] protected ItemData _itemdata;

    public Item(ItemData v_data)
    { _itemdata = v_data; }

    /// <summary> 현재 아이템의 스택을 추가하는 함수 </summary>
    public void F_AddStack(int v_value) 
    {    
        _itemdata.F_AddStack(v_value);
    }

    /// <summary> 매개변수로 들어오는 아이템과 같은 아이템인지 itemCode로 비교/확인 함수 </summary>
    public bool F_CheckItemCode(Item v_data)
    {
        if (_itemdata.itemCode == v_data._itemdata.itemCode)
            return true;
        return false;
    }
}
