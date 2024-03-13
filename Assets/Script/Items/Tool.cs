using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Item, UsableItem
{
    public float durability => _durability;
    [SerializeField] private float _durability;
    public Tool(ItemData data) : base(data)
    {
        _maxStack = 1;
        _durability = data._toolDurability;
        _itemType = data._itemType;
    }

    public void F_UseItem()
    {
        // 플레이어의 도구 장착 함수를 호출 
        // 도구 번호를 매개변수로 보내기 
    }
}
