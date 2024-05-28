using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Install : Item
{
    public float hp => _hp;
    [SerializeField] private float _hp;
    [SerializeField] private int _installNumber;
    public Install(ItemData data) : base(data)
    {
        _maxStack = 1;
        _hp = data._installHP;
        _itemType = data._itemType;
        
        _playerState = data._playerState;
        _installNumber = data._installCode;
    }
    public override void F_UseItem()
    {
        PlayerManager.Instance.F_ChangeState(_playerState, _installNumber);
        ItemManager.Instance.installSystem.F_GetItemInfo(_installNumber);
    }
}