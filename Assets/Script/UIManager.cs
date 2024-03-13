using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private Canvas _canvas;
    // 0 : 산소 , 1 : 물 , 2 : 배고픔
    [SerializeField] private Image[] _player_StatUI;
    public Canvas canvas => _canvas;
    protected override void InitManager() 
    {
    }

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

    public void F_PlayerStatUIUpdate()
    {
        _player_StatUI[0].fillAmount = PlayerManager.Instance.F_GetStat(0) / 100f ;
        _player_StatUI[1].fillAmount = PlayerManager.Instance.F_GetStat(1) / 100f;
        _player_StatUI[2].fillAmount = PlayerManager.Instance.F_GetStat(2) / 100f;
    }
}
