using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    [Header("QuickSlot")]
    private KeyCode _quick_1 = KeyCode.Alpha1;
    private KeyCode _quick_2 = KeyCode.Alpha2;
    private KeyCode _quick_3 = KeyCode.Alpha3;
    private KeyCode _quick_4 = KeyCode.Alpha4;
    private KeyCode _quick_5 = KeyCode.Alpha5;
    private KeyCode _quick_6 = KeyCode.Alpha6;
    private KeyCode _quick_7 = KeyCode.Alpha7;
    private KeyCode _quick_8 = KeyCode.Alpha8;

    [Header("UI")]
    private KeyCode _invetory1 = KeyCode.I;
    private KeyCode _invetory2 = KeyCode.Tab;
    private KeyCode _pause = KeyCode.Escape;

    private void Update()
    {
        F_InputUI();
        F_InputQuickSlot();
    }

    void F_InputUI()
    {
        if(Input.GetKeyDown(_invetory1) || Input.GetKeyDown(_invetory2))
        {
            UIManager.Instance.inventoryUI();
        }
    }

    void F_InputQuickSlot()
    {
        if(Input.GetKeyDown(_quick_1))
            ItemManager.Instance.inventorySystem.F_UseItem(0);

        else if(Input.GetKeyDown(_quick_2))
            ItemManager.Instance.inventorySystem.F_UseItem(1);

        else if (Input.GetKeyDown(_quick_3))
            ItemManager.Instance.inventorySystem.F_UseItem(2);

        else if (Input.GetKeyDown(_quick_4))
            ItemManager.Instance.inventorySystem.F_UseItem(3);

        else if (Input.GetKeyDown(_quick_5))
            ItemManager.Instance.inventorySystem.F_UseItem(4);

        else if (Input.GetKeyDown(_quick_6))
            ItemManager.Instance.inventorySystem.F_UseItem(5);

        else if (Input.GetKeyDown(_quick_7))
            ItemManager.Instance.inventorySystem.F_UseItem(6);

        else if (Input.GetKeyDown(_quick_8))
            ItemManager.Instance.inventorySystem.F_UseItem(7);
    }
}
