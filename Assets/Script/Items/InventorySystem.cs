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
        for(int i = 0; i < _inventorySize; i++)
        {
            Debug.Log(_inventory[i]);
        }
    }
   

    public bool F_AddItem(Item v_newitem)
    {
        // �������� ������ ( ���� / ��ġ�� )
        if (v_newitem is UnCountableItem)
        {
            // 1. �κ��丮 ����Ž��
            // 2. �� ���� Ž��
            // 3. ������ ���Կ� ������ ��� �� true ��ȯ
            // 4. ������ ���Կ� ������ ��������ʰ� false ��ȯ
        }

        // �����ִ� ������ ( ��� / �Һ� )
        else if (v_newitem is CountableItem)
        {
            // 1. �κ��丮 ����Ž��
            // 2. ������ �������� �ִ��� Ȯ��
            //   - ������ ������ ����++
            //   - �ִ� ������ �Ѿ�� �ٸ� ���� Ž��
            // 3. ������ �������� ������ �� ���� Ž��
            // 4. �󽽷��� ������ ������ ��� �� true ����
            // 5. �󽽷��� ������ ������ ��������ʰ� false ����
        }
        return false;
    }

    public void F_InventoryUIUpdate()
    {
        Food f = _inventory[0] as Food;
    }
}
