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

    [Header("Crafting Slot")]
    [SerializeField] private List<CraftingSlot> _craftingSlots;
    private void Start()
    {
        // 1. 초기화 작업
        _inventory = ItemManager.Instance.inventorySystem;

        // 2. 슬롯 생성 및 초기화
        foreach (Recipe recipe in ItemManager.Instance.recipes)
        {
            CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
            slot.F_initStuff(recipe, ref _stuffSlot);
            _craftingSlots.Add(slot);
        }

        // 3. 제작 슬롯을 최신화 하는 함수를 UImanager 의 delegate에 추가.
        UIManager.Instance.F_AddInventoryFunction(
            new UIManager.inventoryDelegate(() => _craftingDelegate()));
    }

    /// <summary> Slot 기능을 델리게이트에 추가하는 함수 </param>
    public void F_AddSlotFunction(CraftingDelegate v_func)
    {
        _craftingDelegate += v_func;
    }
}
