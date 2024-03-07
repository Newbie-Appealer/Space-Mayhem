using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] protected ItemData _data;

    public Item(ItemData v_data)
    { _data = v_data; }

    public bool F_CheckItemCode(ItemData v_data)
    {
        if (_data.itemCode == v_data.itemCode)
            return true;
        return false;
    }
}
