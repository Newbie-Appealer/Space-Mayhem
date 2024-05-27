using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    [Header("System")]
    [SerializeField] private CraftSystem _craftSystem;
    [SerializeField] private InventorySystem _inventorySystem;

    [Header("UI")]
    [SerializeField] private Image _itemImage;
    [SerializeField] private Button _craftButton;
    [SerializeField] private Transform _stuffSlotTransform;

    [SerializeField] private List<StuffSlot> _slots;

    [Header("Data")]
    [SerializeField] private Recipe _recipe;

    public void F_initStuff(Recipe v_recipe, ref GameObject v_stuffSlot)
    {
        // 1. 제작 슬롯 초기화 작업
        _craftSystem = ItemManager.Instance.craftSystem;
        _inventorySystem = ItemManager.Instance.inventorySystem;
        _recipe = v_recipe;

        _itemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(_recipe._itemCode);

        // 2. 재료 슬롯을 만들고 초기화
        for(int i = 0; i < _recipe._recipeCount.Length; i++)
        {
            StuffSlot slot = Instantiate(v_stuffSlot, _stuffSlotTransform).GetComponent<StuffSlot>();
            slot.F_InitData(_recipe._recipeCode[i], _recipe._recipeCount[i]);
            _slots.Add(slot);
        }

        // 3. 제작 버튼 클릭 이벤트에 함수 등록. ( 아이템 제작 )
        _craftButton.onClick.AddListener(F_CraftingItem);

        // 4. 해당 슬롯의 아이템 제작 가능 여부 확인 함수를 CraftSystem의 델리게이트에 추가.
        _craftSystem._craftingDelegate += F_CanCraftItem;

        if(_itemImage.GetComponent<ItemInformation>() != null)
            _itemImage.GetComponent<ItemInformation>()._slotItemCode = _recipe._itemCode;
    }

    public void F_CraftingItem()
    {
        SoundManager.Instance.F_PlaySFX(SFXClip.CLICK4);

        // 1. 인벤토리에 아이템이 추가될수 있는지 확인.
        if (_inventorySystem.F_GetItem(_recipe._itemCode))
        {
            // 인벤토리 아이템을 확인하고 재료에 해당하는 아이템을 사용.
            for (int index = 0; index < _recipe._recipeCount.Length; index++)
                _inventorySystem.F_CraftingItem(_recipe._recipeCode[index], _recipe._recipeCount[index]);
        }
    }

    public void F_CanCraftItem()
    {
        _craftButton.gameObject.SetActive(true);        // 제작버튼 활성화

        // 1. 인벤토리 내 아이템 현황을 재료 슬롯과 연동
        foreach (StuffSlot slot in _slots)
            slot.F_UpdateCounter(ItemManager.Instance.itemCounter[slot.itemCode]);

        // 2. 아이템이 충분한지 확인.
        for(int index = 0; index < _recipe._recipeCount.Length; index++)
        {
            int itemCode = _recipe._recipeCode[index];
            int itemCount = _recipe._recipeCount[index];

            // 템이 하나라도 부족하면 제작버튼 비활성화
            if (ItemManager.Instance.itemCounter[itemCode] < itemCount)
                _craftButton.gameObject.SetActive(false);

            if (_recipe._need_Installation != installation.NONE)
            {
                // @@기획에서 제거됨  리팩토링시 제거해줄것!@@
                // 필요한 구조물이 주위에 있는지 확인하고,
                // 구조물이 없으면 버튼 비활성화.
                // @@@@@@@@@@@@@@@@@@@@
            }
        }
    }
}
