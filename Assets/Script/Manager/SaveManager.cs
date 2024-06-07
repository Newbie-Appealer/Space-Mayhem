using JetBrains.Annotations;
using System;
using System.Collections;
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
    public float _oxygen;
    public float _water;
    public float _hunger;

    // 2. 게임 진행도 ( 해금 / 스토리 진행도 )
    public int _unlockRecipeStep;   // 레시피 해금 단계
    public int _storyStep;          // 스토리 진행도 ( 난이도 )

    // 3. 옵션 ( 사운드 )
    public float _volumeMaster;
    public float _volumeBGM;
    public float _volumeSFX;

    // 4. 옵션 ( 마우스 )
    public float _mouseSensitivity;
    public PlayerWrapper(PlayerData v_data, int v_unlockRecipeStep, int v_storyStep, float v_volumeM, float v_volumeB, float v_volumeS, float v_mouseSensitivity)
    {
        _oxygen = v_data._oxygen;
        _water = v_data._water;
        _hunger = v_data._hunger;

        _unlockRecipeStep = v_unlockRecipeStep;
        _storyStep = v_storyStep;

        _volumeMaster = v_volumeM;
        _volumeBGM = v_volumeB;
        _volumeSFX = v_volumeS;

        _mouseSensitivity = v_mouseSensitivity;
    }
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
    public delegate void SaveDelegate();
    public SaveDelegate GameDataSave;

    // Local Data
    private string _savePath => Application.persistentDataPath + "/saves/";      // 세이브 파일 저장 임시 폴더
    private string _inventorySaveFileName   = "inventoryData";
    private string _buildSaveFileName       = "buildingData";
    private string _furnitureSaveFileName   = "furnitureData";
    private string _playerSaveFileName      = "playerData";

    private string _dataTableName = "gamedata";

    protected override void InitManager() 
    {
        StartCoroutine(C_AutoSave());
    }

    #region 인벤토리 저장
    // 인벤토리 save
    public void F_SaveInventory(ref Item[] v_inventory)
    {
        InventoryWrapper a = new InventoryWrapper(ref v_inventory);                 // 데이터 감싸기.
        string saveData = JsonUtility.ToJson(a);
        int uid = AccountManager.Instance.uid;              // 플레이어 고유 번호

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
                BuildMaster.Instance.F_FirstInitBaseFloor();

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
                    BuildMaster.Instance.F_FirstInitBaseFloor();

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
            BuildMaster.Instance.F_CreateBlockFromSave(typeIdx, detailIdx, currTrs, currRot, hp, maxhp);

        }

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

    #region 플레이어 저장
    // 플레이어 수치
    public void F_SavePlayerData(PlayerData v_data)
    {
        PlayerWrapper wrapper = new PlayerWrapper(
            v_data,                                                     // 플레이어 데이터 ( 산소 물 허기 )
            GameManager.Instance.unlockRecipeStep,                      // 레시피 해금 진행도
            GameManager.Instance.storyStep,                             // 스토리 진행도
            SoundManager.Instance.masterValue,                          // 사운드 ( master )
            SoundManager.Instance.bgmValue,                             // 사운드 ( bgm )
            SoundManager.Instance.sfxValue,                             // 사운드 ( sfx )
            PlayerManager.Instance.PlayerController.mouseSensitivity    // 마우스 ( 감도 )
            );

        string saveData = JsonUtility.ToJson(wrapper);
        int uid = AccountManager.Instance.uid;

        // Guest Login ( LOCAL )
        if (uid == -1)
        {
            if (!Directory.Exists(_savePath))                                            // 폴더가 있는지 확인.
                Directory.CreateDirectory(_savePath);                                   // 폴더 생성

            string saveFilePath = _savePath + _playerSaveFileName + ".json";
            File.WriteAllText(saveFilePath, saveData);

            Debug.Log("Save PlayerData ( Local )");
        }

        // Login ( DB )
        else
        {
            string query = string.Format("UPDATE {0} SET PlayerData = '{1}' WHERE UID = {2}",
                _dataTableName, saveData, uid);

            DBConnector.Instance.F_Update(query);

            Debug.Log("Save PlayerData ( DB )");
        }
    }

    public void F_LoadPlayerData(ref PlayerData v_data)
    {
        v_data = new PlayerData(100, 100, 100);             // 데이터 초기화

        int uid = AccountManager.Instance.uid;              // 플레이어 고유 번호
        string saveFile;
        PlayerWrapper tmpData = null;
        // 1. 인벤토리 배열 초기화
        // Guest Login ( LOCAL )
        if (uid == -1)
        {
            string saveFilePath = _savePath + _playerSaveFileName + ".json"; // 세이브 파일 위치

            // 2. 세이브파일이 없으면 바로 종료
            if (!File.Exists(saveFilePath))
                return;

            // 3. 세이브파일 읽기 ( json )
            saveFile = File.ReadAllText(saveFilePath);

            // 4. 세이브파일 변환 ( json -> PlayerWrapper )
            tmpData = JsonUtility.FromJson<PlayerWrapper>(saveFile);

            Debug.Log("Load playerData ( Local )");
        }

        // Login ( DB )
        else
        {
            // 쿼리문
            string query = string.Format("SELECT PlayerData From {0} where uid = {1}",
                _dataTableName, uid);

            DataSet data = DBConnector.Instance.F_Select(query, _dataTableName);
            foreach (DataRow row in data.Tables[0].Rows)
            {
                saveFile = row["PlayerData"].ToString();     // 세이브 데이터 string으로 가져옴

                // 세이브 파일이 없으면 바로 종료
                if (saveFile == "NONE")
                    return;

                tmpData = JsonUtility.FromJson<PlayerWrapper>(saveFile); // json 변환
                break;
            }
        }

        // 플레이어 데이터 초기화
        v_data._oxygen = tmpData._oxygen;
        v_data._water = tmpData._water;
        v_data._hunger = tmpData._hunger;

        // 레시피해금 / 스토리 진행도
        GameManager.Instance.storyStep = tmpData._storyStep;
        GameManager.Instance.unlockRecipeStep = tmpData._unlockRecipeStep;
        MeteorManager.Instance.F_DifficultyUpdate();

        // 사운드 설정 초기화
        SoundManager.Instance.masterValue = tmpData._volumeMaster;
        SoundManager.Instance.bgmValue = tmpData._volumeBGM;
        SoundManager.Instance.sfxValue = tmpData._volumeSFX;

        // 마우스 설정 초기화
        PlayerManager.Instance.PlayerController.mouseSensitivity = tmpData._mouseSensitivity;
    }
    // 스토리 진행도  ( int )
    // 레시피 해금 진행도 ( int )
    #endregion

    #region 자동 저장 코루틴
    IEnumerator C_AutoSave()
    {
        while(true)
        {
            yield return new WaitForSeconds(60f);
            GameDataSave();
            Debug.Log("Auto Save Data");
        }

    }
    #endregion

    public void F_ResetLocalData()
    {
        int uid = AccountManager.Instance.uid;

        // LOCAL
        if (uid == -1)
        {
            string inventory_saveFilePath = _savePath + _inventorySaveFileName + ".json";
            string build_saveFilePath = _savePath + _buildSaveFileName + ".json";
            string furniture_saveFilePath = _savePath + _furnitureSaveFileName + ".json";
            string player_saveFilePath = _savePath + _playerSaveFileName + ".json";

            File.Delete(inventory_saveFilePath);
            File.Delete(build_saveFilePath);
            File.Delete(furniture_saveFilePath);
            File.Delete(player_saveFilePath);
        }

        // DB
        else
        {
            string query1 = string.Format("UPDATE {0} SET InventoryData = '{1}' WHERE UID = {2}",
               _dataTableName, "NONE", uid);
            DBConnector.Instance.F_Update(query1);

            string query2 = string.Format("UPDATE {0} SET HousingData = '{1}' WHERE UID = {2}",
                _dataTableName, "NONE", uid);
            DBConnector.Instance.F_Update(query2);

            string query3 = string.Format("UPDATE {0} SET FurnitureData = '{1}' WHERE UID = {2}",
                _dataTableName, "NONE", uid);
            DBConnector.Instance.F_Update(query3);

            string query4 = string.Format("UPDATE {0} SET PlayerData = '{1}' WHERE UID = {2}",
                _dataTableName, "NONE", uid);
            DBConnector.Instance.F_Update(query4);
        }
    }
}
