using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

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

    [Header("Slots")]
    [SerializeField] private List<ItemSlot> _slots;

    private int _slotIndex;
    [Header("tempData")]
    public int _selectIndex;
    private void Awake()
    {
        // 0~7  -> �� ����
        // 8~27 -> �κ��丮 ����
        _inventory = new Item[_inventorySize];
        _slots = new List<ItemSlot>();

        for (int i = 0; i < _quickTransform.childCount; i++)
            _slots.Add(_quickTransform.GetChild(i).GetComponent<ItemSlot>());

        for (int i = 0; i < _slotTransform.childCount; i++)
            _slots.Add(_slotTransform.GetChild(i).GetComponent<ItemSlot>());

        for(int i = 0; i < _slots.Count; i++)
            _slots[i]._slotIndex = i;

        F_InventoryUIUpdate();
    }

    public bool F_GetItem(int v_code)
    {
        // ������ ������ /
        for(int index = 0; index < _inventorySize; index++)
        {
            if (_inventory[index] == null)   // ��ĭ
                continue;

            // ������ ������ + ������ ���� �߰� �����Ҷ�
            if (_inventory[index].F_CheckItemCode(v_code) && _inventory[index].F_CheckStack())
            {
                _inventory[index].F_AddStack(1);
                Debug.Log("Get : " + _inventory[index].itemName);
                return true;
            }
        }

        // ������ �������� ������
        for(int index = 0; index < _inventorySize; index++)
        {
            if (_inventory[index] == null)    // ��ĭ �϶�
            {
                ItemData data = ItemManager.Instance.ItemDatas[v_code];
                switch(data._itemType)
                {
                    case ItemType.STUFF:
                        _inventory[index] = new Stuff(data);
                        break;

                    case ItemType.FOOD:
                        _inventory[index] = new Food(data);
                        break;

                    case ItemType.TOOL:
                        _inventory[index] = new Tool(data);
                        break;

                    case ItemType.INSTALL:
                        _inventory[index] = new Install(data);
                        break;
                }
                Debug.Log("Get : " + _inventory[index].itemName);
                return true; // ������ �߰� ����
            }
        }
        return false; // ������ �߰� ����
    }

    public void F_InventoryUIUpdate()
    {
        //�κ��丮 �迭�� �ִ� �����͸� UI�� ����ϴ� �Լ�
        for(int i = 0; i < _slots.Count; i++)
        {
            if (_inventory[i] == null)
                _slots[i].F_EmptySlot();
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
        if (inventory[v_eIndex] == null)
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
    }

    public void F_UseItem(int v_slotNumber)
    {
        
    }
}