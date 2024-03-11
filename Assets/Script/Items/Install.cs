using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Install : Item
{
    public float hp => _hp;
    [SerializeField] private float _hp;
    public Install(ItemData data) : base(data)
    {
        _maxStack = 1;
        _hp = data._installHP;
        _itemType = data._itemType;
    }
}
