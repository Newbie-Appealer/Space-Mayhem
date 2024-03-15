using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Install : Item
{
    public float hp => _hp;
    [SerializeField] private float _hp;
    [SerializeField] private string _installationNumber;
    public Install(ItemData data) : base(data)
    {
        _maxStack = 1;
        _hp = data._installHP;
        _itemType = data._itemType;
    }
    public override void F_UseItem()
    {
        return;
    }
}
