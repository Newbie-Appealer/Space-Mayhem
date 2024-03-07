using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int _inventorySize = 28;
    [SerializeField] private Item[] _inventory;
    [SerializeField] private Transform _slotTransform;
    [SerializeField] private List<ItemSlot> _slots;

    private void Awake()
    {
        // 0~7  -> �� ����
        // 8~27 -> �κ��丮 ����
        _inventory = new Item[_inventorySize];
        _slots = new List<ItemSlot>();

        for(int i = 0; i < _slotTransform.childCount; i++)
        {
            _slots.Add(_slotTransform.GetChild(i).GetComponent<ItemSlot>());
        }
    }
   

    public bool F_AddItem(Item v_newitem)
    {
        // �������� ������ ( ���� / ��ġ�� )
        if (v_newitem is UnCountableItem)
        {
            for(int index = 0; index < _inventorySize; index++) // �κ��丮 Ž��
            {
                if(_inventory[index] == null)                   // �� ������ ������� �κ��丮�� ������ ����
                {
                    _inventory[index] = v_newitem;
                    return true;                                // �κ��丮 ������ �ֱ� ����
                }
            }
        }

        // �����ִ� ������ ( ��� / �Һ� )
        else if (v_newitem is CountableItem)
        {
            for(int index = 0; index < _inventorySize; index++)                     // �κ��丮 Ž��
            {
                if (_inventory[index] == null)                                      // ����ִ� ĭ �Ѿ
                    continue;

                if (!_inventory[index].F_CheckItemCode(v_newitem))                  // ������ �������� �ƴ϶�� �Ѿ
                    continue;
                                                                                    // ������ �������� ���
                if ((_inventory[index] as CountableItem).F_CheckItemStack())        // ���� ������ ������ Ȯ���ϰ�, �������°� �ƴ϶�� true�� ��ȯ��
                {
                    _inventory[index].F_AddStack(v_newitem.itemdata.itemStack);     // �������°� �ƴҶ� ���� ������ ������.
                    return true;                                                    // ������ �ֱ� ����
                }
            }
            for(int index = 0; index < _inventorySize; index++)                     // �κ��丮 Ž��
            {
                if(_inventory[index] == null)                                       // �� ������ ������� �κ��丮�� ������ ����
                {
                    _inventory[index] = v_newitem;                                  
                    return true;                                                    // �κ��丮 ������ �ֱ� ����
                }
            }
        }
        return false;                                                               // �κ��丮 ������ �ֱ� ����
    }

    // TODO:�κ��丮���
    // ������ �߰� ---- ��

    // ������ ����/�̵� ----
    // ������ ���� ----
    // ������ ��� ----
    // ��� �߰��ؾ���.

    public void F_InventoryUIUpdate()
    {
        //�κ��丮 �迭�� �ִ� �����͸� UI�� ����ϴ� �Լ�
        for(int i = 0; i < _slots.Count; i++)
        {
            if (_inventory[i] == null)
                continue;

            _slots[i].UpdateSlost(_inventory[i].itemdata.itemCode, _inventory[i].itemdata.itemStack);
        }
    }
}
