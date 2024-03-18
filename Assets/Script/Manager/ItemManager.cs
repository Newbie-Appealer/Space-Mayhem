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
    [Header("인벤토리 관련")]
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
