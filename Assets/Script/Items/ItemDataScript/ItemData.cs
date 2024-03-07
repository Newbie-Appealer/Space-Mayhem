using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int _itemCode;
    public string _itemName;
    public string _itemDescription;
    public int _itemStack;
    public ItemData(int v_code, string v_name, string v_description)
    {
        _itemCode = v_code;
        _itemName = v_name;
        _itemDescription = v_description;
        _itemStack = 1;                     // 아잍메이 새로 생성되었으니 기본 스택은 1
    }
}
