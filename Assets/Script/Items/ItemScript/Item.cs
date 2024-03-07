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

    /// <summary> ���� �������� ������ �߰��ϴ� �Լ� </summary>
    public void F_AddStack(int v_value) 
    {    
        _itemdata.F_AddStack(v_value);
    }

    /// <summary> �Ű������� ������ �����۰� ���� ���������� itemCode�� ��/Ȯ�� �Լ� </summary>
    public bool F_CheckItemCode(Item v_data)
    {
        if (_itemdata.itemCode == v_data._itemdata.itemCode)
            return true;
        return false;
    }
}
