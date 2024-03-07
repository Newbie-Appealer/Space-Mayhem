using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int inventorySize = 28;
    [SerializeField] private Item[] inventory;

    private void Start()
    {
        inventory = new Item[inventorySize];
    }

    public void AddItem(Item v_newitem)
    {

    }
}
