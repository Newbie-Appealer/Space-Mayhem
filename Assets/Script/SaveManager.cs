using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Jobs;

#region ���̺� ������ ���δ°�
// �κ��丮 ������ Wrapper
public class InventoryWrapper
{
    public List<int> _itemCodes;
    public List<int> _itemStacks;
    public List<int> _itemSlotIndexs;

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
    public List<int> _blockMaxHp;
    public List<Vector3> _blockPosition;
    public List<Vector3> _blockRotation;

    public BuildingWrapper( Transform v_parnet ) 
    {
        // 1. List �ʱ�ȭ 
        _blocktypeIdx       = new List<int>();    
        _blockDetailIdx     = new List<int>();
        _blockHp            = new List<int>();
        _blockMaxHp         = new List<int>();
        _blockPosition      = new List<Vector3>();
        _blockRotation      = new List<Vector3>();   

        // 2.������ ������ ���� 
        for (int i = 0; i < v_parnet.childCount; i++)
        { 
            MyBuildingBlock _m = v_parnet.GetChild(i).GetComponent<MyBuildingBlock>();

            _blocktypeIdx.Add( _m.MyBlockTypeIdx );
            _blockDetailIdx.Add( _m.MyBlockDetailIdx );
            _blockHp.Add( _m.MyBlockHp );
            _blockMaxHp.Add( _m.MyBlockMaxHp );
            _blockPosition.Add( _m.MyPosition);
            _blockRotation.Add( _m.MyRotation );

        }

    }

}

// �÷��̾� ������ Wrapper
public class PlayerWrapper
    {
        // �����ؾ��Ұ�
        // 1. �÷��̾� ���/��/��� ��ġ ( float float float )
        // 2. �÷��̾� ���丮 ������Ȳ ( int )
        // 3. �÷��̾� ������ �ر���Ȳ ( int )

        // ���丮/������ ��Ȳ �߰������� �ۼ��ҿ���.
    }

public class FurnitureWrapper
{
    // 1.������ �ʿ��� �ε��� ( int 
    // 2.��ġ�� ( vector3
    // 3.ȸ���� ( vector3
    // 4.���� ������ (json
    public List<int> _furnitureIndex;
    public List<Vector3> _furniturePosition;
    public List<Vector3> _furnitureRotation;
    public List<string> _furnitureJsonData;

    public FurnitureWrapper(Transform v_furnitureParent)
    {
        _furnitureIndex = new List<int>();
        _furniturePosition = new List<Vector3>();
        _furnitureRotation = new List<Vector3>();
        _furnitureJsonData = new List<string>();

        for(int i = 0; i < v_furnitureParent.childCount; i++)
        {
            Furniture furniture = v_furnitureParent.GetChild(i).GetComponent<Furniture>();

            _furnitureIndex.Add(furniture.InstantiateIndex);
            _furniturePosition.Add(furniture.transform.position);
            _furnitureRotation.Add(furniture.transform.rotation.eulerAngles);
            _furnitureJsonData.Add(furniture.F_GetData());
        }
    }
}
#endregion

public class SaveManager : Singleton<SaveManager>
{
    // Local Data
    private string _savePath => Application.persistentDataPath + "/saves/";      // ���̺� ���� ���� �ӽ� ����
    private string _inventorySaveFileName   = "inventoryData";
    private string _buildSaveFileName       = "buildingData";
    private string _furnitureSaveFileName   = "furnitureData";

    private string _dataTableName = "gamedata";

    protected override void InitManager() {}

    #region �κ��丮 ����
    // �κ��丮 save
    public void F_SaveInventory(ref Item[] v_inventory)
    {
        InventoryWrapper a = new InventoryWrapper(ref v_inventory);                 // ������ ���α�.
        string saveData = JsonUtility.ToJson(a);
        int uid = AccountManager.Instance.uid;              // �÷��̾� ���� ��ȣ

        Debug.Log(saveData);

        // Guest Login ( LOCAL )
        if (uid == -1)
        {
            if (!Directory.Exists(_savePath))                                            // ������ �ִ��� Ȯ��.
                Directory.CreateDirectory(_savePath);                                   // ���� ����

            string saveFilePath = _savePath + _inventorySaveFileName + ".json";
            File.WriteAllText(saveFilePath, saveData);

            Debug.Log("Save inventory ( Local )");
        }

        // Login ( DB )
        else
        {
            string query = string.Format("UPDATE {0} SET InventoryData = '{1}' WHERE UID = {2}",
                _dataTableName, saveData, uid);

            DBConnector.Instance.F_Update(query);

            Debug.Log("Save inventory ( DB )");
        }
    }

    // �κ��丮 load
    public void F_LoadInventory(ref Item[] v_inventory)
    {
        int uid = AccountManager.Instance.uid;              // �÷��̾� ���� ��ȣ
        string saveFile;
        InventoryWrapper tmpData = null;
        // 1. �κ��丮 �迭 �ʱ�ȭ
        v_inventory = new Item[ItemManager.Instance.inventorySystem.inventorySize];
        // Guest Login ( LOCAL )
        if (uid == -1)
        {
            string saveFilePath = _savePath + _inventorySaveFileName + ".json"; // ���̺� ���� ��ġ

            // 2. ���̺������� ������ �ٷ� ����
            if (!File.Exists(saveFilePath))
                return;

            // 3. ���̺����� �б� ( json )
            saveFile = File.ReadAllText(saveFilePath);

            // 4. ���̺����� ��ȯ ( json -> inventoryWrapper )
            tmpData = JsonUtility.FromJson<InventoryWrapper>(saveFile); 

            Debug.Log("Load inventory ( Local )");
        }

        // Login ( DB )
        else
        {
            // ������
            string query = string.Format("SELECT InventoryData From {0} where uid = {1}",
                _dataTableName,uid);

            DataSet data = DBConnector.Instance.F_Select(query, _dataTableName);
            foreach(DataRow row in data.Tables[0].Rows)
            {
                saveFile = row["inventoryData"].ToString();     // ���̺� ������ string���� ������
                // ���̺� ������ ������ �ٷ� ����
                if (saveFile == "NONE")
                    return;
                tmpData = JsonUtility.FromJson<InventoryWrapper>(saveFile); // json ��ȯ
                break;
            }
        }


        // 5. �κ��丮�� ������ �ε�.
        for (int index = 0; index < tmpData._itemCodes.Count; index++)
        {
            int itemCode = tmpData._itemCodes[index];
            int itemStack = tmpData._itemStacks[index];
            int itemSlotIndex = tmpData._itemSlotIndexs[index];
            float itemDurability = tmpData._itemDurability[index];

            // �κ��丮�� ������ �߰�. ( ������ �����ͷ� )
            ItemManager.Instance.inventorySystem.F_AddItem(itemCode, itemSlotIndex);

            // ó�� ������ ������ �⺻ ������ 1 �̶� -1 �������!
            v_inventory[itemSlotIndex].F_AddStack(itemStack - 1);

            // ���� ������ �ʱ�ȭ ( ������ )
            if (itemDurability > 0)
            {
                (v_inventory[itemSlotIndex] as Tool).F_InitDurability(itemDurability);
            }
        }
    }

    #endregion

    #region �Ͽ�¡ ����
    // �Ͽ�¡ save
    public void F_SaveBuilding( Transform v_blockParent ) 
    {
        int uid = AccountManager.Instance.uid;

        BuildingWrapper ba = new BuildingWrapper(v_blockParent);      // ������ Ŭ���� new
        string buildSaveData = JsonUtility.ToJson(ba);              // Ŭ������ json���� ��ȯ (string Ÿ������ )

        // Guest Login ( LOCAL )
        if (uid == -1)
        {
            if (!Directory.Exists(_savePath))                           // ������ �ִ��� Ȯ��.
                Directory.CreateDirectory(_savePath);                   // ���� ����

            Debug.Log(buildSaveData);

            string saveFilePath = _savePath + _buildSaveFileName + ".json"; // ���� ������ ��ο�
            File.WriteAllText(saveFilePath, buildSaveData);                 // file�ۼ� ( ���, ���̺� ������ stinrg )

            Debug.Log("Your Building Is Saved ( Local )");
        }
        // Login ( DB )
        else
        {
            string query = string.Format("UPDATE {0} SET HousingData = '{1}' WHERE UID = {2}",
        _dataTableName, buildSaveData, uid);

            DBConnector.Instance.F_Update(query);

            Debug.Log("Your Building Is Saved ( DB )");
        }
    }
    // �Ͽ�¡ load
    public void F_LoadBuilding( Transform v_blockParent ) 
    {
        int uid = AccountManager.Instance.uid;
        string _buildSaveFile;
        BuildingWrapper _buildData = null;

        // Guest Login ( LOCAL )
        if (uid == -1)
        {
            Debug.Log("Building Is Loading ( Local )");

            // ���̺� ���� ��ġ
            string _saveLocation = _savePath + _buildSaveFileName + ".json";

            // 0. ���̺� ���� ������ �ٷ� ����
            if (!File.Exists(_saveLocation))
            {
                // 0-1. Building Manager�� �⺻ 9�� �� �����ϱ�
                BuildMaster.Instance.myBuildManger.F_FirstInitBaseFloor();
                // 0-2. �� �ε� �� ��ȸ�ϸ鼭 ������Ʈ 
                BuildMaster.Instance.myBuildManger.F_UpdateWholeBlock();
                // 0-3. �� �ε� �� �� model �� �����ؼ� ��ġ�Ϸ� ������ true��
                BuildMaster.Instance.myBuildManger.F_ModelComplte();

                return;
            }

            // 1. ���̺� ���� �б� (��ġ)
            _buildSaveFile = File.ReadAllText(_saveLocation);

            // 2. ���̺� ���� ��ȯ ( json -> BuildingWapper )
            _buildData = JsonUtility.FromJson<BuildingWrapper>(_buildSaveFile);  // string�� file�� <T> Ÿ������ ��ȯ 
        }

        // Login ( DB )
        else
        {
            // ������
            string query = string.Format("SELECT HousingData From {0} where uid = {1}",
                _dataTableName, uid);


            DataSet data = DBConnector.Instance.F_Select(query, _dataTableName);
            foreach (DataRow row in data.Tables[0].Rows)
            {
                // 0. ���̺� ���� �б� ( DB ) 
                _buildSaveFile = row["HousingData"].ToString();     
                // 1. ���̺� ������ ������ �ٷ� ����
                if (_buildSaveFile == "NONE")
                {
                    // 1-1. Building Manager�� �⺻ 9�� �� �����ϱ�
                    BuildMaster.Instance.myBuildManger.F_FirstInitBaseFloor();
                    // 1-2. �� �ε� �� ��ȸ�ϸ鼭 ������Ʈ 
                    BuildMaster.Instance.myBuildManger.F_UpdateWholeBlock();
                    // 1-3. �� �ε� �� �� model �� �����ؼ� ��ġ�Ϸ� ������ true��
                    BuildMaster.Instance.myBuildManger.F_ModelComplte();

                    return;
                }

                // 2. ���̺� ���� ��ȯ ( json -> BuildingWapper )
                _buildData = JsonUtility.FromJson<BuildingWrapper>(_buildSaveFile);
                break;
            }
        }

        // 3. block �ε�
        for (int i = 0; i < _buildData._blocktypeIdx.Count; i++)
        {
            int typeIdx = _buildData._blocktypeIdx[i];
            int detailIdx = _buildData._blockDetailIdx[i];
            int hp = _buildData._blockHp[i];
            int maxhp = _buildData._blockMaxHp[i];
            Vector3 currTrs = _buildData._blockPosition[i];
            Vector3 currRot = _buildData._blockRotation[i];

            // 3-1. ������Ʈ ���� 
            BuildMaster.Instance.myBuildManger.F_CreateBlockFromSave(typeIdx, detailIdx, currTrs, currRot, hp, maxhp);
        }

        // 3-2. �� �ε� �� �� ��ȸ�ϸ鼭 ������Ʈ
        BuildMaster.Instance.myBuildManger.F_UpdateWholeBlock();

        // 3-3. �� �ε� �� �� model �� �����ؼ� ��ġ�Ϸ� ������ true��
        BuildMaster.Instance.myBuildManger.F_ModelComplte();
    }

    #endregion

    #region ������(��ġ��) ����
    public void F_SaveFurniture(Transform v_parent)
    {
        FurnitureWrapper wrapper = new FurnitureWrapper(v_parent);
        string furnitureSaveData = JsonUtility.ToJson(wrapper);
        int uid = AccountManager.Instance.uid;

        //Guest Login ( Local )
        if (uid == -1)
        {
            if (!Directory.Exists(_savePath))                           // ������ �ִ��� Ȯ��.
                Directory.CreateDirectory(_savePath);                   // ���� ����
            
            string saveFilePath = _savePath + _furnitureSaveFileName + ".json";
            File.WriteAllText(saveFilePath, furnitureSaveData);

            Debug.Log("Your Furnitures is Saved ( Local ) ");
        }
        else
        {
            string query = string.Format("UPDATE {0} SET FurnitureData = '{1}' WHERE UID = {2}",
                _dataTableName, furnitureSaveData, uid);

            DBConnector.Instance.F_Update(query);

            Debug.Log("Your Furnitures is Saved ( DB )");
        }
    }

    public void F_LoadFurniture(Transform v_parent)
    {
        int uid = AccountManager.Instance.uid;
        string furnitureSaveFile = "";
        FurnitureWrapper furnitureData = null;

        // Guest Login ( Local )
        if(uid == -1)
        {
            string saveFilePath = _savePath + _furnitureSaveFileName + ".json";

            // 0. ���̺� ���� ������ �ٷ� ����
            if (!File.Exists(saveFilePath))
                return;

            // 1. ���� �ҷ����� ( json -> wrapper )
            furnitureSaveFile = File.ReadAllText(saveFilePath);
        }
        else
        {
            // ����
            string query = string.Format("SELECT FurnitureData FROM {0} WHERE UID = {1}",
                _dataTableName, uid);

            DataSet data = DBConnector.Instance.F_Select(query, _dataTableName);
            foreach(DataRow row in data.Tables[0].Rows)
            {
                furnitureSaveFile = row["FurnitureData"].ToString();

                if (furnitureSaveFile == "NONE")
                    return;
                break;
            }
        }

        furnitureData = JsonUtility.FromJson<FurnitureWrapper>(furnitureSaveFile);

        // ������Ʈ ��ġ
        for(int i = 0; i < furnitureData._furnitureIndex.Count; i++)
        {
            int idx = furnitureData._furnitureIndex[i];
            Vector3 pos = furnitureData._furniturePosition[i];
            Vector3 rotate = furnitureData._furnitureRotation[i];
            string data = furnitureData._furnitureJsonData[i];

            ItemManager.Instance.installSystem.F_LoadFurnitureInstall(idx, pos, rotate, data);
        }
    }
    #endregion

}
