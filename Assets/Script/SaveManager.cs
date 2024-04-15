using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Jobs;

#region 세이브 데이터 감싸는곳
// 인벤토리 데이터 Wrapper
public class InventoryWrapper
{
    public List<int> _itemCodes;
    public List<int> _itemStacks;
    public List<int> _itemSlotIndexs;

    public List<float> _itemDurability;                 // 도구를 제외한 나머지는 전부 -1
    public InventoryWrapper(ref Item[] v_inventory)
    {
        // 1. 데이터 초기화
        _itemCodes      = new List<int>();
        _itemStacks     = new List<int>();
        _itemSlotIndexs = new List<int>();

        _itemDurability = new List<float>();

        // 2. 저장할 데이터 정리
        for (int index = 0; index < v_inventory.Length; index++)
        {
            if (v_inventory[index] == null)
                continue;
            if (v_inventory[index].F_IsEmpty())
                continue;

            _itemCodes.Add(v_inventory[index].itemCode);             
            _itemStacks.Add(v_inventory[index].currentStack);       
            _itemSlotIndexs.Add(index);                            

            // 도구 내구도 저징을 위한 처리
            if (v_inventory[index] is Tool)
                _itemDurability.Add((v_inventory[index] as Tool).durability);
            else
                _itemDurability.Add(-1);
        }
    }
}

// building 데이터 Wrapper
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
        // 1. List 초기화 
        _blocktypeIdx       = new List<int>();    
        _blockDetailIdx     = new List<int>();
        _blockHp            = new List<int>();
        _blockMaxHp         = new List<int>();
        _blockPosition      = new List<Vector3>();
        _blockRotation      = new List<Vector3>();   

        // 2.저장할 데이터 정리 
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

// 플레이어 데이터 Wrapper
public class PlayerWrapper
    {
        // 저장해야할것
        // 1. 플레이어 산소/물/허기 수치 ( float float float )
        // 2. 플레이어 스토리 진행현황 ( int )
        // 3. 플레이어 레시피 해금현황 ( int )

        // 스토리/레시피 현황 추가됐을때 작성할예정.
    }

public class FurnitureWrapper
{
    // 1.생성에 필요한 인덱스 ( int 
    // 2.위치값 ( vector3
    // 3.회전값 ( vector3
    // 4.내부 데이터 (json
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
    private string _savePath => Application.persistentDataPath + "/saves/";      // 세이브 파일 저장 임시 폴더
    private string _inventorySaveFileName   = "inventoryData";
    private string _buildSaveFileName       = "buildingData";
    private string _furnitureSaveFileName   = "furnitureData";

    private string _dataTableName = "gamedata";

    protected override void InitManager() {}

    #region 인벤토리 저장
    // 인벤토리 save
    public void F_SaveInventory(ref Item[] v_inventory)
    {
        InventoryWrapper a = new InventoryWrapper(ref v_inventory);                 // 데이터 감싸기.
        string saveData = JsonUtility.ToJson(a);
        int uid = AccountManager.Instance.uid;              // 플레이어 고유 번호

        Debug.Log(saveData);

        // Guest Login ( LOCAL )
        if (uid == -1)
        {
            if (!Directory.Exists(_savePath))                                            // 폴더가 있는지 확인.
                Directory.CreateDirectory(_savePath);                                   // 폴더 생성

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

    // 인벤토리 load
    public void F_LoadInventory(ref Item[] v_inventory)
    {
        int uid = AccountManager.Instance.uid;              // 플레이어 고유 번호
        string saveFile;
        InventoryWrapper tmpData = null;
        // 1. 인벤토리 배열 초기화
        v_inventory = new Item[ItemManager.Instance.inventorySystem.inventorySize];
        // Guest Login ( LOCAL )
        if (uid == -1)
        {
            string saveFilePath = _savePath + _inventorySaveFileName + ".json"; // 세이브 파일 위치

            // 2. 세이브파일이 없으면 바로 종료
            if (!File.Exists(saveFilePath))
                return;

            // 3. 세이브파일 읽기 ( json )
            saveFile = File.ReadAllText(saveFilePath);

            // 4. 세이브파일 변환 ( json -> inventoryWrapper )
            tmpData = JsonUtility.FromJson<InventoryWrapper>(saveFile); 

            Debug.Log("Load inventory ( Local )");
        }

        // Login ( DB )
        else
        {
            // 쿼리문
            string query = string.Format("SELECT InventoryData From {0} where uid = {1}",
                _dataTableName,uid);

            DataSet data = DBConnector.Instance.F_Select(query, _dataTableName);
            foreach(DataRow row in data.Tables[0].Rows)
            {
                saveFile = row["inventoryData"].ToString();     // 세이브 데이터 string으로 가져옴
                // 세이브 파일이 없으면 바로 종료
                if (saveFile == "NONE")
                    return;
                tmpData = JsonUtility.FromJson<InventoryWrapper>(saveFile); // json 변환
                break;
            }
        }


        // 5. 인벤토리에 데이터 로드.
        for (int index = 0; index < tmpData._itemCodes.Count; index++)
        {
            int itemCode = tmpData._itemCodes[index];
            int itemStack = tmpData._itemStacks[index];
            int itemSlotIndex = tmpData._itemSlotIndexs[index];
            float itemDurability = tmpData._itemDurability[index];

            // 인벤토리에 아이템 추가. ( 저장한 데이터로 )
            ItemManager.Instance.inventorySystem.F_AddItem(itemCode, itemSlotIndex);

            // 처음 아이템 생성시 기본 스택이 1 이라서 -1 해줘야함!
            v_inventory[itemSlotIndex].F_AddStack(itemStack - 1);

            // 도구 데이터 초기화 ( 내구도 )
            if (itemDurability > 0)
            {
                (v_inventory[itemSlotIndex] as Tool).F_InitDurability(itemDurability);
            }
        }
    }

    #endregion

    #region 하우징 저장
    // 하우징 save
    public void F_SaveBuilding( Transform v_blockParent ) 
    {
        int uid = AccountManager.Instance.uid;

        BuildingWrapper ba = new BuildingWrapper(v_blockParent);      // 저장할 클래스 new
        string buildSaveData = JsonUtility.ToJson(ba);              // 클래스를 json으로 변환 (string 타입으로 )

        // Guest Login ( LOCAL )
        if (uid == -1)
        {
            if (!Directory.Exists(_savePath))                           // 폴더가 있는지 확인.
                Directory.CreateDirectory(_savePath);                   // 폴더 생성

            Debug.Log(buildSaveData);

            string saveFilePath = _savePath + _buildSaveFileName + ".json"; // 내가 지정한 경로에
            File.WriteAllText(saveFilePath, buildSaveData);                 // file작성 ( 경로, 세이브 데이터 stinrg )

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
    // 하우징 load
    public void F_LoadBuilding( Transform v_blockParent ) 
    {
        int uid = AccountManager.Instance.uid;
        string _buildSaveFile;
        BuildingWrapper _buildData = null;

        // Guest Login ( LOCAL )
        if (uid == -1)
        {
            Debug.Log("Building Is Loading ( Local )");

            // 세이브 파일 위치
            string _saveLocation = _savePath + _buildSaveFileName + ".json";

            // 0. 세이브 파일 없으면 바로 종료
            if (!File.Exists(_saveLocation))
            {
                // 0-1. Building Manager의 기본 9개 블럭 생성하기
                BuildMaster.Instance.myBuildManger.F_FirstInitBaseFloor();
                // 0-2. 블럭 로드 후 순회하면서 업데이트 
                BuildMaster.Instance.myBuildManger.F_UpdateWholeBlock();
                // 0-3. 블럭 로드 후 각 model 에 접근해서 설치완료 변수를 true로
                BuildMaster.Instance.myBuildManger.F_ModelComplte();

                return;
            }

            // 1. 세이브 파일 읽기 (위치)
            _buildSaveFile = File.ReadAllText(_saveLocation);

            // 2. 세이브 파일 변환 ( json -> BuildingWapper )
            _buildData = JsonUtility.FromJson<BuildingWrapper>(_buildSaveFile);  // string형 file을 <T> 타입으로 변환 
        }

        // Login ( DB )
        else
        {
            // 쿼리문
            string query = string.Format("SELECT HousingData From {0} where uid = {1}",
                _dataTableName, uid);


            DataSet data = DBConnector.Instance.F_Select(query, _dataTableName);
            foreach (DataRow row in data.Tables[0].Rows)
            {
                // 0. 세이브 파일 읽기 ( DB ) 
                _buildSaveFile = row["HousingData"].ToString();     
                // 1. 세이브 파일이 없으면 바로 종료
                if (_buildSaveFile == "NONE")
                {
                    // 1-1. Building Manager의 기본 9개 블럭 생성하기
                    BuildMaster.Instance.myBuildManger.F_FirstInitBaseFloor();
                    // 1-2. 블럭 로드 후 순회하면서 업데이트 
                    BuildMaster.Instance.myBuildManger.F_UpdateWholeBlock();
                    // 1-3. 블럭 로드 후 각 model 에 접근해서 설치완료 변수를 true로
                    BuildMaster.Instance.myBuildManger.F_ModelComplte();

                    return;
                }

                // 2. 세이브 파일 변환 ( json -> BuildingWapper )
                _buildData = JsonUtility.FromJson<BuildingWrapper>(_buildSaveFile);
                break;
            }
        }

        // 3. block 로드
        for (int i = 0; i < _buildData._blocktypeIdx.Count; i++)
        {
            int typeIdx = _buildData._blocktypeIdx[i];
            int detailIdx = _buildData._blockDetailIdx[i];
            int hp = _buildData._blockHp[i];
            int maxhp = _buildData._blockMaxHp[i];
            Vector3 currTrs = _buildData._blockPosition[i];
            Vector3 currRot = _buildData._blockRotation[i];

            // 3-1. 오브젝트 생성 
            BuildMaster.Instance.myBuildManger.F_CreateBlockFromSave(typeIdx, detailIdx, currTrs, currRot, hp, maxhp);
        }

        // 3-2. 블럭 로드 후 블럭 순회하면서 업데이트
        BuildMaster.Instance.myBuildManger.F_UpdateWholeBlock();

        // 3-3. 블럭 로드 후 각 model 에 접근해서 설치완료 변수를 true로
        BuildMaster.Instance.myBuildManger.F_ModelComplte();
    }

    #endregion

    #region 구조물(설치류) 저장
    public void F_SaveFurniture(Transform v_parent)
    {
        FurnitureWrapper wrapper = new FurnitureWrapper(v_parent);
        string furnitureSaveData = JsonUtility.ToJson(wrapper);
        int uid = AccountManager.Instance.uid;

        //Guest Login ( Local )
        if (uid == -1)
        {
            if (!Directory.Exists(_savePath))                           // 폴더가 있는지 확인.
                Directory.CreateDirectory(_savePath);                   // 폴더 생성
            
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

            // 0. 세이브 파일 없으면 바로 종료
            if (!File.Exists(saveFilePath))
                return;

            // 1. 파일 불러오기 ( json -> wrapper )
            furnitureSaveFile = File.ReadAllText(saveFilePath);
        }
        else
        {
            // 쿼리
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

        // 오브젝트 배치
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
