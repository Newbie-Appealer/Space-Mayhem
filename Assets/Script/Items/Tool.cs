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

    /// <summary> ������ ���� �Լ�</summary>
    public void F_UseTool()
    {

    }

    public override void F_UseItem()
    {
        // ������ ������Ű�� �Լ��� ���� ( playerManager �Ǵ� playerController )
    }
}
