using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int itemCode => _itemCode;
    public string itemName => _itemName;
    public string itemDescription => _itemDescription;
    public int itemStack => _itemStack;

    [SerializeField] protected int _itemCode;
    [SerializeField] protected string _itemName;
    [SerializeField] protected string _itemDescription;
    [SerializeField] protected int _itemStack;
    public ItemData(int v_code, string v_name, string v_description)
    {
        _itemCode = v_code;
        _itemName = v_name;
        _itemDescription = v_description;
        _itemStack = 0;
    }
}
