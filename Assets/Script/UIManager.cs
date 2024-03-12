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
}
