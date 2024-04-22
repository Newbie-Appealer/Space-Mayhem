using System.Collections;
using System.Collections.Generic;
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
        // 1. �ʱ�ȭ �۾�
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
    }

    // #TODO:�رݷ����� ���� ��� �����ϱ�
    // �ر� �ܰ�� PlayerManager�� ���� ��������.
}
