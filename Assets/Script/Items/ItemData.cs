using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int _itemCode;
    public string _itemName;
    public string _itemDescription;

    public ItemData(int v_code, string v_name, string v_description)
    {
        _itemCode = v_code;
        _itemName = v_name;
        _itemDescription = v_description;
    }
}
