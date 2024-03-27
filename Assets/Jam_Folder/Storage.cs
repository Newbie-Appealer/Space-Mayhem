using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [Header("Storage Infomation")]
    [SerializeField] private int _storageSize;                  // 스토리지 크기
    [SerializeField] private List<ItemSlot> _slots;             // 슬롯

    [Header("Storage itemData")]
    [SerializeField] private Item[] _items;                     // 스토리지 아이템 정보
    public Item[] items => _items;

    [Space]
    [SerializeField] private Transform _storageSlotTransform;                // Slot Parent
    [SerializeField] private InventorySystem _inventorySystem;
    private void Start()
    {
        _items = new Item[_storageSize]; 
        _slots = new List<ItemSlot>();
        _inventorySystem = ItemManager.Instance.inventorySystem;
        _storageSlotTransform = _inventorySystem.smallStorage;

        // 해당 스토리지가 사용할 아이템 슬롯을 초기화
        for (int i = 0; i < _storageSize; i++)
            _slots.Add(_storageSlotTransform.GetChild(i).GetComponent<ItemSlot>());
    }

    /// <summary> 상자 열때 / 내용에 변화가 있을때 사용하기</summary>
    public void F_StorageUIUpdate()
    {
        for(int i = 0; i < _slots.Count; i++)
        {
            if (_items[i] == null)
                _slots[i].F_EmptySlot();
            else if (_items[i].F_IsEmpty())
            {
                _slots[i].F_EmptySlot();
                _items[i] = null;
            }
            else
            {
                _slots[i].F_UpdateSlot(_items[i].itemCode, _items[i].currentStack);
            }   
        }
    }

    public void F_OpenStorage()
    {
        // 1. ItemManager에서 선택된 스토리지를 업데이트.
        ItemManager.Instance.F_SelectStorage(this);

        // 2. 창고 아이템 내용 최신화
        F_StorageUIUpdate();

        // 3. 창고/인벤토리 UI 활성화    --> UIManager
        // 4. 제작 UI 비활성화           --> UIManager
    }

    /// <summary> 창고에서 창고로 아이템 스왑 함수 </summary>
    public void F_SwapItem(int v_sIndex, int v_eIndex)
    {
        // 같은 위치일 경우 동작 X
        if (v_sIndex == v_eIndex)
            return;

        // 비어있는 칸으로 이동
        if (_items[v_eIndex] == null)                    // 비어있을때
        {
            _items[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = null;
        }
        else if (_items[v_eIndex].F_IsEmpty())           // 예외처리
        {
            _items[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = null;
        }

        // 다른 아이템일때 ( 스왑 )
        else if (!_items[v_sIndex].F_CheckItemCode(_items[v_eIndex].itemCode))
        {
            Item tmp_item = _items[v_eIndex];
            _items[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = tmp_item;
        }

        // 같은 아이템일때 
        else
        {
            // 스왑하는 아이템의 maxStack이 1인 경우 그냥 스왑 ( 도구 / 설치류 )
            if (_items[v_eIndex].maxStack == 1)
            {
                Item tmp_item = _items[v_eIndex];
                _items[v_eIndex] = _items[v_sIndex];
                _items[v_sIndex] = tmp_item;
            }
            else
            {
                // 32 - 현재스택 => 더 채울수 있는 스택
                int canAddStack = _items[v_eIndex].maxStack - _items[v_eIndex].currentStack;
                // 채울 스택
                int stack = _items[v_sIndex].currentStack;

                // 채워야할 스택이 더 적을때
                if (stack <= canAddStack)
                {
                    _items[v_sIndex] = null;
                    _items[v_eIndex].F_AddStack(stack);
                }
                // 채워야할 스택이 더 많을때
                else
                {
                    // sindex의 스택이 canAddStack만큼 줄어듬
                    _items[v_sIndex].F_AddStack(-canAddStack);
                    // eindex의 스택이 canAddStack만큼 늘어남
                    _items[v_eIndex].F_AddStack(canAddStack);
                }
            }
        }
        F_StorageUIUpdate();
    }

    /// <summary> 창고에서 인벤토리로 아이템 스왑 함수 </summary>
    public void F_SwapItemToInven(int v_sIndex, int v_eIndex)
    {
        // 같은 위치일 경우 동작 X
        if (v_sIndex == v_eIndex)
            return;

        // 비어있는 칸으로 이동
        if (_inventorySystem.inventory[v_eIndex] == null)                    // 비어있을때
        {
            _inventorySystem.inventory[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = null;
        }
        else if (_inventorySystem.inventory[v_eIndex].F_IsEmpty())           // 예외처리
        {
            _inventorySystem.inventory[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = null;
        }

        // 다른 아이템일때 ( 스왑 )
        else if (!_items[v_sIndex].F_CheckItemCode(_inventorySystem.inventory[v_eIndex].itemCode))
        {
            Item tmp_item = _inventorySystem.inventory[v_eIndex];
            _inventorySystem.inventory[v_eIndex] = _items[v_sIndex];
            _items[v_sIndex] = tmp_item;
        }

        // 같은 아이템일때 
        else
        {
            // 스왑하는 아이템의 maxStack이 1인 경우 그냥 스왑 ( 도구 / 설치류 )
            if (_inventorySystem.inventory[v_eIndex].maxStack == 1)
            {
                Item tmp_item = _inventorySystem.inventory[v_eIndex];
                _inventorySystem.inventory[v_eIndex] = _items[v_sIndex];
                _items[v_sIndex] = tmp_item;
            }
            else
            {
                // 32 - 현재스택 => 더 채울수 있는 스택
                int canAddStack = _inventorySystem.inventory[v_eIndex].maxStack - _inventorySystem.inventory[v_eIndex].currentStack;
                // 채울 스택
                int stack = _items[v_sIndex].currentStack;

                // 채워야할 스택이 더 적을때
                if (stack <= canAddStack)
                {
                    _items[v_sIndex] = null;
                    _inventorySystem.inventory[v_eIndex].F_AddStack(stack);
                }
                // 채워야할 스택이 더 많을때
                else
                {
                    // sindex의 스택이 canAddStack만큼 줄어듬
                    _items[v_sIndex].F_AddStack(-canAddStack);
                    // eindex의 스택이 canAddStack만큼 늘어남
                    _inventorySystem.inventory[v_eIndex].F_AddStack(canAddStack);
                }
            }
        }

        F_StorageUIUpdate();
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
    }

}
