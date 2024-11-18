using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Item
{
    public float durability => _durability;
    [SerializeField] private float _durability;
    [SerializeField] private int _toolCode;

    private float _maxDurability;

    public float durabilityAmount => durability / _maxDurability;

    public Tool(ItemData data) : base(data)
    {
        _maxStack = 1;

        _durability = data._toolDurability;
        _maxDurability = _durability;

        _itemType = data._itemType;

        _toolCode = data._toolCode;
        _playerState = data._playerState;

    }

    /// <summary> ������ �ʱ�ȭ �Լ� </summary>
    public void F_InitDurability(float value)
    {
        _durability = value;
    }

    /// <summary> ������ ���� �Լ�</summary>
    public void F_UseDurability()
    {
        // ���� �������� �����ִ� UI�� �߰��ؾ��ҵ�
        _durability -= 1.0f;
    }

    /// <summary> 
    /// ���� ������ Ȯ�� �Լ� 
    /// ������ �����ϸ� ������ ����
    /// </summary>
    public void F_CheckDurability()
    {
        // ���� �������� ������ ������ �ı��ϱ�.
        if (_durability <= 0.0f)
        {
            int idx = ItemManager.Instance.inventorySystem.selectQuickSlotNumber;   // ���� ���õ� ������ ��ȣ
            ItemManager.Instance.inventorySystem.inventory[idx] = null;             // ������ ���� ( �ı� )
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);             // ���º�ȯ
            UIManager.Instance.F_QuickSlotFocus(-1);                                // ������ ��Ŀ�� ����
        }

        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();             // �κ��丮 ������Ʈ ( UI )
    }

    /// <summary> �����Կ��� �������� ���õ����� </summary>
    public override void F_UseItem()
    {
        // ���� �ٲٱ�
        PlayerManager.Instance.F_ChangeState(_playerState, _toolCode);
    }

}
