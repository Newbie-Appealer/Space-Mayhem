using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [Header("Storage Infomation")]
    [SerializeField] private int _storageSize;                  // ���丮�� ũ��
    [SerializeField] private List<ItemSlot> _slots;             // ����

    [Header("Storage itemData")]
    [SerializeField] private Item[] _items;                     // ���丮�� ������ ����
    public Item[] items => _items;

    [Space]
    [SerializeField] private Transform _storageSlotTransform;                // Slot Parent
    [SerializeField] private InventorySystem _inventorySystem;
    private void Start()
    {
        _items = new Item[_storageSize]; 
        _slots = new List<ItemSlot>();
        _inventorySystem = ItemManager.Instance.inventorySystem;
        _storageSlotTransform = _inventorySystem.smallStorage;

        // �ش� ���丮���� ����� ������ ������ �ʱ�ȭ
        for (int i = 0; i < _storageSize; i++)
            _slots.Add(_storageSlotTransform.GetChild(i).GetComponent<ItemSlot>());
    }

    /// <summary> ���� ���� / ���뿡 ��ȭ�� ������ ����ϱ�</summary>
    public void F_StorageUIUpdate()
    {
        for(int i = 0; i < _slots.Count; i++)
        {
            if (_items[i] == null)
                _slots[i].F_EmptySlot();
            else if (_items[i].F_IsEmpty())
            {
                _slots[i].F_EmptySlot();
                _items[i] = null;
            }
            else
            {
                _slots[i].F_UpdateSlot(_items[i].itemCode, _items[i].currentStack);
            }   
        }
    }

    public void F_OpenStorage()
    {
        // 1. ItemManager���� ���õ� ���丮���� ������Ʈ.
        ItemManager.Instance.F_SelectStorage(this);

        // 2. â�� ������ ���� �ֽ�ȭ
        F_StorageUIUpdate();

        // 3. â��/�κ��丮 UI Ȱ��ȭ    --> UIManager
        // 4. ���� UI ��Ȱ��ȭ           --> UIManager
    }

    /// <summary> â���� â��� ������ ���� �Լ� </summary>
    public void F_SwapItem(int v_sIndex, int v_eIndex)
    {
        // ���� ��ġ�� ��� ���� X
        if (v_sIndex == v_eIndex)
            return;

        // ����ִ� ĭ���� �̵�
        if (_items[v_eIndex] == null)                    // ���������
        {
            _items[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = null;
        }
        else if (_items[v_eIndex].F_IsEmpty())           // ����ó��
        {
            _items[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = null;
        }

        // �ٸ� �������϶� ( ���� )
        else if (!_items[v_sIndex].F_CheckItemCode(_items[v_eIndex].itemCode))
        {
            Item tmp_item = _items[v_eIndex];
            _items[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = tmp_item;
        }

        // ���� �������϶� 
        else
        {
            // �����ϴ� �������� maxStack�� 1�� ��� �׳� ���� ( ���� / ��ġ�� )
            if (_items[v_eIndex].maxStack == 1)
            {
                Item tmp_item = _items[v_eIndex];
                _items[v_eIndex] = _items[v_sIndex];
                _items[v_sIndex] = tmp_item;
            }
            else
            {
                // 32 - ���罺�� => �� ä��� �ִ� ����
                int canAddStack = _items[v_eIndex].maxStack - _items[v_eIndex].currentStack;
                // ä�� ����
                int stack = _items[v_sIndex].currentStack;

                // ä������ ������ �� ������
                if (stack <= canAddStack)
                {
                    _items[v_sIndex] = null;
                    _items[v_eIndex].F_AddStack(stack);
                }
                // ä������ ������ �� ������
                else
                {
                    // sindex�� ������ canAddStack��ŭ �پ��
                    _items[v_sIndex].F_AddStack(-canAddStack);
                    // eindex�� ������ canAddStack��ŭ �þ
                    _items[v_eIndex].F_AddStack(canAddStack);
                }
            }
        }
        F_StorageUIUpdate();
    }

    /// <summary> â���� �κ��丮�� ������ ���� �Լ� </summary>
    public void F_SwapItemToInven(int v_sIndex, int v_eIndex)
    {
        // ���� ��ġ�� ��� ���� X
        if (v_sIndex == v_eIndex)
            return;

        // ����ִ� ĭ���� �̵�
        if (_inventorySystem.inventory[v_eIndex] == null)                    // ���������
        {
            _inventorySystem.inventory[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = null;
        }
        else if (_inventorySystem.inventory[v_eIndex].F_IsEmpty())           // ����ó��
        {
            _inventorySystem.inventory[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = null;
        }

        // �ٸ� �������϶� ( ���� )
        else if (!_items[v_sIndex].F_CheckItemCode(_inventorySystem.inventory[v_eIndex].itemCode))
        {
            Item tmp_item = _inventorySystem.inventory[v_eIndex];
            _inventorySystem.inventory[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = tmp_item;
        }

        // ���� �������϶� 
        else
        {
            // �����ϴ� �������� maxStack�� 1�� ��� �׳� ���� ( ���� / ��ġ�� )
            if (_inventorySystem.inventory[v_eIndex].maxStack == 1)
            {
                Item tmp_item = _inventorySystem.inventory[v_eIndex];
                _inventorySystem.inventory[v_eIndex] = _items[v_sIndex];
                _items[v_sIndex] = tmp_item;
            }
            else
            {
                // 32 - ���罺�� => �� ä��� �ִ� ����
                int canAddStack = _inventorySystem.inventory[v_eIndex].maxStack - _inventorySystem.inventory[v_eIndex].currentStack;
                // ä�� ����
                int stack = _items[v_sIndex].currentStack;

                // ä������ ������ �� ������
                if (stack <= canAddStack)
                {
                    _items[v_sIndex] = null;
                    _inventorySystem.inventory[v_eIndex].F_AddStack(stack);
                }
                // ä������ ������ �� ������
                else
                {
                    // sindex�� ������ canAddStack��ŭ �پ��
                    _items[v_sIndex].F_AddStack(-canAddStack);
                    // eindex�� ������ canAddStack��ŭ �þ
                    _inventorySystem.inventory[v_eIndex].F_AddStack(canAddStack);
                }
            }
        }

        F_StorageUIUpdate();
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
    }

}
