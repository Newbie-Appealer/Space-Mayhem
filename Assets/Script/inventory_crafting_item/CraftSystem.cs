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
        // 1. �ʱ�ȭ �۾�
        _inventory = ItemManager.Instance.inventorySystem;
        _itemCounter = new int[ItemManager.Instance.ItemDatas.Count];

        // 2. ���� ���� �� �ʱ�ȭ
        foreach (Recipe recipe in ItemManager.Instance.recipes)
        {
            CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
            slot.F_initStuff(recipe, ref _stuffSlot);
            _craftingSlots.Add(slot);
        }

        // 3. ���� ������ �ִ� �������� �迭�� �����ϴ� �Լ��� UiManager�� delegate�� �߰�.
        UIManager.Instance.F_AddInventoryFunction(
            new UIManager.inventoryDelegate(F_UpdateItemCounter));

        // 4. ���� ������ �ֽ�ȭ �ϴ� �Լ��� UImanager �� delegate�� �߰�.
        UIManager.Instance.F_AddInventoryFunction(
            new UIManager.inventoryDelegate(() => _craftingDelegate()));
    }

    public void F_UpdateItemCounter()
    {
        // 1. ���� 0���� �ʱ�ȭ
        for(int i = 0; i <  _itemCounter.Length; i++)
            _itemCounter[i] = 0;

        // 2. �κ��丮�� �������� ������ ����.
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

    /// <summary> Slot ����� ��������Ʈ�� �߰��ϴ� �Լ� </param>
    public void F_AddSlotFunction(CraftingDelegate v_func)
    {
        _craftingDelegate += v_func;
    }
}
