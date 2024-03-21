using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuff : Item
{
    public Stuff(ItemData data) : base(data)
    {
        _maxStack = 32;
        _itemType = data._itemType;

        _playerState = data._playerState;
    }

    public override void F_UseItem() 
    {
        base.F_UseItem();
    }
}
