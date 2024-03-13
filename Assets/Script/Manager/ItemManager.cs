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
    public InventorySystem inventorySystem => _inventorySystem;
    [Header("Data")]
    [SerializeField] private List<ItemData> _itemDatas;
    public List<ItemData> ItemDatas => _itemDatas;
    protected override void InitManager() { }

    public ItemData F_GetData(int v_code)
    {
        return _itemDatas[v_code];
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            for (int l = 0; l < 31; l++)
            {
                Debug.Log(_inventorySystem.F_GetItem(l));
            }

            _inventorySystem.F_InventoryUIUpdate();
        }
    }
}
