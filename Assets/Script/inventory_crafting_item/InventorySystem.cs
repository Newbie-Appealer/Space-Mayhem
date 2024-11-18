using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class InventorySystem : MonoBehaviour
{
    [Header("Drag and Drop")]
    [SerializeField] private Transform _quickTransform;
    [SerializeField] private Transform _slotTransform;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _smallStorage;
    [SerializeField] private Transform _bigStorage;

    public Transform smallStorage => _smallStorage;
    public Transform bigStorage => _bigStorage;

    [Header("inventory Data")]
    [SerializeField] private int _inventorySize = 28;
    [SerializeField] private Item[] _inventory;
    public Item[] inventory => _inventory;
    public int inventorySize => _inventorySize;

    [Header("Slots")]
    [SerializeField] private List<ItemSlot> _slots;
    [SerializeField] private int _selectQuickSlotNumber;
    public int selectQuickSlotNumber => _selectQuickSlotNumber;
    [Header("Crafting")]
    [SerializeField] private CraftSystem _craftSystem;


    [Header("tempData")]
    public int _selectIndex;
    public SlotType _selectSlotType;

    private void Awake()
    {
        // SaveManager의 save델리게이트에 save등록
        SaveManager.Instance.GameDataSave += () => SaveManager.Instance.F_SaveInventory(ref _inventory);

        // 1. 인벤토리 불러오기
        SaveManager.Instance.F_LoadInventory(ref _inventory);

        // 2. 슬롯 초기화
        // 0~7  -> 퀵 슬롯 8~27 -> 인벤토리 슬롯
        _slots = new List<ItemSlot>();
        for (int i = 0; i < _quickTransform.childCount; i++)
            _slots.Add(_quickTransform.GetChild(i).GetComponent<ItemSlot>());
        for (int i = 0; i < _slotTransform.childCount; i++)
            _slots.Add(_slotTransform.GetChild(i).GetComponent<ItemSlot>());
        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i]._slotIndex = i;
            _slots[i].itemSlotRef = _inventory;
            _slots[i]._slotType = SlotType.INVENTORY;
        }

        _craftSystem = GetComponent<CraftSystem>();

        F_InventoryUIUpdate();

        // Storage Slot 초기화
        for (int i = 0; i < _smallStorage.GetChild(0).childCount; i++)
        {
            ItemSlot slot = _smallStorage.GetChild(0).GetChild(i).GetComponent<ItemSlot>();
            slot._slotIndex = i;
            slot._slotType = SlotType.STORAGE;
        }

        for (int i = 0; i < _bigStorage.GetChild(0).childCount; i++)
        {
            ItemSlot slot = _bigStorage.GetChild(0).GetChild(i).GetComponent<ItemSlot>();
            slot._slotIndex = i;
            slot._slotType = SlotType.STORAGE;
        }
    }
    #region 인벤토리
    /// <summary> 아이템 획득 시도 함수( 성공 여부 반환 )</summary>
    public bool F_GetItem(int v_code)
    {
        // 동일한 아이템 /
        for(int index = 0; index < _inventorySize; index++)
        {
            if (_inventory[index] == null)   // 빈칸
                continue;
            if (_inventory[index].F_IsEmpty())  // 빈칸 버그 예외처리
                continue;

            // 동일한 아이템 + 아이템 개수 추가 가능할때
            if (_inventory[index].F_CheckItemCode(v_code) && _inventory[index].F_CheckStack())
            {
                _inventory[index].F_AddStack(1);
                return true;
            }
        }

        // 동일한 아이템이 없을때
        for(int index = 0; index < _inventorySize; index++)
        {
            if (_inventory[index] == null)
            {
                F_AddItem(v_code, index);
                return true; // 아이템 추가 성공
            }
            if (_inventory[index].F_IsEmpty())
            {
                F_AddItem(v_code, index);
                return true; // 아이템 추가 성공
            }
        }

        UIManager.Instance.F_PlayerMessagePopupTEXT("인벤토리가 가득 찼습니다.");
        return false; // 아이템 추가 실패
    }

    /// <summary> 인벤토리에 아이템을 추가하는 함수</summary>
    /// <param name="v_code">추가할 아이템 번호</param>
    /// <param name="v_index">아이템이 인벤토리내 추가될 위치</param>
    public void F_AddItem(int v_code, int v_index)
    {
        ItemData data = ItemManager.Instance.ItemDatas[v_code];

        switch (data._itemType)
        {
            case ItemType.STUFF:
                _inventory[v_index] = new Stuff(data);
                break;

            case ItemType.FOOD:
                _inventory[v_index] = new Food(data);
                break;

            case ItemType.TOOL:
                _inventory[v_index] = new Tool(data);
                break;

            case ItemType.INSTALL:
                _inventory[v_index] = new Install(data);
                break;
        }
    }

    /// <summary> 
    /// 인벤토리 UI 업데이트 
    /// 인벤토리 배열의 데이터를 UI에 출력하는 함수
    /// </summary>
    public void F_InventoryUIUpdate()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_inventory[i] == null)
                _slots[i].F_EmptySlot();

            else if (_inventory[i].F_IsEmpty())
            {
                _slots[i].F_EmptySlot();
                _inventory[i] = null;
            }

            else
                _slots[i].F_UpdateSlot(_inventory[i].itemCode, _inventory[i].currentStack, i);
        }
    }

    /// <summary> ItemSlot간 아이템 스왑 함수 </summary>
    public void F_SwapItem(int v_sIndex, int v_eIndex,ref Item[] v_from,ref Item[] v_target)
    {
        Item[] from = v_from;
        Item[] target = v_target;

        // 같은 배열 참조 && 같은 위치 => 예외처리
        if (from == target && v_sIndex == v_eIndex)
            return;
        
        // 비어있는 칸 찾기
        if (target[v_eIndex] == null)
        {
            target[v_eIndex] = from[v_sIndex];
            from[v_sIndex] = null;
        }

        // 비어있는 칸 찾기
        else if (target[v_eIndex].F_IsEmpty())
        {
            target[v_eIndex] = from[v_sIndex];
            from[v_sIndex] = null;
        }

        // 다른 아이템일때 ( 스왑 )
        else if (!from[v_sIndex].F_CheckItemCode(target[v_eIndex].itemCode))
        {
            Item tmp_item = target[v_eIndex];
            target[v_eIndex] = from[v_sIndex];
            from[v_sIndex] = tmp_item;
        }

        // 같은 아이템일때 
        else
        {
            // 스왑하는 아이템의 maxStack이 1인 경우 그냥 스왑 ( 도구 / 설치류 )
            if (target[v_eIndex].maxStack == 1)
            {
                Item tmp_item = target[v_eIndex];
                target[v_eIndex] = from[v_sIndex];
                from[v_sIndex] = tmp_item;
            }
            else
            {
                // 32 - 현재스택 => 더 채울수 있는 스택
                int canAddStack = target[v_eIndex].maxStack - target[v_eIndex].currentStack;
                // 채울 스택
                int stack = from[v_sIndex].currentStack;

                // 채워야할 스택이 더 적을때
                if(stack <= canAddStack)
                {
                    from[v_sIndex] = null;
                    target[v_eIndex].F_AddStack(stack);
                }
                // 채워야할 스택이 더 많을때
                else
                {
                    // sindex의 스택이 canAddStack만큼 줄어듬
                    from[v_sIndex].F_AddStack(-canAddStack);
                    // eindex의 스택이 canAddStack만큼 늘어남
                    target[v_eIndex].F_AddStack(canAddStack);
                }
            }
        }

        F_InventoryUIUpdate();
        ItemManager.Instance.F_UpdateItemCounter();             // 아이템 현항 업데이트
        if(ItemManager.Instance.selectedStorage != null)
        {
            ItemManager.Instance.selectedStorage.F_StorageUIUpdate();
        }
    }

    /// <summary> 아이템 삭제 함수</summary>
    public void F_DeleteItem()
    {
        SoundManager.Instance.F_PlaySFX(SFXClip.CLICK4);

        if (_selectIndex == -1 || _selectSlotType == SlotType.NONE)
            return;

        switch(_selectSlotType)
        {
            case SlotType.STORAGE:
                ItemManager.Instance.selectedStorage.items[_selectIndex] = null;    // 아이템 삭제
                ItemManager.Instance.selectedStorage.F_StorageUIUpdate();           // 창고 현황 업데이트
                break;
            case SlotType.INVENTORY:
                inventory[_selectIndex] = null;                         // 아이템 삭제
                F_InventoryUIUpdate();                                  // 인벤토리 현황 업데이트
                ItemManager.Instance.F_UpdateItemCounter();             // 아이템 현항 업데이트
                _craftSystem._craftingDelegate();                       // 제작 관련 업데이트
                break;
        }

        UIManager.Instance.F_UpdateItemInformation_Empty();     // 아이템 툴팁 초기화
        UIManager.Instance.F_SlotFuntionUIOff();                // 아이템 삭제 UI 끄기
    }

    /// <summary> 아이템 분할 함수</summary>
    public void F_DivisionItem()
    {
        SoundManager.Instance.F_PlaySFX(SFXClip.CLICK4);

        if (_selectIndex == -1 || _selectSlotType == SlotType.NONE)
            return;

        switch(_selectSlotType)
        {
            case SlotType.STORAGE:
                ItemManager.Instance.selectedStorage.F_DivisionStorageItem(_selectIndex);            
                break;
            case SlotType.INVENTORY:
                F_DivisionInventoryItem();
                break;
        }
    }

    /// <summary> 인벤토리 아이템을 분할하는 함수</summary>
    public void F_DivisionInventoryItem()
    {
        int stack = _inventory[_selectIndex].currentStack;
        if (stack == 1)
            return;

        int decreaseStack = stack / 2;
        int increaseStack = stack / 2;

        int itemCode = _inventory[_selectIndex].itemCode;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (_inventory[i] == null)   // 빈칸
                F_AddItem(itemCode, i);

            else if (_inventory[i].F_IsEmpty())  // 빈칸 버그 예외처리
                F_AddItem(itemCode, i);

            else
                continue;

            _inventory[_selectIndex].F_AddStack(-decreaseStack);
            _inventory[i].F_AddStack(increaseStack - 1);

            UIManager.Instance.F_SlotFuntionUIOff();                // 아이템 삭제 UI 끄기
            F_InventoryUIUpdate();                                  // 인벤토리 UI 업데이트
            break;
        }
    }

    /// <summary> 퀵슬롯 아이템 사용 함수 </summary>
    /// <param name="v_slotNumber"> 퀵슬롯 번호</param>
    public void F_UseItem(int v_slotNumber)
    {
        _selectQuickSlotNumber = v_slotNumber;
        PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        // -1을 매개변수로 넣어서 F_UseItem을 실행시키면  아무것도 들지않은 상태로.
        if (_selectQuickSlotNumber == -1)       
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        if (inventory[_selectQuickSlotNumber] == null)            // 아이템이 없는 경우 
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        else if (inventory[_selectQuickSlotNumber].F_IsEmpty())        // 아이템이 없는 경우 예외처리
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        else
            inventory[_selectQuickSlotNumber].F_UseItem();

        UIManager.Instance.F_QuickSlotFocus(_selectQuickSlotNumber);
        F_InventoryUIUpdate();
    }

    // 사용한 아이템 인벤에서 갯수 빼기
    // 아이템은 인벤토리에 존재한다는 가정하에
    public void F_UpdateItemUsing(int v_code, int v_count ) 
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
                continue;
            if (inventory[i].F_IsEmpty())
                continue;
            if (inventory[i].itemCode != v_code)
                continue;
            // 현재 슬롯의 아이템의 개수가 필요개수 이상일때
            if (inventory[i].currentStack >= v_count)
            {
                inventory[i].F_AddStack(-v_count);

                // 슬롯의 아이템개수가 0이하가 되면 아이템 슬롯 비우기.
                if (inventory[i].F_IsEmpty())
                    inventory[i] = null;

                break;
            }

            // 슬롯의 아이템 개수가 부족할때.
            else
            {
                // 슬롯의 개수만큼 v_count에서 제외하고 슬롯 비우기
                v_count -= inventory[i].currentStack;
                inventory[i] = null;
            }
        }

        F_InventoryUIUpdate();
        ItemManager.Instance.F_UpdateItemCounter();
    }

    // craft 용 아이템 소모코드
    public void F_CraftingItem(int v_code, int v_count)
    {
        F_UpdateItemUsing(v_code, v_count);
        _craftSystem._craftingDelegate();
    }
    #endregion
}