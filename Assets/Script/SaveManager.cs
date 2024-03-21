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
    private string _inventorySaveFileName = "inventoryData";

    protected override void InitManager() {}

    #region 인벤토리 저장
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

    #region 구조물(설치류) 저장
    #endregion

    #region 집 저장
    #endregion
}
