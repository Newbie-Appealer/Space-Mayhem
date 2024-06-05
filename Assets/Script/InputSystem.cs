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

    [Header("Motion")]
    private KeyCode _motion1 = KeyCode.F1;
    private KeyCode _motion2 = KeyCode.F2;
    private KeyCode _motion3= KeyCode.F3;
    private KeyCode _motion4 = KeyCode.F4;

    private void Update()
    {
        // 로딩창이 켜져있는 상태에서 키입력 방지
        if (UIManager.Instance.onLoading)
            return;

        F_InputUI();
        F_InputQuickSlot();
        F_InputMotion();
    }

    void F_InputUI()
    {
        if(Input.GetKeyDown(_invetory1) || Input.GetKeyDown(_invetory2))
        {   
            // 1. 사운드 재생
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK);

            // 2. 인벤토리 ON/OFF
            UIManager.Instance.OnInventoryUI();

            // 3. 플레이어 상태 전환
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

            // 4. 퀵슬롯 포커스 초기화
            UIManager.Instance.F_QuickSlotFocus(-1);
        }

        if(Input.GetKeyDown(_pause))
        {
            // 1. 사운드 재생
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK);

            // 2. 플레이어 상태 전환
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

            // 3. 퀵슬롯 포커스 초기화
            UIManager.Instance.F_QuickSlotFocus(-1);

            // 인벤토리가 열린상태일때 -> 인벤토리 닫음
            if (UIManager.Instance.onInventory)
            {
                UIManager.Instance.OnInventoryUI();
                return;
            }

            // 인벤토리가 열린상태가 아닐때 -> Puase UI ON
            UIManager.Instance.F_OnPauseUI(!UIManager.Instance.onPause);
        }
    }

    void F_InputQuickSlot()
    {
        // 인벤토리/Pause 열려있을때 퀵슬롯 사용 못하도록 막음.
        if (UIManager.Instance.onInventory || UIManager.Instance.onPause)
            return;

        if(Input.GetKeyDown(_quick_1))
        {
            ItemManager.Instance.inventorySystem.F_UseItem(0);
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK2);
        }

        else if (Input.GetKeyDown(_quick_2))
        {
            ItemManager.Instance.inventorySystem.F_UseItem(1);
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK2);
        }

        else if (Input.GetKeyDown(_quick_3))
        {
            ItemManager.Instance.inventorySystem.F_UseItem(2);
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK2);
        }

        else if (Input.GetKeyDown(_quick_4))
        {
            ItemManager.Instance.inventorySystem.F_UseItem(3);
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK2);
        }

        else if (Input.GetKeyDown(_quick_5))
        {
            ItemManager.Instance.inventorySystem.F_UseItem(4);
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK2);
        }

        else if (Input.GetKeyDown(_quick_6))
        {
            ItemManager.Instance.inventorySystem.F_UseItem(5);
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK2);
        }

        else if (Input.GetKeyDown(_quick_7))
        {
            ItemManager.Instance.inventorySystem.F_UseItem(6);
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK2);
        }

        else if (Input.GetKeyDown(_quick_8))
        {
            ItemManager.Instance.inventorySystem.F_UseItem(7);
            SoundManager.Instance.F_PlaySFX(SFXClip.CLICK2);
        }
    }

    void F_InputMotion()
    {
        //한손 따봉
       if (Input.GetKeyDown(_motion1) && !PlayerManager.Instance._isLeftGoodPlaying)
            PlayerManager.Instance.PlayerController.F_LeftGoodMotion();
       else if (Input.GetKeyDown(_motion1) && PlayerManager.Instance._isLeftGoodPlaying)
            PlayerManager.Instance.PlayerController.F_LeftGoodMotionEnd();

       //양손 따봉
        if (Input.GetKeyDown(_motion2) && PlayerManager.Instance._isLeftGoodPlaying && !PlayerManager.Instance._isDoubleGoodPlaying)
            PlayerManager.Instance.PlayerController.F_RightGoodMotion();
        else if (Input.GetKeyDown(_motion2) && PlayerManager.Instance._isDoubleGoodPlaying)
            PlayerManager.Instance.PlayerController.F_GoodMotionEnd();

        //인사
       if (Input.GetKeyDown(_motion3)) 
            PlayerManager.Instance.PlayerController.F_HelloMotion();

       //춤추기
       if (Input.GetKeyDown(_motion4) && !PlayerManager.Instance._isDancing)
            PlayerManager.Instance.PlayerController.F_DanceMotion();
       else if (Input.GetKeyDown(_motion4) && PlayerManager.Instance._isDancing)
            PlayerManager.Instance.PlayerController.F_DanceMotionEnd();
    }
}
