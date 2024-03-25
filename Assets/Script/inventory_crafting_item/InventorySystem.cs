using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [Header("Drag and Drop")]
    [SerializeField] private Transform _quickTransform;
    [SerializeField] private Transform _slotTransform;
    [SerializeField] private Canvas _canvas;

    [Header("inventory Data")]
    [SerializeField] private int _inventorySize = 28;
    [SerializeField] private Item[] _inventory;
    public Item[] inventory => _inventory;
    public int inventorySize => _inventorySize;

    [Header("Slots")]
    [SerializeField] private List<ItemSlot> _slots;

    [Header("Crafting")]
    [SerializeField] private CraftSystem _craftSystem;

    [Space]
    [Header("Storage")]
    [SerializeField] private GameObject _smallStorage;
    public GameObject smallStorage => _smallStorage;

    [Header("tempData")]
    public int _selectIndex;
    private int _slotIndex;

    private void Awake()
    {
        // 0~7  -> �� ����
        // 8~27 -> �κ��丮 ����

        _slots = new List<ItemSlot>();
        for (int i = 0; i < _quickTransform.childCount; i++)
            _slots.Add(_quickTransform.GetChild(i).GetComponent<ItemSlot>());

        for (int i = 0; i < _slotTransform.childCount; i++)
            _slots.Add(_slotTransform.GetChild(i).GetComponent<ItemSlot>());

        for(int i = 0; i < _slots.Count; i++)
            _slots[i]._slotIndex = i;

        // �κ��丮 �ҷ�����
        SaveManager.Instance.F_LoadInventory(ref _inventory);

        _craftSystem = GetComponent<CraftSystem>();

        F_InventoryUIUpdate();
    }

    private void Update()
    {
        // �κ��丮 �����ϱ�
        if (Input.GetKeyDown(KeyCode.K))
            SaveManager.Instance.F_SaveInventory(ref _inventory);
    }

    #region �κ��丮
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
                F_AddItem(v_code,index);
                return true; // ������ �߰� ����
            }
            if (_inventory[index].F_IsEmpty())
            {
                F_AddItem(v_code, index);
                return true; // ������ �߰� ����
            }
        }
        return false; // ������ �߰� ����
    }

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

    public void F_InventoryUIUpdate()
    {
        //�κ��丮 �迭�� �ִ� �����͸� UI�� ����ϴ� �Լ�
        for(int i = 0; i < _slots.Count; i++)
        {
            if (_inventory[i] == null)
                _slots[i].F_EmptySlot();

            else if (_inventory[i].F_IsEmpty())
            {
                _slots[i].F_EmptySlot();
                _inventory[i] = null;
            }

            else
                _slots[i].F_UpdateSlot(_inventory[i].itemCode, _inventory[i].currentStack);
        }
    }

    public void F_SwapItem(int v_sIndex, int v_eIndex)
    {
        // ���� ��ġ�� ��� ���� X
        if (v_sIndex == v_eIndex)
            return;
        
        // ����ִ� ĭ���� �̵�
        if (inventory[v_eIndex] == null)                    // ���������
        {
            inventory[v_eIndex] = inventory[v_sIndex];
            inventory[v_sIndex] = null;
        }
        else if (inventory[v_eIndex].F_IsEmpty())           // ����ó��
        {
            inventory[v_eIndex] = inventory[v_sIndex];
            inventory[v_sIndex] = null;
        }

        // �ٸ� �������϶� ( ���� )
        else if (!inventory[v_sIndex].F_CheckItemCode(inventory[v_eIndex].itemCode))
        {
            Item tmp_item = inventory[v_eIndex];
            inventory[v_eIndex] = inventory[v_sIndex];
            inventory[v_sIndex] = tmp_item;
        }

        // ���� �������϶� 
        else
        {
            // �����ϴ� �������� maxStack�� 1�� ��� �׳� ���� ( ���� / ��ġ�� )
            if (inventory[v_eIndex].maxStack == 1)
            {
                Item tmp_item = inventory[v_eIndex];
                inventory[v_eIndex] = inventory[v_sIndex];
                inventory[v_sIndex] = tmp_item;
            }
            else
            {
                // 32 - ���罺�� => �� ä��� �ִ� ����
                int canAddStack = inventory[v_eIndex].maxStack - inventory[v_eIndex].currentStack;
                // ä�� ����
                int stack = inventory[v_sIndex].currentStack;

                // ä������ ������ �� ������
                if(stack <= canAddStack)
                {
                    inventory[v_sIndex] = null;
                    inventory[v_eIndex].F_AddStack(stack);
                }
                // ä������ ������ �� ������
                else
                {
                    // sindex�� ������ canAddStack��ŭ �پ��
                    inventory[v_sIndex].F_AddStack(-canAddStack);
                    // eindex�� ������ canAddStack��ŭ �þ
                    inventory[v_eIndex].F_AddStack(canAddStack);
                }
            }
        }
        F_InventoryUIUpdate();
    }

    public void F_DeleteItem()
    {
        if (_selectIndex == -1)
            return;
        inventory[_selectIndex] = null;

        F_InventoryUIUpdate();
        UIManager.Instance.F_SlotFuntionUIOff();
        UIManager.Instance.F_UpdateItemInformation_Empty();

        ItemManager.Instance.F_UpdateItemCounter();
        _craftSystem._craftingDelegate();
    }

    public void F_UseItem(int v_slotNumber)
    {
        if (inventory[v_slotNumber] == null)            // �������� ���� ��� 
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        else if (inventory[v_slotNumber].F_IsEmpty())        // �������� ���� ��� ����ó��
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        else
            inventory[v_slotNumber].F_UseItem();

        F_InventoryUIUpdate();
    }

    public void F_CraftingItem(int v_code, int v_count)
    {
        for(int i = 0; i < inventory.Length; i++)
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
        _craftSystem._craftingDelegate();
    }

    // ������ ����ڵ� �ϳ� ����� (�Ͽ�¡��)



    #endregion
}