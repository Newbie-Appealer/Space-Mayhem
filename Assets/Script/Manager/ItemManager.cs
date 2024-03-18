using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ItemType
{
    NONE,           // ����ó����
    STUFF,          // ��������
    FOOD,           // ����(�Һ�)������
    TOOL,           // ����������
    INSTALL         // ��ġ������
}
public class ItemManager : Singleton<ItemManager>
{
    [Header("�κ��丮 ����")]
    [SerializeField] private InventorySystem _inventorySystem;
    public InventorySystem inventorySystem => _inventorySystem;
    [Header("Data")]
    [SerializeField] private List<ItemData> _itemDatas;
    public List<ItemData> ItemDatas => _itemDatas;
    protected override void InitManager() { }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {

            Debug.Log(_inventorySystem.F_GetItem(20));
            Debug.Log(_inventorySystem.F_GetItem(21));

            _inventorySystem.F_InventoryUIUpdate();
        }
    }
}
