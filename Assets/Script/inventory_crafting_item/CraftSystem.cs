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
        // 1. �ʱ�ȭ �۾�
        _inventory = ItemManager.Instance.inventorySystem;

        // 2. ���� ���� �� �ʱ�ȭ
        foreach (Recipe recipe in ItemManager.Instance.recipes)
        {
            CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
            slot.F_initStuff(recipe, ref _stuffSlot);
            _craftingSlots.Add(slot);
        }

        // 3. ���� ������ �ֽ�ȭ �ϴ� �Լ��� UImanager �� delegate�� �߰�.
        UIManager.Instance.F_AddInventoryFunction(
            new UIManager.inventoryDelegate(() => _craftingDelegate()));
    }

    /// <summary> Slot ����� ��������Ʈ�� �߰��ϴ� �Լ� </param>
    public void F_AddSlotFunction(CraftingDelegate v_func)
    {
        _craftingDelegate += v_func;
    }
}
