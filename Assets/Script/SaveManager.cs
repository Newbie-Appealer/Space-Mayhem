using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;

#region 세이브 데이터 감싸는곳
// 인벤토리 데이터 Wrapper
public class InventoryWrapper
{
    public List<int> _itemCodes;
    public List<int> _itemStacks;
    public List<int> _itemSlotIndexs;
    public List<int> _itemTypes;

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
    public List<Vector3> _blockPosition;
    public List<Vector3> _blockRotation;

    public BuildingWrapper( Transform v_parnet ) 
    {
        // 1. List 초기화 
        _blocktypeIdx = new List<int>();    
        _blockDetailIdx = new List<int>();
        _blockHp = new List<int>();
        _blockPosition = new List<Vector3>();
        _blockRotation = new List<Vector3>();   

        // 2.저장할 데이터 정리 
        for (int i = 0; i < v_parnet.childCount; i++)
        { 
            MyBuildingBlock _m = v_parnet.GetChild(i).GetComponent<MyBuildingBlock>();

            _blocktypeIdx.Add( _m.MyBlockTypeIdx );
            _blockDetailIdx.Add( _m.MyBlockDetailIdx );
            _blockHp.Add( _m.MyBlockHp );
            _blockPosition.Add( _m.MyPosition);
            _blockRotation.Add( _m.MyRotation );

        }

    }

}

// 플레이어 데이터 Wrapper
public class PlauerWrapper
    {
        // 저장해야할것
        // 1. 플레이어 산소/물/허기 수치 ( float float float )
        // 2. 플레이어 스토리 진행현황 ( int )
        // 3. 플레이어 레시피 해금현황 ( int )

        // 스토리/레시피 현황 추가됐을때 작성할예정.
    }
#endregion

public class SaveManager : Singleton<SaveManager>
{
    private string _savePath => Application.persistentDataPath + "/saves/";      // 세이브 파일 저장 임시 폴더
    private string _inventorySaveFileName   = "inventoryData";
    private string _buildSaveFileName       = "buildingData";

    protected override void InitManager() {}

    #region 인벤토리 저장
    // 인벤토리 save
    public void F_SaveInventory(ref Item[] v_inventory)
    {
        if(!Directory.Exists(_savePath))                                            // 폴더가 있는지 확인.
            Directory.CreateDirectory(_savePath);                                   // 폴더 생성

        InventoryWrapper a = new InventoryWrapper(ref v_inventory);                 // 데이터 감싸기.
        string saveData = JsonUtility.ToJson(a);

        Debug.Log(saveData);

        string saveFilePath = _savePath + _inventorySaveFileName + ".json";
        File.WriteAllText(saveFilePath, saveData);

        Debug.Log("Save inventory");
    }
    // 인벤토리 load
    public void F_LoadInventory(ref Item[] v_inventory)
    {
        // 세이브 파일 위치
        string saveFilePath = _savePath + _inventorySaveFileName + ".json";

        // 1.인벤토리 배열 초기화
        v_inventory = new Item[ItemManager.Instance.inventorySystem.inventorySize];

        // 2. 세이브파일이 없으면 바로 종료
        if (!File.Exists(saveFilePath))
            return;

        // 3. 세이브파일 읽기 ( json )
        string saveFile = File.ReadAllText(saveFilePath);

        // 4. 세이브파일 변환 ( json -> inventoryWrapper )
        InventoryWrapper tmpData = JsonUtility.FromJson<InventoryWrapper>(saveFile);

        // 5. 인벤토리에 데이터 로드.
        for (int index = 0; index < tmpData._itemCodes.Count; index++)
        {
            int itemCode = tmpData._itemCodes[index];
            int itemStack = tmpData._itemStacks[index];
            int itemSlotIndex = tmpData._itemSlotIndexs[index];
            float itemDurabiloity = tmpData._itemDurability[index];

            // 인벤토리에 아이템 추가. ( 저장한 데이터로 )
            ItemManager.Instance.inventorySystem.F_AddItem(itemCode, itemSlotIndex);

            // 처음 아이템 생성시 기본 스택이 1 이라서 -1 해줘야함!
            v_inventory[itemSlotIndex].F_AddStack(itemStack - 1);  

            // 도구 데이터 초기화 ( 내구도 )
            if (itemDurabiloity > 0)     
            {
                (v_inventory[itemSlotIndex] as Tool).F_InitDurability(itemDurabiloity);
            }
        }
    }

    #endregion

    #region 하우징 저장
    // 하우징 save
    public void F_SaveBuilding( Transform v_blockParent ) 
    {
        if (!Directory.Exists(_savePath))                           // 폴더가 있는지 확인.
            Directory.CreateDirectory(_savePath);                   // 폴더 생성

        BuildingWrapper ba = new BuildingWrapper(v_blockParent);      // 저장할 클래스 new
        string buildSaveData = JsonUtility.ToJson(ba);              // 클래스를 json으로 변환 (string 타입으로 )

        Debug.Log(buildSaveData);

        string saveFilePath = _savePath + _buildSaveFileName + ".json"; // 내가 지정한 경로에
        File.WriteAllText(saveFilePath, buildSaveData);                 // file작성 ( 경로, 세이브 데이터 stinrg )

        Debug.Log("Your Building Is Saved");
    }
    // 하우징 load
    public void F_LoadBuilding( Transform v_blockParent ) 
    {
        Debug.Log("Building Is Loading");
        
        // 세이브 파일 위치
        string _saveLocation = _savePath + _buildSaveFileName + ".json";
        
        // 0. 세이브 파일 없으면 바로 종료
        if (!File.Exists(_saveLocation))
        {
            // 0.1 Building Manager의 기본 9개 블럭 생성하기
            MyBuildManager.Instance.F_FirstInitBaseFloor();
            // 블럭 로드 후 순회하면서 업데이트 
            MyBuildManager.Instance.F_UpdateWholeBlock();

            return;
        }

        // 1. 세이브 파일 읽기 (위치)
        string _buildSaveFile = File.ReadAllText(_saveLocation);

        // 2. 세이브 파일 변환 ( json -> BuildingWapper )
        BuildingWrapper _buildData = JsonUtility.FromJson<BuildingWrapper>(_buildSaveFile);  // string형 file을 <T> 타입으로 변환 

        // 3. block 로드
        for (int i = 0; i < _buildData._blocktypeIdx.Count; i++) 
        {
            int typeIdx         = _buildData._blocktypeIdx[i];
            int detailIdx       = _buildData._blockDetailIdx[i];
            int hp              = _buildData._blockHp[i];
            Vector3 currTrs     = _buildData._blockPosition[i];
            Vector3 currRot     = _buildData._blockRotation[i];

            // 3-1. 오브젝트 생성 
            MyBuildManager.Instance.F_CreateBlockFromSave( typeIdx , detailIdx , currTrs , currRot ,  hp);
        }

        // 블럭 로드 후 블럭 순회하면서 업데이트
        MyBuildManager.Instance.F_UpdateWholeBlock();
    }

    #endregion

    #region 구조물(설치류) 저장
    #endregion

}
