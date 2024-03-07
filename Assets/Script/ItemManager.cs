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
        // 아이템 종류 추가가 필요할땐 여기서.
}
public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] private InventorySystem _inventorySystem;

    [Header("Data")]
    [SerializeField] private List<ItemData> _itemDatas;
    protected override void InitManager()
    {
        _itemDatas = new List<ItemData>();

        _itemDatas.Add(new StuffData(0, "Plastic", "플라스틱"));
        _itemDatas.Add(new StuffData(1, "Fiber", "섬유"));
        _itemDatas.Add(new StuffData(2, "Scrap", "고철"));
        _itemDatas.Add(new FoodData(3, "Potato", "감자",0.1f));
        _itemDatas.Add(new ToolData(4, "장비", "장비", 1.5f));
        _itemDatas.Add(new InstallData(5, "작업대", "작업대", 5f));
    }

    public Item GetItem(int v_code,ItemType v_type)
    {
        switch(v_type)
        {
            case ItemType.NONE:
                return null;

            case ItemType.STUFF:
                if (_itemDatas[v_code] is StuffData)
                    return new Stuff(_itemDatas[v_code] as StuffData);
                break;

            case ItemType.FOOD:
                if(_itemDatas[v_code] is FoodData)
                    return new Food(_itemDatas[v_code] as FoodData);
                break;

            case ItemType.TOOL:
                if (_itemDatas[v_code] is ToolData)
                    return new Tool(_itemDatas[v_code] as ToolData);
                break;

            case ItemType.INSTALL:
                if (_itemDatas[v_code] is InstallData)
                    return new Install(_itemDatas[v_code] as InstallData);
                break;
        }

        return null;
    }

    private void Start()
    {
        _inventorySystem.AddItem(GetItem(2, ItemType.STUFF));
        _inventorySystem.AddItem(GetItem(3, ItemType.FOOD));
        _inventorySystem.AddItem(GetItem(4, ItemType.TOOL));
        _inventorySystem.AddItem(GetItem(5, ItemType.INSTALL));
    }
}
