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
    [SerializeField] private InventorySystem _inventorySystem;
    [Header("Data")]
    [SerializeField] private List<ItemData> _itemDatas;
    public List<ItemData> ItemDatas => _itemDatas;
    protected override void InitManager() { }

    public void Start()
    {
        Debug.Log(_inventorySystem.F_GetItem(0));
        Debug.Log(_inventorySystem.F_GetItem(0));
        Debug.Log(_inventorySystem.F_GetItem(0));
        Debug.Log(_inventorySystem.F_GetItem(0));

        Debug.Log(_inventorySystem.F_GetItem(1));
        Debug.Log(_inventorySystem.F_GetItem(1));
        Debug.Log(_inventorySystem.F_GetItem(1));
        Debug.Log(_inventorySystem.F_GetItem(1));

        Debug.Log(_inventorySystem.F_GetItem(2));
        Debug.Log(_inventorySystem.F_GetItem(2));
        Debug.Log(_inventorySystem.F_GetItem(2));
        Debug.Log(_inventorySystem.F_GetItem(2));

        Debug.Log(_inventorySystem.F_GetItem(3));
        Debug.Log(_inventorySystem.F_GetItem(3));
        Debug.Log(_inventorySystem.F_GetItem(3));
        Debug.Log(_inventorySystem.F_GetItem(3));

        _inventorySystem.F_InventoryUIUpdate();
    }

    public ItemData F_GetData(int v_code)
    {
        return _itemDatas[v_code];
    }
}
