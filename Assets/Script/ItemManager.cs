using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    NONE,           // ����ó����
    STUFF,          // ��������
    FOOD,           // ����(�Һ�)������
    TOOL,           // ����������
    INSTALL         // ��ġ������
        // ������ ���� �߰��� �ʿ��Ҷ� ���⼭.
}
public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] private InventorySystem _inventorySystem;

    [Header("Data")]
    [SerializeField] private List<ItemData> _itemDatas;
    protected override void InitManager()
    {
        _itemDatas = new List<ItemData>();

        _itemDatas.Add(new StuffData(0, "Plastic", "�ö�ƽ"));
        _itemDatas.Add(new StuffData(1, "Fiber", "����"));
        _itemDatas.Add(new StuffData(2, "Scrap", "��ö"));
        _itemDatas.Add(new FoodData(3, "Potato", "����",0.1f));
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
