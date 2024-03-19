using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Item
{
    public float durability => _durability;
    [SerializeField] private float _durability;
    [SerializeField] private int _toolNumber;
    public Tool(ItemData data) : base(data)
    {
        _maxStack = 1;
        _durability = data._toolDurability;
        _itemType = data._itemType;
    }

    /// <summary> 내구도 감소 함수</summary>
    public void F_UseTool()
    {

    }

    public override void F_UseItem()
    {
        // 도구를 장착시키는 함수를 실행 ( playerManager 또는 playerController )
    }
}
