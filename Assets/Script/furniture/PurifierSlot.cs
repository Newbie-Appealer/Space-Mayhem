using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurifierSlot : MonoBehaviour
{
    [Header("System")]
    [SerializeField] private ProduceSystem      _produceSystem;     // 생산 시스템 참조

    [Header("=== Recipe Data ===")]
    [SerializeField] private Recipe             _recipe;            // 해당 슬롯이 만드는 아이템 레시피

    [Header("=== UI ===")]
    [SerializeField] private Image              _stuffImage;        // 필요한 아이템 이미지
    [SerializeField] private Image              _resultImage;       // 제작 아이템 이미지
    [SerializeField] private TextMeshProUGUI    _itemCounterTEXT;   // 필요 개수 / 보유 개수 텍스트
    [SerializeField] private Button             _craftButton;       // 제작 시작 버튼
    public void F_InitRecipe(int v_recipeIndex)
    {
        _produceSystem  = ItemManager.Instance.produceSystem;
        _recipe         = ItemManager.Instance.recipes[v_recipeIndex];

        _stuffImage.sprite      = ResourceManager.Instance.F_GetInventorySprite(_recipe._recipeCode[0]);
        _resultImage.sprite     = ResourceManager.Instance.F_GetInventorySprite(_recipe._itemCode);
        _itemCounterTEXT.text   = "- / -";

        // 현재 선택된 정제기의 함수를 실행시킴
        int resultItemCode = _recipe._itemCode;
        int stuffItemCode = _recipe._recipeCode[0];
        int stuffItemCount = _recipe._recipeCount[0];
        _craftButton.onClick.AddListener(
            () => _produceSystem.F_PurifierStartProduction(resultItemCode, stuffItemCode, stuffItemCount));

        // 테스트
        F_ItemInformation();
    }

    /// <summary> 버튼 상태 업데이트 해야할때 호출하기</summary>
    public void F_UpdateCraftButton()
    {
        // UI 켜질때, 아이템 생성버튼 눌렀을때, 아이템 획득버튼 눌렀을때
        _craftButton.gameObject.SetActive(false);

        Purifier purifier = _produceSystem._purifier_Selected;

        // 예외처리
        if (purifier == null)
            return;

        if(purifier.purifierState == PurifierState.DONOTHING && purifier.onEnergy)
        {
            int haveCount = ItemManager.Instance.itemCounter[_recipe._recipeCode[0]];
            int needCount = _recipe._recipeCount[0] / ((int)purifier.purifierLevel + 1);

            // 필요한 재료가 있으면
            if(haveCount >= needCount)
            {
                _craftButton.gameObject.SetActive(true);
            }
        }
    }

    /// <summary> 아이템 개수 업데이트 해야할때 호출하기.</summary>
    public void F_UpdateCounterTEXT()
    {
        // UI 켜질때, 아이템 생성버튼 눌렀을때, 아이템 획득버튼 눌렀을때  호출하기

        int haveCount = ItemManager.Instance.itemCounter[_recipe._recipeCode[0]];
        int needCount = _recipe._recipeCount[0] / ((int)_produceSystem._purifier_Selected.purifierLevel + 1);

        _itemCounterTEXT.text = needCount + " / " + haveCount;
    }


    private void F_ItemInformation()
    {
        ItemInformation info1 = _stuffImage.GetComponent<ItemInformation>();
        ItemInformation info2 = _resultImage.GetComponent<ItemInformation>();

        if(info1 != null)
            info1._slotItemCode = _recipe._recipeCode[0];
        if (info2 != null)
            info2._slotItemCode = _recipe._itemCode;
    }
}