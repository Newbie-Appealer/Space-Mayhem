using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    public float foodValue => _foodValue;
    [SerializeField] private float _foodValue;
    public Food(ItemData data) : base(data) 
    {
        _maxStack = 32;
        _foodValue = data._foodValue;
        _itemType = data._itemType;

        _playerState = data._playerState;
    }
    public override void F_UseItem()
    {
        Debug.Log("음식 아이템 사용");
        //PlayerManager.Instance.F_HealHunger();  // 매개변수로 _foodValue 넘기기
        F_AddStack(-1);
    }
}
