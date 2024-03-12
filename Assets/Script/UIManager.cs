using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private Canvas _canvas;
    public Canvas canvas => _canvas;
    protected override void InitManager() { }

    public void F_InventoryUI()
    {
        // 인벤토리 켜져있을때
        // 움직임 X
        // 마우스 커서 고정 해제

        if (_inventoryUI.activeSelf) // 켜져있으면
        {
            _inventoryUI.SetActive(false);                              // 인벤토리 OFF
        }
        else // 꺼져있으면
        {
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); // 인벤토리 업데이트
            _inventoryUI.SetActive(true);                               // 인벤토리 ON
        }
    }
}
