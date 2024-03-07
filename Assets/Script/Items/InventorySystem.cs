using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int _inventorySize = 28;
    [SerializeField] private Item[] _inventory;

    private void Awake()
    {
        // 0~7  -> �� ����
        // 8~27 -> �κ��丮 ����
        _inventory = new Item[_inventorySize];
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
        Food f = _inventory[0] as Food;
        //�κ��丮 �迭�� �ִ� �����͸� UI�� ����ϴ� �Լ�
    }
}
