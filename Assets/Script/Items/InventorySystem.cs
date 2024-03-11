using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class InventorySystem : MonoBehaviour
{
    [Header("Drag and Drop")]
    [SerializeField] private Transform _quickTransform;
    [SerializeField] private Transform _slotTransform;

    [Header("inventory Data")]
    [SerializeField] private int _inventorySize = 28;
    [SerializeField] private Item[] _inventory;

    [Header("Slots")]
    [SerializeField] private List<ItemSlot> _slots;

    int _beginSlotIndex;
    int _endSlotIndex;
    
    int _slotIndex;
    private void Awake()
    {
        _slotIndex = 0;
        // 0~7  -> �� ����
        // 8~27 -> �κ��丮 ����
        _inventory = new Item[_inventorySize];
        _slots = new List<ItemSlot>();

        for (int i = 0; i < _quickTransform.childCount; i++)
        {
            _slots.Add(_quickTransform.GetChild(i).GetComponent<ItemSlot>());
            _slots[i]._slotIndex = _slotIndex++;
        }

        for (int i = 0; i < _slotTransform.childCount; i++)
        {
            _slots.Add(_slotTransform.GetChild(i).GetComponent<ItemSlot>());
            _slots[i]._slotIndex = _slotIndex++;
        }
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
                _inventory[index].F_AddStack();
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
                _slots[i].F_UpdateSlost(_inventory[i].itemCode, _inventory[i].currentStack);
        }
    }

    // TODO:�κ��丮���
    // ������ �߰� ---- ��
    // ������ ����/�̵� ----
    // �̵�/���� -> ���콺 �巡�׸� ������ ������ ��ȣ�� ���콺 �巡�׸� ����� ������ ��ȣ
    // ������ ���� ----
    // ������ ��� ----
    // ��� �߰��ؾ���.

}