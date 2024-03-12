using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item, UsableItem
{
    public float foodValue => _foodValue;
    [SerializeField] private float _foodValue;
    public Food(ItemData data) : base(data) 
    {
        _maxStack = 32;
        _foodValue = data._foodValue;
        _itemType = data._itemType;
    }

    public void F_UseItem()
    {
        // 플레이어의 허기 수치를 회복시키기.
    }
}
