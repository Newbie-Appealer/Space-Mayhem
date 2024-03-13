using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private Canvas _canvas;
    // 0 : ��� , 1 : �� , 2 : �����
    [SerializeField] private Image[] _player_StatUI;
    public Canvas canvas => _canvas;
    protected override void InitManager() 
    {
    }

    public void F_InventoryUI()
    {
        // �κ��丮 ����������
        // ������ X
        // ���콺 Ŀ�� ���� ����

        if (_inventoryUI.activeSelf) // ����������
        {
            _inventoryUI.SetActive(false);                              // �κ��丮 OFF
        }
        else // ����������
        {
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); // �κ��丮 ������Ʈ
            _inventoryUI.SetActive(true);                               // �κ��丮 ON
        }
    }

    public void F_PlayerStatUIUpdate()
    {
        _player_StatUI[0].fillAmount = PlayerManager.Instance.F_GetStat(0) / 100f ;
        _player_StatUI[1].fillAmount = PlayerManager.Instance.F_GetStat(1) / 100f;
        _player_StatUI[2].fillAmount = PlayerManager.Instance.F_GetStat(2) / 100f;
    }
}
