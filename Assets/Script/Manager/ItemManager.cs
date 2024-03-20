using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ItemType
{
    NONE,           // 예외처리용
    STUFF,          // 재료아이템
    FOOD,           // 음식(소비)아이템
    TOOL,           // 도구아이템
    INSTALL         // 설치아이템
}
public class ItemManager : Singleton<ItemManager>
{


    [Header("Systems")]
    [SerializeField] private InventorySystem _inventorySystem;
    [SerializeField] private CraftSystem _craftSystem;
    public InventorySystem inventorySystem => _inventorySystem;
    public CraftSystem craftSystem => _craftSystem;

    [Header("Data")]
    [SerializeField] private List<ItemData> _itemDatas;
    public List<ItemData> ItemDatas => _itemDatas;
    protected override void InitManager() { }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {

            _inventorySystem.F_GetItem(0);
            _inventorySystem.F_GetItem(1);
            _inventorySystem.F_GetItem(20);
            _inventorySystem.F_GetItem(21);

            _inventorySystem.F_InventoryUIUpdate();
        }
    }

    public void F_InitItemDatas()
    {
        _itemDatas = new List<ItemData>();

    }
}
