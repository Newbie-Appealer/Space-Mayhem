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
        // 0~7  -> 퀵 슬롯
        // 8~27 -> 인벤토리 슬롯
        inventory = new Item[inventorySize];
    }
    public void AddItem(Item v_newitem)
    {
        // 셀수없는 아이템 ( 도구 / 설치류 )
        if (v_newitem is UnCountableItem)
        {
            // 인벤토리에서       
        }

        // 셀수있는 아이템 ( 재료 / 소비 )
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
