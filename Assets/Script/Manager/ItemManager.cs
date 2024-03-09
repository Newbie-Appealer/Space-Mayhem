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
        _itemDatas.Add(new ToolData(4, "���", "���", 1.5f));
        _itemDatas.Add(new InstallData(5, "�۾���", "�۾���", 5f));
    }

    public Item F_GetItem(int v_code,ItemType v_type)
    {
        switch(v_type)
        {
            case ItemType.NONE:
                return null;

            case ItemType.STUFF:
                StuffData data_1 = _itemDatas[v_code] as StuffData;
                StuffData newData_1 = new StuffData(data_1.itemCode, data_1.itemName, data_1.itemDescription);
                return new Stuff(newData_1);

            case ItemType.FOOD:
                FoodData data_2 = _itemDatas[v_code] as FoodData;
                FoodData newData_2 = new FoodData(data_2.itemCode, data_2.itemName, data_2.itemDescription, data_2.value);
                return new Food(newData_2);

            case ItemType.TOOL:
                ToolData data_3 = _itemDatas[v_code] as ToolData;
                ToolData newData_3 = new ToolData(data_3.itemCode, data_3.itemName, data_3.itemDescription, data_3.durability);
                return new Tool(newData_3);

            case ItemType.INSTALL:
                InstallData data_4 = _itemDatas[v_code] as InstallData;
                InstallData newData_4 = new InstallData(data_4.itemCode, data_4.itemName, data_4.itemDescription, data_4.hp);
                return new Install(newData_4);
        }

        return null;
    }

    public void F_GetScrap(int v_code, ItemType v_type)
    {
        _inventorySystem.F_AddItem(F_GetItem(v_code, v_type));
        _inventorySystem.F_InventoryUIUpdate();
    }

    private void Start()
    {

        //// ������ �߰� 2����
        //_inventorySystem.F_AddItem(F_GetItem(0, ItemType.STUFF));
        //_inventorySystem.F_AddItem(F_GetItem(0, ItemType.STUFF));

        //_inventorySystem.F_AddItem(F_GetItem(1, ItemType.STUFF));
        //_inventorySystem.F_AddItem(F_GetItem(1, ItemType.STUFF));

        //_inventorySystem.F_AddItem(F_GetItem(2, ItemType.STUFF));
        //_inventorySystem.F_AddItem(F_GetItem(2, ItemType.STUFF))
        //    ;
        //_inventorySystem.F_AddItem(F_GetItem(3, ItemType.FOOD));
        //_inventorySystem.F_AddItem(F_GetItem(3, ItemType.FOOD));

        //_inventorySystem.F_AddItem(F_GetItem(4, ItemType.TOOL));
        //_inventorySystem.F_AddItem(F_GetItem(4, ItemType.TOOL));

        //_inventorySystem.F_AddItem(F_GetItem(5, ItemType.INSTALL));
        //_inventorySystem.F_AddItem(F_GetItem(5, ItemType.INSTALL));

        //_inventorySystem.F_InventoryUIUpdate();     // UI ������Ʈ
    }
}
