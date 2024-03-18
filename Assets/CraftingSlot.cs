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
        // 1. 제작 슬롯 초기화 작업
        _craftSystem = v_system;
        _recipe = v_recipe;
        _itemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(_recipe._itemCode);

        // 2. 재료 슬롯을 만들고 초기화
        for(int i = 0; i < _recipe._recipeCount.Length; i++)
        {
            StuffSlot slot = Instantiate(v_stuffSlot, _stuffSlotTransform).GetComponent<StuffSlot>();
            slot.F_InitData(_recipe._recipeCode[i], _recipe._recipeCount[i]);
            _slots.Add(slot);
        }

        // 3. 해당 슬롯의 아이템 제작 가능 여부 확인 함수를 CraftSystem의 델리게이트에 추가.
        _craftSystem.F_AddSlotFunction(new CraftSystem.CraftingDelegate(F_CanCraftItem)); 
    }

    public void F_CanCraftItem()
    {
        _craftButton.interactable = true;           // 버튼 활성회

        // 1. 인벤토리 내 아이템 현황을 재료 슬롯과 연동
        foreach (StuffSlot slot in _slots)
            slot.F_UpdateCounter(_craftSystem.itemCounter[slot.itemCode]);

        // 2. 아이템이 충분한지 확인.
        for(int index = 0; index < _recipe._recipeCount.Length; index++)
        {
            int itemCode = _recipe._recipeCode[index];
            int itemCount = _recipe._recipeCount[index];

            // 템이 하나라도 부족하면 버튼 비활성회
            if (_craftSystem.itemCounter[itemCode] < itemCount)
                _craftButton.interactable = false;

            if (_recipe._need_Installation != installation.NONE)
            {
                // 필요한 구조물이 있는지 확인하고,
                // 구조물이 없으면 버튼 비활성화.
            }
        }
    }
}
