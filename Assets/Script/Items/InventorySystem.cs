using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : Singleton<InventorySystem>
{
    [SerializeField] private int inventorySize = 28;
    [SerializeField] private Item[] inventory;


    protected override void InitManager()
    {
        inventory = new Item[inventorySize];
    }

    public void AddItem(Item v_item)
    {
        for(int i = 0; i < inventorySize; i++)
        {
            if (inventory[i].id == 0)
                inventory[i] = v_item;
        }
    }

}
