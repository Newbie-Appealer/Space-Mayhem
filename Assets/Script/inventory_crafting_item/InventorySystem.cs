using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [Header("Drag and Drop")]
    [SerializeField] private Transform _quickTransform;
    [SerializeField] private Transform _slotTransform;
    [SerializeField] private Canvas _canvas;

    [Header("inventory Data")]
    [SerializeField] private int _inventorySize = 28;
    [SerializeField] private Item[] _inventory;
    public Item[] inventory => _inventory;
    public int inventorySize => _inventorySize;

    [Header("Slots")]
    [SerializeField] private List<ItemSlot> _slots;

    [Header("Crafting")]
    [SerializeField] private CraftSystem _craftSystem;

    [Space]
    [Header("Storage")]
    [SerializeField] private GameObject _smallStorage;
    public GameObject smallStorage => _smallStorage;

    [Header("tempData")]
    public int _selectIndex;
    private int _slotIndex;

    private void Awake()
    {
        // 0~7  -> 퀵 슬롯
        // 8~27 -> 인벤토리 슬롯

        _slots = new List<ItemSlot>();
        for (int i = 0; i < _quickTransform.childCount; i++)
            _slots.Add(_quickTransform.GetChild(i).GetComponent<ItemSlot>());

        for (int i = 0; i < _slotTransform.childCount; i++)
            _slots.Add(_slotTransform.GetChild(i).GetComponent<ItemSlot>());

        for(int i = 0; i < _slots.Count; i++)
            _slots[i]._slotIndex = i;

        // 인벤토리 불러오기
        SaveManager.Instance.F_LoadInventory(ref _inventory);

        _craftSystem = GetComponent<CraftSystem>();

        F_InventoryUIUpdate();
    }

    private void Update()
    {
        // 인벤토리 저장하기
        if (Input.GetKeyDown(KeyCode.K))
            SaveManager.Instance.F_SaveInventory(ref _inventory);
    }

    #region 인벤토리
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
                F_AddItem(v_code,index);
                return true; // 아이템 추가 성공
            }
            if (_inventory[index].F_IsEmpty())
            {
                F_AddItem(v_code, index);
                return true; // 아이템 추가 성공
            }
        }
        return false; // 아이템 추가 실패
    }

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

    public void F_InventoryUIUpdate()
    {
        //인벤토리 배열에 있는 데이터를 UI에 출력하는 함수
        for(int i = 0; i < _slots.Count; i++)
        {
            if (_inventory[i] == null)
                _slots[i].F_EmptySlot();

            else if (_inventory[i].F_IsEmpty())
            {
                _slots[i].F_EmptySlot();
                _inventory[i] = null;
            }

            else
                _slots[i].F_UpdateSlot(_inventory[i].itemCode, _inventory[i].currentStack);
        }
    }

    public void F_SwapItem(int v_sIndex, int v_eIndex)
    {
        // 같은 위치일 경우 동작 X
        if (v_sIndex == v_eIndex)
            return;
        
        // 비어있는 칸으로 이동
        if (inventory[v_eIndex] == null)                    // 비어있을때
        {
            inventory[v_eIndex] = inventory[v_sIndex];
            inventory[v_sIndex] = null;
        }
        else if (inventory[v_eIndex].F_IsEmpty())           // 예외처리
        {
            inventory[v_eIndex] = inventory[v_sIndex];
            inventory[v_sIndex] = null;
        }

        // 다른 아이템일때 ( 스왑 )
        else if (!inventory[v_sIndex].F_CheckItemCode(inventory[v_eIndex].itemCode))
        {
            Item tmp_item = inventory[v_eIndex];
            inventory[v_eIndex] = inventory[v_sIndex];
            inventory[v_sIndex] = tmp_item;
        }

        // 같은 아이템일때 
        else
        {
            // 스왑하는 아이템의 maxStack이 1인 경우 그냥 스왑 ( 도구 / 설치류 )
            if (inventory[v_eIndex].maxStack == 1)
            {
                Item tmp_item = inventory[v_eIndex];
                inventory[v_eIndex] = inventory[v_sIndex];
                inventory[v_sIndex] = tmp_item;
            }
            else
            {
                // 32 - 현재스택 => 더 채울수 있는 스택
                int canAddStack = inventory[v_eIndex].maxStack - inventory[v_eIndex].currentStack;
                // 채울 스택
                int stack = inventory[v_sIndex].currentStack;

                // 채워야할 스택이 더 적을때
                if(stack <= canAddStack)
                {
                    inventory[v_sIndex] = null;
                    inventory[v_eIndex].F_AddStack(stack);
                }
                // 채워야할 스택이 더 많을때
                else
                {
                    // sindex의 스택이 canAddStack만큼 줄어듬
                    inventory[v_sIndex].F_AddStack(-canAddStack);
                    // eindex의 스택이 canAddStack만큼 늘어남
                    inventory[v_eIndex].F_AddStack(canAddStack);
                }
            }
        }
        F_InventoryUIUpdate();
    }

    public void F_DeleteItem()
    {
        if (_selectIndex == -1)
            return;
        inventory[_selectIndex] = null;

        F_InventoryUIUpdate();
        UIManager.Instance.F_SlotFuntionUIOff();
        UIManager.Instance.F_UpdateItemInformation_Empty();

        ItemManager.Instance.F_UpdateItemCounter();
        _craftSystem._craftingDelegate();
    }

    public void F_UseItem(int v_slotNumber)
    {
        if (inventory[v_slotNumber] == null)            // 아이템이 없는 경우 
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        else if (inventory[v_slotNumber].F_IsEmpty())        // 아이템이 없는 경우 예외처리
            PlayerManager.Instance.F_ChangeState(PlayerState.NONE, -1);

        else
            inventory[v_slotNumber].F_UseItem();

        F_InventoryUIUpdate();
    }

    public void F_CraftingItem(int v_code, int v_count)
    {
        for(int i = 0; i < inventory.Length; i++)
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
        _craftSystem._craftingDelegate();
    }

    // 아이템 사용코드 하나 만들기 (하우징용)



    #endregion
}