using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftSystem : MonoBehaviour
{
    public delegate void CraftingDelegate();
    public CraftingDelegate _craftingDelegate;

    [Header("Prefabs")]
    [SerializeField] private GameObject _craftSlot;
    [SerializeField] private GameObject _stuffSlot;

    [Header("Content")]
    [SerializeField] private Transform[] _category;

    [Header("inventory")]
    [SerializeField] private InventorySystem _inventory;
    [SerializeField] private int[] _itemCounter;
    public int[] itemCounter => _itemCounter;

    [Header("Crafting Slot")]
    [SerializeField] private List<CraftingSlot> _craftingSlots;
    private void Start()
    {
        // 1. 초기화 작업
        _inventory = ItemManager.Instance.inventorySystem;
        _itemCounter = new int[ItemManager.Instance.ItemDatas.Count];

        // 2. 슬롯 생성 및 초기화
        foreach (Recipe recipe in ItemManager.Instance.recipes)
        {
            CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
            slot.F_initStuff(recipe, ref _stuffSlot);
            _craftingSlots.Add(slot);
        }

        // 3. 현재 가지고 있는 아이템을 배열로 정리하는 함수를 UiManager의 delegate에 추가.
        UIManager.Instance.F_AddInventoryFunction(
            new UIManager.inventoryDelegate(F_UpdateItemCounter));

        // 4. 제작 슬롯을 최신화 하는 함수를 UImanager 의 delegate에 추가.
        UIManager.Instance.F_AddInventoryFunction(
            new UIManager.inventoryDelegate(() => _craftingDelegate()));
    }

    public void F_UpdateItemCounter()
    {
        // 1. 값을 0으로 초기화
        for(int i = 0; i <  _itemCounter.Length; i++)
            _itemCounter[i] = 0;

        // 2. 인벤토리내 아이템의 개수를 정리.
        for(int index = 0; index  < _inventory.inventory.Length; index++)
        {
            if (_inventory.inventory[index] == null)
                continue;
            if (_inventory.inventory[index].currentStack == 0)
                continue;

            int item = _inventory.inventory[index].itemCode;
            int itemStack = _inventory.inventory[index].currentStack;
            _itemCounter[item] += itemStack;
        }
    }

    /// <summary> Slot 기능을 델리게이트에 추가하는 함수 </param>
    public void F_AddSlotFunction(CraftingDelegate v_func)
    {
        _craftingDelegate += v_func;
    }
}
