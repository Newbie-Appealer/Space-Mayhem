using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    [SerializeField] private InventorySystem _inventorySystem;
    [SerializeField] private Player_Controller _playerController;

    // 키 설정
    [Header("MOVE")]
    private KeyCode _moveJump = KeyCode.Space;
    private KeyCode _moveCrouch = KeyCode.C;

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
        F_InputPlayer();
        F_InputUI();
        F_InputQuickSlot();
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
        if(Input.GetKeyDown(_quick_1))
        {
            // int slotNumber = (int)KeyCode.Alpha1 - 49;
            // ItemManager.Instance.inventorySystem.UseableItem(slotNumber);
        }
    }

    void F_InputPlayer()
    {
        _playerController.F_PlayerRun();                    // 달리기      1
        _playerController.F_PlayerCrouch(_moveCrouch);      // 앉기        2             
        _playerController.F_PlayerMove();                   // 움직임      3
        _playerController.F_PlayerCheckScrap();             // 스크랩 체크 4
        _playerController.F_PlayerCameraMove();             // 카메라 회전 5
    }
}
