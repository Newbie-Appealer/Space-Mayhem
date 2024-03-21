using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Item
{
    public float durability => _durability;
    [SerializeField] private float _durability;
    [SerializeField] private int _toolCode;

    public Tool(ItemData data) : base(data)
    {
        _maxStack = 1;
        _durability = data._toolDurability;
        _itemType = data._itemType;

        _toolCode = data._toolCode;
        _playerState = data._playerState;
    }

    public void F_InitDurability(float value)
    {
        _durability = value;
    }

    /// <summary> 내구도 감소 함수</summary>
    public void F_UseTool()
    {

    }

    public override void F_UseItem()
    {
        PlayerManager.Instance.F_ChangeState(_playerState, _toolCode);
    }
}
