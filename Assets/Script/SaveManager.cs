using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;

#region ���̺� ������ ���δ°�
// �κ��丮 ������ Wrapper
public class InventoryWrapper
{
    public List<int> _itemCodes;
    public List<int> _itemStacks;
    public List<int> _itemSlotIndexs;
    public List<int> _itemTypes;

    public List<float> _itemDurability;                 // ������ ������ �������� ���� -1
    public InventoryWrapper(ref Item[] v_inventory)
    {
        // 1. ������ �ʱ�ȭ
        _itemCodes      = new List<int>();
        _itemStacks     = new List<int>();
        _itemSlotIndexs = new List<int>();

        _itemDurability = new List<float>();

        // 2. ������ ������ ����
        for (int index = 0; index < v_inventory.Length; index++)
        {
            if (v_inventory[index] == null)
                continue;
            if (v_inventory[index].F_IsEmpty())
                continue;

            _itemCodes.Add(v_inventory[index].itemCode);             
            _itemStacks.Add(v_inventory[index].currentStack);       
            _itemSlotIndexs.Add(index);                            

            // ���� ������ ��¡�� ���� ó��
            if (v_inventory[index] is Tool)
                _itemDurability.Add((v_inventory[index] as Tool).durability);
            else
                _itemDurability.Add(-1);
        }
    }
}

// �÷��̾� ������ Wrapper
public class PlauerWrapper
{
    // �����ؾ��Ұ�
    // 1. �÷��̾� ���/��/��� ��ġ ( float float float )
    // 2. �÷��̾� ���丮 ������Ȳ ( int )
    // 3. �÷��̾� ������ �ر���Ȳ ( int )

    // ���丮/������ ��Ȳ �߰������� �ۼ��ҿ���.
}
#endregion

public class SaveManager : Singleton<SaveManager>
{
    private string _savePath => Application.persistentDataPath + "/saves/";      // ���̺� ���� ���� �ӽ� ����
    private string _inventorySaveFileName = "inventoryData";

    protected override void InitManager() {}

    #region �κ��丮 ����
    public void F_SaveInventory(ref Item[] v_inventory)
    {
        if(!Directory.Exists(_savePath))                                            // ������ �ִ��� Ȯ��.
            Directory.CreateDirectory(_savePath);                                   // ���� ����

        InventoryWrapper a = new InventoryWrapper(ref v_inventory);                 // ������ ���α�.
        string saveData = JsonUtility.ToJson(a);

        Debug.Log(saveData);

        string saveFilePath = _savePath + _inventorySaveFileName + ".json";
        File.WriteAllText(saveFilePath, saveData);

        Debug.Log("Save inventory");
    }
    public void F_LoadInventory(ref Item[] v_inventory)
    {
        // ���̺� ���� ��ġ
        string saveFilePath = _savePath + _inventorySaveFileName + ".json";

        // 1.�κ��丮 �迭 �ʱ�ȭ
        v_inventory = new Item[ItemManager.Instance.inventorySystem.inventorySize];

        // 2. ���̺������� ������ �ٷ� ����
        if (!File.Exists(saveFilePath))
            return;

        // 3. ���̺����� �б� ( json )
        string saveFile = File.ReadAllText(saveFilePath);

        // 4. ���̺����� ��ȯ ( json -> inventoryWrapper )
        InventoryWrapper tmpData = JsonUtility.FromJson<InventoryWrapper>(saveFile);

        // 5. �κ��丮�� ������ �ε�.
        for (int index = 0; index < tmpData._itemCodes.Count; index++)
        {
            int itemCode = tmpData._itemCodes[index];
            int itemStack = tmpData._itemStacks[index];
            int itemSlotIndex = tmpData._itemSlotIndexs[index];
            float itemDurabiloity = tmpData._itemDurability[index];

            // �κ��丮�� ������ �߰�. ( ������ �����ͷ� )
            ItemManager.Instance.inventorySystem.F_AddItem(itemCode, itemSlotIndex);

            // ó�� ������ ������ �⺻ ������ 1 �̶� -1 �������!
            v_inventory[itemSlotIndex].F_AddStack(itemStack - 1);  

            // ���� ������ �ʱ�ȭ ( ������ )
            if (itemDurabiloity > 0)     
            {
                (v_inventory[itemSlotIndex] as Tool).F_InitDurability(itemDurabiloity);
            }
        }
    }

    #endregion

    #region ������(��ġ��) ����
    #endregion

    #region �� ����
    #endregion
}
