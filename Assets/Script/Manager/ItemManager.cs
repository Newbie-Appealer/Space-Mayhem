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
    [SerializeField] private CraftSystem _craftSystem;
    [SerializeField] private InventorySystem _inventorySystem;
    public InventorySystem inventorySystem => _inventorySystem;
    [Header("Data")]
    [SerializeField] private List<ItemData> _itemDatas;
    public List<ItemData> ItemDatas => _itemDatas;
    protected override void InitManager() { }

    public void Start()
    {
        _inventorySystem.F_InventoryUIUpdate();
    }
}
