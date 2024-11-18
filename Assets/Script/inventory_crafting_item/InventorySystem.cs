using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class InventorySystem : MonoBehaviour
{
    [Header("Drag and Drop")]
    [SerializeField] private Transform _quickTransform;
    [SerializeField] private Transform _slotTransform;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _smallStorage;
    [SerializeField] private Transform _bigStorage;

    public Transform smallStorage => _smallStorage;
    public Transform bigStorage => _bigStorage;

    [Header("inventory Data")]
    [SerializeField] private int _inventorySize = 28;
    [SerializeField] private Item[] _inventory;
    public Item[] inventory => _inventory;
    public int inventorySize => _inventorySize;

    [Header("Slots")]
    [SerializeField] private List<ItemSlot> _slots;
    [SerializeField] private int _selectQuickSlotNumber;
    public int selectQuickSlotNumber => _selectQuickSlotNumber;
    [Header("Crafting")]
    [SerializeField] private CraftSystem _craftSystem;


    [Header("tempData")]
    public int _selectIndex;
    public SlotType _selectSlotType;

    private void Awake()
    {
        // SaveManager�� save��������Ʈ�� save���
        SaveManager.Instance.GameDataSave += () => SaveManager.Instance.F_SaveInventory(ref _inventory);

        // 1. �κ��丮 �ҷ�����
        SaveManager.Instance.F_LoadInventory(ref _inventory);

        // 2. ���� �ʱ�ȭ
        // 0~7  -> �� ���� 8~27 -> �κ��丮 ����
        _slots = new List<ItemSlot>();
        for (int i = 0; i < _quickTransform.childCount; i++)
            _slots.Add(_quickTransform.GetChild(i).GetComponent<ItemSlot>());
        for (int i = 0; i < _slotTransform.childCount; i++)
            _slots.Add(_slotTransform.GetChild(i).GetComponent<ItemSlot>());
        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i]._slotIndex = i;
            _slots[i].itemSlotRef = _inventory;
            _slots[i]._slotType = SlotType.INVENTORY;
        }

        _craftSystem = GetComponent<CraftSystem>();

        F_InventoryUIUpdate();

        // Storage Slot �ʱ�ȭ
        for (int i = 0; i < _smallStorage.GetChild(0).childCount; i++)
        {
            ItemSlot slot = _smallStorage.GetChild(0).GetChild(i).GetComponent<ItemSlot>();
            slot._slotIndex = i;
            slot._slotType = SlotType.STORAGE;
        }

        for (int i = 0; i < _bigStorage.GetChild(0).childCount; i++)
        {
            ItemSlot slot = _bigStorage.GetChild(0).GetChild(i).GetComponent<ItemSlot>();
            slot._slotIndex = i;
            slot._slotType = SlotType.STORAGE;
        }
    }
    #region �κ��丮
    /// <summary> ������ ȹ�� �õ� �Լ�( ���� ���� ��ȯ )</summary>
    public bool F_GetItem(int v_code)
    {
        // ������ ������ /
        for(int index = 0; index < _inventorySize; index++)
        {
            if (_inventory[index] == null)   // ��ĭ
                continue;
            if (_inventory[index].F_IsEmpty())  // ��ĭ ���� ����ó��
                continue;

            // ������ ������ + ������ ���� �߰� �����Ҷ�
            if (_inventory[index].F_CheckItemCode(v_code) && _inventory[index].F_CheckStack())
            {
                _inventory[index].F_AddStack(1);
                return true;
            }
        }

        // ������ �������� ������
        for(int index = 0; index < _inventorySize; index++)
        {
            if (_inventory[index] == null)
            {
                F_AddItem(v_code, index);
                return true; // ������ �߰� ����
            }
            if (_inventory[index].F_IsEmpty())
            {
                F_AddItem(v_code, index);
                return true; // ������ �߰� ����
            }
        }

        UIManager.Instance.F_PlayerMessagePopupTEXT("�κ��丮�� ���� á���ϴ�.");
        return false; // ������ �߰� ����
    }

    /// <summary> �κ��丮�� �������� �߰��ϴ� �Լ�</summary>
    /// <param name="v_code">�߰��� ������ ��ȣ</param>
    /// <param name="v_index">�������� �κ��丮�� �߰��� ��ġ</param>
    public void F_AddItem(int v_code, int v_index)
    {
        ItemData data = ItemManager.Instance.ItemDatas[v_code];

        switch (data._itemType)
        {
            case ItemType.STUFF:
                _inventory[v_index] = new Stuff(data);
                break;

            case ItemType.FOOD:
                _inventory[v_index] = new Food(data);
                break;

            case ItemType.TOOL:
                _inventory[v_index] = new Tool(data);
                break;

            case ItemType.INSTALL:
                _inventory[v_index] = new Install(data);
                break;
        }
    }

    /// <summary> 
    /// �κ��丮 UI ������Ʈ 
    /// �κ��丮 �迭�� �����͸� UI�� ����ϴ� �Լ�
    /// </summary>
    public void F_InventoryUIUpdate()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_inventory[i] == null)
                _slots[i].F_EmptySlot();

            else if (_inventory[i].F_IsEmpty())
            {
                _slots[i].F_EmptySlot();
                _inventory[i] = null;
            }

            else
                _slots[i].F_UpdateSlot(_inventory[i].itemCode, _inventory[i].currentStack, i);
        }
    }

    /// <summary> ItemSlot�� ������ ���� �Լ� </summary>
    public void F_SwapItem(int v_sIndex, int v_eIndex,ref Item[] v_from,ref Item[] v_target)
    {
        Item[] from = v_from;
        Item[] target = v_target;

        // ���� �迭 ���� && ���� ��ġ => ����ó��
        if (from == target && v_sIndex == v_eIndex)
            return;
        
        // ����ִ� ĭ ã��
        if (target[v_eIndex] == null)
        {
            target[v_eIndex] = from[v_sIndex];
            from[v_sIndex] = null;
        }

        // ����ִ� ĭ ã��
        else if (target[v_eIndex].F_IsEmpty())
        {
            target[v_eIndex] = from[v_sIndex];
            from[v_sIndex] = null;
        }

        // �ٸ� �������϶� ( ���� )
        else if (!from[v_sIndex].F_CheckItemCode(target[v_eIndex].itemCode))
        {
            Item tmp_item = target[v_eIndex];
            target[v_eIndex] = from[v_sIndex];
            from[v_sIndex] = tmp_item;
        }

        // ���� �������϶� 
        else
        {
            // �����ϴ� �������� maxStack�� 1�� ��� �׳� ���� ( ���� / ��ġ�� )
            if (target[v_eIndex].maxStack == 1)
            {
                Item tmp_item = target[v_eIndex];
                target[v_eIndex] = from[v_sIndex];
                from[v_sIndex] = tmp_item;
            }
            else
            {
                // 32 - ���罺�� => �� ä��� �ִ� ����
                int canAddStack = target[v_eIndex].maxStack - target[v_eIndex].currentStack;
                // ä�� ����
                int stack = from[v_sIndex].currentStack;

                // ä������ ������ �� ������
                if(stack <= canAddStack)
                {
                    from[v_sIndex] = null;
                    target[v_eIndex].F_AddStack(stack);
                }
                // ä������ ������ �� ������
                else
                {
                    // sindex�� ������ canAddStack��ŭ �پ��
                    from[v_sIndex].F_AddStack(-canAddStack);
                    // eindex�� ������ canAddStack��ŭ �þ
                    target[v_eIndex].F_AddStack(canAddStack);
                }
            }
        }

        F_InventoryUIUpdate();
        ItemManager.Instance.F_UpdateItemCounter();             // ������ ���� ������Ʈ
        if(ItemManager.Instance.selectedStorage != null)
        {
            ItemManager.Instance.selectedStorage.F_StorageUIUpdate();
        }
    }

    /// <summary> ������ ���� �Լ�</summary>
    public void F_DeleteItem()
    {
        SoundManager.Instance.F_PlaySFX(SFXClip.CLICK4);

        if (_selectIndex == -1 || _selectSlotType == SlotType.NONE)
            return;

        switch(_selectSlotType)
        {
            case SlotType.STORAGE:
                ItemManager.Instance.selectedStorage.items[_selectIndex] = null;    // ������ ����
                ItemManager.Instance.selectedStorage.F_StorageUIUpdate();           // â�� ��Ȳ ������Ʈ
                break;
            case SlotType.INVENTORY:
                inventory[_selectIndex] = null;                         // ������ ����
                F_InventoryUIUpdate();                                  // �κ��丮 ��Ȳ ������Ʈ
                ItemManager.Instance.F_UpdateItemCounter();             // ������ ���� ������Ʈ
                _craftSystem._craftingDelegate();                       // ���� ���� ������Ʈ
                break;
        }

        UIManager.Instance.F_UpdateItemInformation_Empty();     // ������ ���� �ʱ�ȭ
        UIManager.Instance.F_SlotFuntionUIOff();                // ������ ���� UI ����
    }

    /// <summary> ������ ���� �Լ�</summary>
    public void F_DivisionItem()
    {
        SoundManager.Instance.F_PlaySFX(SFXClip.CLICK4);

        if (_selectIndex == -1 || _selectSlotType == SlotType.NONE)
            return;

        switch(_selectSlotType)
        {
            case SlotType.STORAGE:
                ItemManager.Instance.selectedStorage.F_DivisionStorageItem(_selectIndex);            
                break;
            case SlotType.INVENTORY:
                F_DivisionInventoryItem();
                break;
        }
    }

    /// <summary> �κ��丮 �������� �����ϴ� �Լ�</summary>
    public void F_DivisionInventoryItem()
    {
        int stack = _inventory[_selectIndex].currentStack;
        if (stack == 1)
            return;

        int decreaseStack = stack / 2;
        int increaseStack = stack / 2;

        int itemCode = _inventory[_selectIndex].itemCode;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (_inventory[i] == null)   // ��ĭ
                F_AddItem(itemCode, i);

            else if (_inventory[i].F_IsEmpty())  // ��ĭ ���� ����ó��
                F_AddItem(itemCode, i);

            else
                continue;

            _inventory[_selectIndex].F_AddStack(-decreaseStack);
            _inventory[i].F_AddStack(increaseStack - 1);

            UIManager.Instance.F_SlotFuntionUIOff();                // ������ ���� UI ����
            F_InventoryUIUpdate();                                  // �κ��丮 UI ������Ʈ
            break;
        }
    }

    /// <summary> ������ ������ ��� �Լ� </summary>
    /// <param name="v_slotNumber"> ������ ��ȣ</param>
    public void F_UseItem(int v_slotNumber)
    {
        _selectQuickSlotNumber = v_slotNumber;
        PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        // -1�� �Ű������� �־ F_UseItem�� �����Ű��  �ƹ��͵� �������� ���·�.
        if (_selectQuickSlotNumber == -1)       
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        if (inventory[_selectQuickSlotNumber] == null)            // �������� ���� ��� 
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        else if (inventory[_selectQuickSlotNumber].F_IsEmpty())        // �������� ���� ��� ����ó��
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        else
            inventory[_selectQuickSlotNumber].F_UseItem();

        UIManager.Instance.F_QuickSlotFocus(_selectQuickSlotNumber);
        F_InventoryUIUpdate();
    }

    // ����� ������ �κ����� ���� ����
    // �������� �κ��丮�� �����Ѵٴ� �����Ͽ�
    public void F_UpdateItemUsing(int v_code, int v_count ) 
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
                continue;
            if (inventory[i].F_IsEmpty())
                continue;
            if (inventory[i].itemCode != v_code)
                continue;
            // ���� ������ �������� ������ �ʿ䰳�� �̻��϶�
            if (inventory[i].currentStack >= v_count)
            {
                inventory[i].F_AddStack(-v_count);

                // ������ �����۰����� 0���ϰ� �Ǹ� ������ ���� ����.
                if (inventory[i].F_IsEmpty())
                    inventory[i] = null;

                break;
            }

            // ������ ������ ������ �����Ҷ�.
            else
            {
                // ������ ������ŭ v_count���� �����ϰ� ���� ����
                v_count -= inventory[i].currentStack;
                inventory[i] = null;
            }
        }

        F_InventoryUIUpdate();
        ItemManager.Instance.F_UpdateItemCounter();
    }

    // craft �� ������ �Ҹ��ڵ�
    public void F_CraftingItem(int v_code, int v_count)
    {
        F_UpdateItemUsing(v_code, v_count);
        _craftSystem._craftingDelegate();
    }
    #endregion
}