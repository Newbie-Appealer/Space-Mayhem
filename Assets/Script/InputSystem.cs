using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    [SerializeField] private InventorySystem _inventorySystem;

    // 키 설정
    [Header("MOVE")]
    private KeyCode _moveFront = KeyCode.W;
    private KeyCode _moveBack = KeyCode.S; 
    private KeyCode _moveLeft = KeyCode.D;
    private KeyCode _moveRight = KeyCode.A;
    private KeyCode _moveJump = KeyCode.Space;

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
        
    }

    void F_InputUI()
    {
        if(Input.GetKeyDown(_invetory1) || Input.GetKeyDown(_invetory2))
        {
            UIManager.Instance.F_InventoryUI();
        }
    }

    void F_InputQuickSlot()
    {
        // _inventorySystem 에서 인벤토리 0 ~ 8번의 usable 함수 호출하기.
    }
}
