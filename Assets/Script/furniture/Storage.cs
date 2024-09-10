using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Storage : Furniture
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

    /// <summary> 상자 초기화 함수</summary>
    protected override void F_InitFurniture()
    {
        _items = new Item[_storageSize];
        _slots = new List<ItemSlot>();
        _inventorySystem = ItemManager.Instance.inventorySystem;

        if (_storageSize == 8)
            _storageSlotTransform = _inventorySystem.smallStorage.GetChild(0);
        else
            _storageSlotTransform = _inventorySystem.bigStorage.GetChild(0);


        // 해당 스토리지가 사용할 아이템 슬롯을 초기화
        for (int i = 0; i < _storageSize; i++)
            _slots.Add(_storageSlotTransform.GetChild(i).GetComponent<ItemSlot>());
    }

    /// <summary> 상호작용 함수 </summary>
    public override void F_Interaction()
    {
        // 1. ItemManager에서 선택된 스토리지를 업데이트.
        ItemManager.Instance.F_SelectStorage(this);

        foreach (ItemSlot slot in _slots)
            slot.itemSlotRef = items;

        // 2. 창고 아이템 내용 최신화
        F_StorageUIUpdate();

        // 3. 창고/인벤토리 UI 활성화
        UIManager.Instance.OnInventoryUI();

        if (_storageSize == 8)
            UIManager.Instance.F_OnSmallStorageUI(true);
        else
            UIManager.Instance.F_OnBigStorageUI(true);

        SoundManager.Instance.F_PlaySFX(SFXClip.OPEN);              // 오픈 사운드 
        PlayerManager.Instance.PlayerController.F_PickupMotion();   // 애니메이션 재생
    }

    /// <summary> 상자 열때 / 내용에 변화가 있을때 사용하기</summary>
    public void F_StorageUIUpdate()
    {
        for (int i = 0; i < _slots.Count; i++)
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
                _slots[i].F_UpdateSlot(_items[i].itemCode, _items[i].currentStack, i);
            }
        }
    }

    public void F_DivisionStorageItem(int v_index)
    {
        int stack = _items[v_index].currentStack;
        if (stack == 1)
            return;

        int decreaseStack = stack / 2;
        int increaseStack = stack / 2;

        int itemCode = _items[v_index].itemCode;
        for(int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
                F_AddStorageItem(itemCode, i);

            else if (_items[i].F_IsEmpty())
                F_AddStorageItem(itemCode, i);

            else
                continue;

            _items[v_index].F_AddStack(-decreaseStack);
            _items[i].F_AddStack(increaseStack - 1);

            UIManager.Instance.F_SlotFuntionUIOff();                // 아이템 삭제 UI 끄기
            F_StorageUIUpdate();                                    // 창고 UI 업데이트
            break;
        }
    }

    private void F_AddStorageItem(int v_code, int v_index)
    {
        ItemData data = ItemManager.Instance.ItemDatas[v_code];

        switch (data._itemType)
        {
            case ItemType.STUFF:
                _items[v_index] = new Stuff(data);
                break;

            case ItemType.FOOD:
                _items[v_index] = new Food(data);
                break;

            case ItemType.TOOL:
                _items[v_index] = new Tool(data);
                break;

            case ItemType.INSTALL:
                _items[v_index] = new Install(data);
                break;
        }
    }

    public override void F_TakeFurniture()
    {
        foreach(var item in _items)
        {
            // item이 null ( 없음 )
            if (item == null)
                continue;

            // item이 empty ( 없음 )
            else if (item.F_IsEmpty())
                continue;

            // 아이템이 있음
            else
            {
                UIManager.Instance.F_PlayerMessagePopupTEXT("상자에 아이템이 존재합니다.");
                return;
            }
        }

        if (ItemManager.Instance.inventorySystem.F_GetItem(_itemCode))   // 인벤토리에 아이템 추가 시도
        {
            SoundManager.Instance.F_PlaySFX(SFXClip.USEHAND);           // 회수 사운드 재생
            PlayerManager.Instance.PlayerController.F_PickupMotion();   // 회수 애니메이션 재생

            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
            Destroy(this.gameObject);                                   // 아이템 획득 성공
        }
    }
    #region 저장 및 불러오기
    public override string F_GetData()
    {
        InventoryWrapper storageData = new InventoryWrapper(ref _items);
        string jsonData = JsonUtility.ToJson(storageData);
        string base64Data = GameManager.Instance.F_EncodeBase64(jsonData);  // base64 인코딩
        // * Json 데이터로 Json 넣을려면 base64 사용해야함
        return base64Data;
    }

    public override void F_SetData(string v_data)
    {
        //_items = new Item[_storageSize];
        string dataString = GameManager.Instance.F_DecodeBase64(v_data);

        if (dataString == "NONE")
            return;

        InventoryWrapper data = JsonUtility.FromJson<InventoryWrapper>(dataString);

        for(int i = 0; i < data._itemCodes.Count; i++)
        {
            int itemCode = data._itemCodes[i];
            int itemStack = data._itemStacks[i];
            int itemSlotIndex = data._itemSlotIndexs[i];
            float itemDurability = data._itemDurability[i];

            F_AddStorageItem(itemCode, itemSlotIndex);
            _items[itemSlotIndex].F_AddStack(itemStack - 1);

            if (itemDurability > 0)
            {
                (_items[itemSlotIndex] as Tool).F_InitDurability(itemDurability);
            }
        }
    }

    #endregion
}

