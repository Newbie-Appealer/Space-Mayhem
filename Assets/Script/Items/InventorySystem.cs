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
            // 1. �κ��丮 Ž��
            // 2. �� ���� Ž��
            // 3. ������ ���Կ� ������ ��� �� true ��ȯ
            // 4. ������ ���Կ� ������ ��������ʰ� false ��ȯ
            for(int index = 0; index < _inventorySize; index++)
            {
                if(_inventory[index] == null)
                {
                    _inventory[index] = v_newitem;
                    return true;
                }
            }
        }

        // �����ִ� ������ ( ��� / �Һ� )
        else if (v_newitem is CountableItem)
        {
            // 1. �κ��丮 Ž��
            // 2. ������ �������� �ִ��� Ȯ��
            //   - ������ ������ ����++
            //   - �ִ� ������ �Ѿ�� �ٸ� ���� Ž��
            for(int index = 0; index < _inventorySize; index++)
            {
                if (_inventory[index] == null)
                    continue;

                if (!_inventory[index].F_CheckItemCode(v_newitem))
                    continue;

                if ((_inventory[index] as CountableItem).F_CheckItemStack())
                {
                    _inventory[index].F_AddStack(v_newitem.itemdata.itemStack);
                    return true;
                }
            }
            // 3. ������ �������� ������ �� ���� Ž��
            // 4. �󽽷��� ������ ������ ��� �� true ����
            // 5. �󽽷��� ������ ������ ��������ʰ� false ����
            for(int index = 0; index < _inventorySize; index++)
            {
                if(_inventory[index] == null)
                {
                    _inventory[index] = v_newitem;
                    return true;
                }
            }
        }
        return false;
    }

    public void F_InventoryUIUpdate()
    {
        Food f = _inventory[0] as Food;
    }
}
