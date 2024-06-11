using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    [SerializeField] private float _foodValue;
    [SerializeField] HealType _healType;
    public float foodValue => _foodValue;
    public Food(ItemData data) : base(data) 
    {
        _maxStack = 32;
        _foodValue = data._foodValue;
        _itemType = data._itemType;

        _playerState = data._playerState;
        _healType = data._healType;
    }
    public override void F_UseItem()
    {
        if (_healType == HealType.NONE)
            return;

        PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);
        F_AddStack(-1);
        PlayerManager.Instance.F_HealState(_healType, _foodValue);

        //플레이어 따봉 발사
        PlayerManager.Instance.PlayerController.F_LeftGoodMotion();
        PlayerManager.Instance.PlayerController.F_LeftGoodMotionEnd();
    }
}
