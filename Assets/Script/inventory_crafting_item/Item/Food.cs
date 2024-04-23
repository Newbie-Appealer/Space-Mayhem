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
        PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);
        F_AddStack(-1);
        PlayerManager.Instance.F_HealHunger(_foodValue);                // 회복
        UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.HUNGER); // UI 업데이트
    }
}
