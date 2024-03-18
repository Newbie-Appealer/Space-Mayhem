using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    [Header("System")]
    [SerializeField] private CraftSystem _craftSystem;

    [Header("UI")]
    [SerializeField] private Image _itemImage;
    [SerializeField] private Button _craftButton;
    [SerializeField] private Transform _stuffSlotTransform;

    [SerializeField] private List<StuffSlot> _slots;

    [Header("Data")]
    [SerializeField] private Recipe _recipe;

    public void F_initStuff(CraftSystem v_system, Recipe v_recipe, ref GameObject v_stuffSlot)
    {
        // 1. ���� ���� �ʱ�ȭ �۾�
        _craftSystem = v_system;
        _recipe = v_recipe;
        _itemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(_recipe._itemCode);

        // 2. ��� ������ ����� �ʱ�ȭ
        for(int i = 0; i < _recipe._recipeCount.Length; i++)
        {
            StuffSlot slot = Instantiate(v_stuffSlot, _stuffSlotTransform).GetComponent<StuffSlot>();
            slot.F_InitData(_recipe._recipeCode[i], _recipe._recipeCount[i]);
            _slots.Add(slot);
        }

        // 3. �ش� ������ ������ ���� ���� ���� Ȯ�� �Լ��� CraftSystem�� ��������Ʈ�� �߰�.
        _craftSystem.F_AddSlotFunction(new CraftSystem.CraftingDelegate(F_CanCraftItem)); 
    }

    public void F_CanCraftItem()
    {
        _craftButton.interactable = true;           // ��ư Ȱ��ȸ

        // 1. �κ��丮 �� ������ ��Ȳ�� ��� ���԰� ����
        foreach (StuffSlot slot in _slots)
            slot.F_UpdateCounter(_craftSystem.itemCounter[slot.itemCode]);

        // 2. �������� ������� Ȯ��.
        for(int index = 0; index < _recipe._recipeCount.Length; index++)
        {
            int itemCode = _recipe._recipeCode[index];
            int itemCount = _recipe._recipeCount[index];

            // ���� �ϳ��� �����ϸ� ��ư ��Ȱ��ȸ
            if (_craftSystem.itemCounter[itemCode] < itemCount)
                _craftButton.interactable = false;

            if (_recipe._need_Installation != installation.NONE)
            {
                // �ʿ��� �������� �ִ��� Ȯ���ϰ�,
                // �������� ������ ��ư ��Ȱ��ȭ.
            }
        }
    }
}
