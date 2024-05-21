using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CraftSystem : MonoBehaviour
{
    public delegate void CraftingDelegate();
    public CraftingDelegate _craftingDelegate;      // ���� UI ���� ��� ��������Ʈ ü��

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
        // 1. �ʱ�ȭ �۾� ( inventorySystem ������Ʈ �������� )
        _inventory = ItemManager.Instance.inventorySystem;

        // 2. ���� ���� �� �ʱ�ȭ
        foreach (Recipe recipe in ItemManager.Instance.recipes)
        {
            CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
            slot.F_initStuff(recipe, ref _stuffSlot);
            _craftingSlots.Add(slot);
        }

        // 3. ���� ������ �ֽ�ȭ �ϴ� �Լ� ��������Ʈ ���
        UIManager.Instance.OnInventoryUI += () => _craftingDelegate();

        // 4. �ر� ������ �߰�
        F_InitUnlockRecipe();   
    }

    private void F_InitUnlockRecipe()
    {
        for(int i = 0; i < GameManager.Instance.unlockRecipeStep; i++)
        {
            // �رݷ����� ������ �Ѿ�� 
            if (i >= ItemManager.Instance.unlockrecipes.Count)
                return;

            Recipe recipe = ItemManager.Instance.unlockrecipes[i];

            CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
            slot.F_initStuff(recipe, ref _stuffSlot);
            _craftingSlots.Add(slot);
        }
    }

    /// <summary> �ر� ������ �߰� ( GameManager.instance.unlockRecipeStep �����Ҷ����� ȣ���ϱ� ) </summary>
    public void F_AddUnlockRecipe(int v_index)
    {
        // �ر� ������ ������ �Ѿ��
        if (v_index > ItemManager.Instance.unlockrecipes.Count)
            return;

        Recipe recipe = ItemManager.Instance.unlockrecipes[v_index - 1];
          
        CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
        slot.F_initStuff(recipe, ref _stuffSlot);
        _craftingSlots.Add(slot);
    }
}
