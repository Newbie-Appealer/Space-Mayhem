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
    [SerializeField] private ProduceSystem      _produceSystem;     // ���� �ý��� ����

    [Header("=== Recipe Data ===")]
    [SerializeField] private Recipe             _recipe;            // �ش� ������ ����� ������ ������

    [Header("=== UI ===")]
    [SerializeField] private Image              _stuffImage;        // �ʿ��� ������ �̹���
    [SerializeField] private Image              _resultImage;       // ���� ������ �̹���
    [SerializeField] private TextMeshProUGUI    _itemCounterTEXT;   // �ʿ� ���� / ���� ���� �ؽ�Ʈ
    [SerializeField] private Button             _craftButton;       // ���� ���� ��ư
    public void F_InitRecipe(int v_recipeIndex)
    {
        _produceSystem  = ItemManager.Instance.produceSystem;
        _recipe         = ItemManager.Instance.recipes[v_recipeIndex];

        _stuffImage.sprite      = ResourceManager.Instance.F_GetInventorySprite(_recipe._recipeCode[0]);
        _resultImage.sprite     = ResourceManager.Instance.F_GetInventorySprite(_recipe._itemCode);
        _itemCounterTEXT.text   = "- / -";

        // ���� ���õ� �������� �Լ��� �����Ŵ
        int resultItemCode = _recipe._itemCode;
        int stuffItemCode = _recipe._recipeCode[0];
        int stuffItemCount = _recipe._recipeCount[0];
        _craftButton.onClick.AddListener(
            () => _produceSystem.F_PurifierStartProduction(resultItemCode, stuffItemCode, stuffItemCount));

        // �׽�Ʈ
        F_ItemInformation();
    }

    /// <summary> ��ư ���� ������Ʈ �ؾ��Ҷ� ȣ���ϱ�</summary>
    public void F_UpdateCraftButton()
    {
        // UI ������, ������ ������ư ��������, ������ ȹ���ư ��������
        _craftButton.gameObject.SetActive(false);

        Purifier purifier = _produceSystem._purifier_Selected;

        // ����ó��
        if (purifier == null)
            return;

        if(purifier.purifierState == PurifierState.DONOTHING && purifier.onEnergy)
        {
            int haveCount = ItemManager.Instance.itemCounter[_recipe._recipeCode[0]];
            int needCount = _recipe._recipeCount[0] / ((int)purifier.purifierLevel + 1);

            // �ʿ��� ��ᰡ ������
            if(haveCount >= needCount)
            {
                _craftButton.gameObject.SetActive(true);
            }
        }
    }

    /// <summary> ������ ���� ������Ʈ �ؾ��Ҷ� ȣ���ϱ�.</summary>
    public void F_UpdateCounterTEXT()
    {
        // UI ������, ������ ������ư ��������, ������ ȹ���ư ��������  ȣ���ϱ�

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