using System.Collections;
using System.Collections.Generic;
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
    }

    public Stuff GetStuff(int v_code)
    {
        return new Stuff(_itemDatas[v_code] as StuffData);
    }

    public Food GetFood(int v_code)
    {
        return new Food(_itemDatas[v_code] as FoodData);
    }

    private void Start()
    {
        
    }
}
