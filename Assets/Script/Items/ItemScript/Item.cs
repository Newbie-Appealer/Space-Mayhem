using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] private ItemData _data;

    public Item(ItemData v_data)
    {
        _data = v_data;
    }
}
