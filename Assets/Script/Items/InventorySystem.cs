using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int inventorySize = 28;
    [SerializeField] private Item[] inventory;

    private void Awake()
    {
        // 0~7  -> �� ����
        // 8~27 -> �κ��丮 ����
        inventory = new Item[inventorySize];
    }
    public void AddItem(Item v_newitem)
    {
        // �������� ������ ( ���� / ��ġ�� )
        if (v_newitem is UnCountableItem)
        {
            // �κ��丮����       
        }

        // �����ִ� ������ ( ��� / �Һ� )
        else if (v_newitem is CountableItem)
        {
            Debug.Log("is Countable");
        }


        //if (v_newitem is Stuff)
        //else if(v_newitem is Food)
        //else if (v_newitem is Tool)
        //else if (v_newitem is Install)
    }
}
