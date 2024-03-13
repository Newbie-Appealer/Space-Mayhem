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
        // �÷��̾��� ���� ���� �Լ��� ȣ�� 
        // ���� ��ȣ�� �Ű������� ������ 
    }
}
