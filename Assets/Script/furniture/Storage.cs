using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Storage : Furniture
{
    [Header("Storage Infomation")]
    [SerializeField] private int _storageSize;                  // ���丮�� ũ��
    [SerializeField] private List<ItemSlot> _slots;             // ����

    [Header("Storage itemData")]
    [SerializeField] private Item[] _items;                     // ���丮�� ������ ����
    public Item[] items => _items;

    [Space]
    [SerializeField] private Transform _storageSlotTransform;                // Slot Parent
    [SerializeField] private InventorySystem _inventorySystem;

    /// <summary> ���� �ʱ�ȭ �Լ�</summary>
    protected override void F_InitFurniture()
    {
        _items = new Item[_storageSize];
        _slots = new List<ItemSlot>();
        _inventorySystem = ItemManager.Instance.inventorySystem;

        if (_storageSize == 8)
            _storageSlotTransform = _inventorySystem.smallStorage.GetChild(0);
        else
            _storageSlotTransform = _inventorySystem.bigStorage.GetChild(0);


        // �ش� ���丮���� ����� ������ ������ �ʱ�ȭ
        for (int i = 0; i < _storageSize; i++)
            _slots.Add(_storageSlotTransform.GetChild(i).GetComponent<ItemSlot>());
    }

    /// <summary> ��ȣ�ۿ� �Լ� </summary>
    public override void F_Interaction()
    {
        // 1. ItemManager���� ���õ� ���丮���� ������Ʈ.
        ItemManager.Instance.F_SelectStorage(this);

        foreach (ItemSlot slot in _slots)
            slot.itemSlotRef = items;

        // 2. â�� ������ ���� �ֽ�ȭ
        F_StorageUIUpdate();

        // 3. â��/�κ��丮 UI Ȱ��ȭ
        UIManager.Instance.OnInventoryUI();

        if (_storageSize == 8)
            UIManager.Instance.F_OnSmallStorageUI(true);
        else
            UIManager.Instance.F_OnBigStorageUI(true);

        SoundManager.Instance.F_PlaySFX(SFXClip.OPEN);              // ���� ���� 
        PlayerManager.Instance.PlayerController.F_PickupMotion();   // �ִϸ��̼� ���
    }

    /// <summary> ���� ���� / ���뿡 ��ȭ�� ������ ����ϱ�</summary>
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

            UIManager.Instance.F_SlotFuntionUIOff();                // ������ ���� UI ����
            F_StorageUIUpdate();                                    // â�� UI ������Ʈ
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
            // item�� null ( ���� )
            if (item == null)
                continue;

            // item�� empty ( ���� )
            else if (item.F_IsEmpty())
                continue;

            // �������� ����
            else
            {
                UIManager.Instance.F_PlayerMessagePopupTEXT("���ڿ� �������� �����մϴ�.");
                return;
            }
        }

        if (ItemManager.Instance.inventorySystem.F_GetItem(_itemCode))   // �κ��丮�� ������ �߰� �õ�
        {
            SoundManager.Instance.F_PlaySFX(SFXClip.USEHAND);           // ȸ�� ���� ���
            PlayerManager.Instance.PlayerController.F_PickupMotion();   // ȸ�� �ִϸ��̼� ���

            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
            Destroy(this.gameObject);                                   // ������ ȹ�� ����
        }
    }
    #region ���� �� �ҷ�����
    public override string F_GetData()
    {
        InventoryWrapper storageData = new InventoryWrapper(ref _items);
        string jsonData = JsonUtility.ToJson(storageData);
        string base64Data = GameManager.Instance.F_EncodeBase64(jsonData);  // base64 ���ڵ�
        // * Json �����ͷ� Json �������� base64 ����ؾ���
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

