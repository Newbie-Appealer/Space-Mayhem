using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum installation 
{
    NONE,       // n
    TEST
}

public enum CraftCategory
{
    STUFF,
    FOOD,
    TOOL,
    INSTALL
}

[System.Serializable]
public struct Recipe
{
    public int _itemCode;                       // ���� ������ ��ȣ
    public CraftCategory _itemType;             // ���� ������ Ÿ��
    public installation _need_Installation;     // ���� Ȱ��ȭ�� �ʿ��� ��ġ��

    public int[] _recipeCode;                   // ���ۿ� �ʿ��� ������ ��ȣ
    public int[] _recipeCount;                  // ���ۿ� �ʿ��� ������ ����

    // _�������ڵ� [0] 
    // _������cnt  [0]
    // �ε��� ��ȣ�� ����
}

public class CraftSystem : MonoBehaviour
{
    public delegate void CraftingDelegate();
    CraftingDelegate _craftingDelegate;

    [Header("Recipe Data")]
    [SerializeField] private List<Recipe> _recipes;

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
        foreach (Recipe recipe in _recipes)
        {
            CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
            slot.F_initStuff(this, recipe, ref _stuffSlot);
            _craftingSlots.Add(slot);
        }

        // 3. ���� ������ �ִ� �������� �迭�� �����ϴ� �Լ��� UiManager�� delegate�� �߰�.
        UIManager.Instance.F_AddInventoryFunction(
            new UIManager.inventoryDelegate(F_UpdateItemCounter));

        // 4. ���� ������ �ֽ�ȭ �ϴ� �Լ��� UImanager �� delegate�� �߰�.
        UIManager.Instance.F_AddInventoryFunction(
            new UIManager.inventoryDelegate(F_SlotFunction));
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

    #region delegate
    /// <summary> CraftSystem�� Slot����� ���� ��������Ʈ�� �����ϴ� �Լ�.</summary>
    public void F_SlotFunction()
    {
        _craftingDelegate();
    }
    /// <summary> Slot ����� ��������Ʈ�� �߰��ϴ� �Լ� </param>
    public void F_AddSlotFunction(CraftingDelegate v_func)
    {
        _craftingDelegate += v_func;
    }
    #endregion
}
