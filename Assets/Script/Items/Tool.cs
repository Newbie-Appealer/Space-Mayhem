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
        Debug.Log(" 도구의 아이템 사용은 아직 구현되지 않았다! ");
        // 도구의 번호를 매개변수로 보내서
        // 플레이어의 도구 장착 함수를 호출합시다
    }
}
