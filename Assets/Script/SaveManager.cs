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

// building ������ Wrapper
public class BuildingWrapper
{
    public List<int> _blocktypeIdx;
    public List<int> _blockDetailIdx;
    public List<int> _blockHp;
    public List<Vector3> _blockPosition;

    public BuildingWrapper( Transform v_parnet ) 
    {
        // 1. List �ʱ�ȭ 
        _blocktypeIdx = new List<int>();    
        _blockDetailIdx = new List<int>();
        _blockHp = new List<int>();
        _blockPosition = new List<Vector3>();

        // 2.������ ������ ���� 
        for (int i = 0; i < v_parnet.childCount; i++)
        { 
            MyBuildingBlock _m = v_parnet.GetChild(i).GetComponent<MyBuildingBlock>();

            _blocktypeIdx.Add( _m.MyBlockTypeIdx );
            _blockDetailIdx.Add( _m.MyBlockDetailIdx );
            _blockHp.Add( _m.MyBlockHp );
            _blockPosition.Add(_m.MyPosition);
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
    private string _inventorySaveFileName   = "inventoryData";
    private string _buildSaveFileName       = "buildingData";

    protected override void InitManager() {}

    #region �κ��丮 ����
    // �κ��丮 save
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
    // �κ��丮 load
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

    #region �Ͽ�¡ ����
    // �Ͽ�¡ save
    public void F_SaveBuilding( Transform v_blockParent ) 
    {
        if (!Directory.Exists(_savePath))                           // ������ �ִ��� Ȯ��.
            Directory.CreateDirectory(_savePath);                   // ���� ����

        BuildingWrapper ba = new BuildingWrapper(v_blockParent);      // ������ Ŭ���� new
        string buildSaveData = JsonUtility.ToJson(ba);              // Ŭ������ json���� ��ȯ (string Ÿ������ )

        Debug.Log(buildSaveData);

        string saveFilePath = _savePath + _buildSaveFileName + ".json"; // ���� ������ ��ο�
        File.WriteAllText(saveFilePath, buildSaveData);                 // file�ۼ� ( ���, ���̺� ������ stinrg )

        Debug.Log("Your Building Is Saved");
    }
    // �Ͽ�¡ load
    public void F_LoadBuilding( Transform v_blockParent ) 
    {
        Debug.Log("Building Is Loading");
        
        // ���̺� ���� ��ġ
        string _saveLocation = _savePath + _buildSaveFileName + ".json";
        
        // 0. ���̺� ���� ������ �ٷ� ����
        if (!File.Exists(_saveLocation))
        {
            // 0.1 Building Manager�� �⺻ 9�� �� �����ϱ�
            MyBuildManager.Instance.F_FirstInitBaseFloor();
            return;
        }

        // 1. ���̺� ���� �б� (��ġ)
        string _buildSaveFile = File.ReadAllText(_saveLocation);

        // 2. ���̺� ���� ��ȯ ( json -> BuildingWapper )
        BuildingWrapper _buildData = JsonUtility.FromJson<BuildingWrapper>(_buildSaveFile);  // string�� file�� <T> Ÿ������ ��ȯ 

        // 3. block �ε�
        for (int i = 0; i < _buildData._blocktypeIdx.Count; i++) 
        {
            int typeIdx         = _buildData._blocktypeIdx[i];
            int detailIdx       = _buildData._blockDetailIdx[i];
            int hp              = _buildData._blockHp[i];
            Vector3 currTrs    = _buildData._blockPosition[i];

            // 3-1. ������Ʈ ���� 
            GameObject _tmp = Instantiate( MyBuildManager.Instance._bundleBulingPrefab[typeIdx][detailIdx]);
            // 3-2. ��ġ ���� 
            _tmp.transform.position = currTrs;
            // 3-3. �θ�����
            _tmp.transform.parent = v_blockParent;

            // 3-4. mybuildng ��ũ��Ʈ �˻� 
            if (_tmp.GetComponent<MyBuildingBlock>() == null)
                _tmp.AddComponent<MyBuildingBlock>();

            // 3-5. hp ����
            MyBuildingBlock _tmpBlock = _tmp.GetComponent<MyBuildingBlock>();
            _tmpBlock.F_SetBlockFeild( typeIdx , detailIdx , hp );

            // 3-6. �� ���� ���� Ŀ���� ������Ʈ
            _tmpBlock.F_BlockCollisionConnector();

        }

    }

    #endregion

    #region ������(��ġ��) ����
    #endregion

}
