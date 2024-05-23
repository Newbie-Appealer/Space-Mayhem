using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProduceSystem : MonoBehaviour
{
    [Header("=== Systems ===")]
    InventorySystem _inventorySystem;

    [Header("=== Purifier ===")]
    [SerializeField] private Transform          _purifier_RecipeSlotParent;     // ���� ���� ��ġ ( parent )
    [SerializeField] private GameObject         _purifier_RecipeSlotPrefab;     // ���� ������
    [SerializeField] private TextMeshProUGUI    _purifier_ProgressTEXT;         // ������ ������� Text
    [SerializeField] private Image              _purifier_ResultImage;          // �����⿡�� �������� ������ �̹���
    [SerializeField] private Button             _purifier_ResultButton;         // ������ ���� �Ϸ� ��ư ( ������ ȹ�� )
    [SerializeField] private List<PurifierSlot> _purifier_Slots;                // ���� ������ ����

    [Header("Select Objects")]
    [SerializeField] public Purifier            _purifier_Selected;             // ���� ���õ� ������Ʈ ( ������ )
    [SerializeField] public Tanks               _Tank_Selected;                 // ���� ���õ� ������Ʈ ( ���� / ����� )

    private int[] _recipeNumber = { 0, 1, 2 };
    private void Start()
    {
        _inventorySystem = ItemManager.Instance.inventorySystem;
        _purifier_ResultButton.onClick.AddListener(F_ProductionCompletFunction);    // ���� �Ϸ� �� ȹ�� ��ư

        F_InitPurifier();                               // ������ ���� �ʱ�ȭ
        StartCoroutine(C_PurifierProgressUpdate());     // ������ UI ������Ʈ �ڷ�ƾ
        
        StartCoroutine(C_TankUIUpdate());
    }

    #region Purifier
    private void F_InitPurifier()
    {
        _purifier_Slots = new List<PurifierSlot>();

        for (int index = 0; index < _recipeNumber.Length; index++)
        {
            int recipeIndex = _recipeNumber[index];

            // Slot ���� �� �ʱ�ȭ
            GameObject slot_object = Instantiate(_purifier_RecipeSlotPrefab, _purifier_RecipeSlotParent);
            PurifierSlot slot_component = slot_object.GetComponent<PurifierSlot>();

            slot_component.F_InitRecipe(recipeIndex);

            // ������ ����
            _purifier_Slots.Add(slot_component);
        }
    }

    /// <summary> ���õ� �������� ���� ���� �Լ�</summary>
    public void F_PurifierStartProduction(int v_ResultItemCode, int v_stuffItemCode, int v_stuffItemCount)
    {
        // ����ó��
        if (_purifier_Selected == null)
            return;

        // 1. ����� ������ �κ��丮���� ����
        ItemManager.Instance.inventorySystem.F_CraftingItem(v_stuffItemCode,v_stuffItemCount);
        // 2. ���� ������ �������� �۾����� �Լ� ȣ��
        _purifier_Selected.F_StartingProgress(v_ResultItemCode);
        // 3. ������ ���� UI ������Ʈ
        F_UpdateSlotUI();
    }

    /// <summary> ���� �������� ���� UI�� ������Ʈ �ϴ� �ڷ�ƾ </summary>
    IEnumerator C_PurifierProgressUpdate()
    {
        while (true)
        {
            yield return null;

            // ����ó��
            if (_purifier_Selected == null)
                continue;

            // ���� UI�� �����ִ� �����϶��� UI�� ������Ʈ��������.
            if (!UIManager.Instance.onPurifier)
                continue;

            F_UpdatePurifierUI();
        }
    }

    /// <summary> ������ ������ ������Ʈ�ϴ� �Լ� </summary>
    public void F_UpdateSlotUI()
    {
        foreach(PurifierSlot slot in _purifier_Slots)
        {
            slot.F_UpdateCraftButton();     // ��ư ��� ���� ���� ������Ʈ
            slot.F_UpdateCounterTEXT();     // ���� ������ ���� ���� ������Ʈ
        }
    }

    /// <summary> �������� ����UI�� ������Ʈ�ϴ� �Լ�</summary>
    public void F_UpdatePurifierUI()
    {
        // ���� ������ ������
        if (!_purifier_Selected.onEnergy)
        {
            _purifier_ResultImage.gameObject.SetActive(false);      // ������ ������ �̹��� ��Ȱ��ȭ
            _purifier_ProgressTEXT.text = "Power OFF";              // ���� �������� ���� �޼���
            _purifier_ResultButton.gameObject.SetActive(false);     // ���� �Ϸ� ��ư ��Ȱ��ȭ
            return;
        }

        switch (_purifier_Selected.purifierState)
        {
            case PurifierState.DONOTHING:
                _purifier_ResultImage.gameObject.SetActive(false);
                _purifier_ProgressTEXT.text = "Ready for production";
                _purifier_ResultButton.gameObject.SetActive(false);
                break;
            case PurifierState.INPROGRESS:
                _purifier_ResultImage.gameObject.SetActive(true);
                _purifier_ResultImage.sprite = ResourceManager.Instance.F_GetInventorySprite(_purifier_Selected.resultItemCode);
                _purifier_ProgressTEXT.text = "wait " + _purifier_Selected.leftTime + " Second";
                _purifier_ResultButton.gameObject.SetActive(false);
                break;
            case PurifierState.END:
                _purifier_ResultImage.gameObject.SetActive(true);
                _purifier_ResultImage.sprite = ResourceManager.Instance.F_GetInventorySprite(_purifier_Selected.resultItemCode);
                _purifier_ProgressTEXT.text = "Production completed";
                _purifier_ResultButton.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary> �Ϸ� ��ư �Լ� </summary>
    private void F_ProductionCompletFunction()
    {
        // ������ �߰� �õ� ( ���� )
        if(_inventorySystem.F_GetItem(_purifier_Selected.resultItemCode))
        {
            _purifier_Selected.F_InitPurifierData();    // ������ �ʱ�ȭ ( �ʱ� ���� = ���� �� )
            F_UpdateSlotUI();                           // ���� ui ������Ʈ ( ���
            F_UpdatePurifierUI();                       // ������ ui ������Ʈ ( �ϴ�

            _inventorySystem.F_InventoryUIUpdate();     // �κ��丮 ������Ʈ
        }
    }
    #endregion

    #region tanks
    IEnumerator C_TankUIUpdate()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            
            // ������Ʈ�� ���õ� ����
            if (_Tank_Selected == null)
                continue;

            // UI�� ��������������
            if (!UIManager.Instance.onTank)
                continue;


            UIManager.Instance.F_OnTankUI(_Tank_Selected.tankType,_Tank_Selected.onEnergy, _Tank_Selected.onFilter, true);
            UIManager.Instance.F_UpdateTankGauge(_Tank_Selected.gaugeAmount, _Tank_Selected.gaugeText);
        }
    }
    #endregion
}