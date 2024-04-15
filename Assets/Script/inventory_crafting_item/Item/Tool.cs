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
    public void F_UseDurability()
    {
        // 남은 내구도를 보여주는 UI를 추가해야할듯
        _durability -= 1.0f;
    }

    public void F_CheckDurability()
    {
        // 남은 내구도가 없을때 아이템 파괴하기.
        if (_durability <= 0.0f)
        {
            int idx = ItemManager.Instance.inventorySystem.selectQuickSlotNumber;   // 현재 선택된 아이템 ( 도구 )
            ItemManager.Instance.inventorySystem.inventory[idx] = null;             // 아이템 삭제 ( 파괴 )
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();             // 인벤토리 업데이트 ( UI )
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);             // 상태변환
            UIManager.Instance.F_QuickSlotFocus(-1);                                // 아이템 포커스 해제
        }
    }

    public override void F_UseItem()
    {
        PlayerManager.Instance.F_ChangeState(_playerState, _toolCode);
    }
}
