using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CraftSystem : MonoBehaviour
{
    public delegate void CraftingDelegate();
    public CraftingDelegate _craftingDelegate;      // 제작 UI 슬롯 기능 델리게이트 체인

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
        // 1. 초기화 작업 ( inventorySystem 컴포넌트 가져오기 )
        _inventory = ItemManager.Instance.inventorySystem;

        // 2. 슬롯 생성 및 초기화
        foreach (Recipe recipe in ItemManager.Instance.recipes)
        {
            CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
            slot.F_initStuff(recipe, ref _stuffSlot);
            _craftingSlots.Add(slot);
        }

        // 3. 제작 슬롯을 최신화 하는 함수 델리게이트 등록
        UIManager.Instance.OnInventoryUI += () => _craftingDelegate();

        // 4. 해금 레시피 추가
        F_InitUnlockRecipe();   
    }

    private void F_InitUnlockRecipe()
    {
        for(int i = 0; i < GameManager.Instance.unlockRecipeStep; i++)
        {
            // 해금레시피 범위를 넘어가면 
            if (i >= ItemManager.Instance.unlockrecipes.Count)
                return;

            Recipe recipe = ItemManager.Instance.unlockrecipes[i];

            CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
            slot.F_initStuff(recipe, ref _stuffSlot);
            _craftingSlots.Add(slot);
        }
    }

    /// <summary> 해금 레시피 추가 ( GameManager.instance.unlockRecipeStep 증가할때마다 호출하기 ) </summary>
    public void F_AddUnlockRecipe(int v_index)
    {
        // 해금 레시피 범위를 넘어가면
        if (v_index > ItemManager.Instance.unlockrecipes.Count)
            return;

        Recipe recipe = ItemManager.Instance.unlockrecipes[v_index - 1];
          
        CraftingSlot slot = Instantiate(_craftSlot, _category[(int)recipe._itemType]).GetComponent<CraftingSlot>();
        slot.F_initStuff(recipe, ref _stuffSlot);
        _craftingSlots.Add(slot);
    }
}
