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
    [SerializeField] private Transform          _purifier_RecipeSlotParent;     // 슬롯 생성 위치 ( parent )
    [SerializeField] private GameObject         _purifier_RecipeSlotPrefab;     // 슬롯 프리팹
    [SerializeField] private TextMeshProUGUI    _purifier_ProgressTEXT;         // 정제기 진행상태 Text
    [SerializeField] private Image              _purifier_ResultImage;          // 정제기에서 생산중인 아이템 이미지
    [SerializeField] private Button             _purifier_ResultButton;         // 정제기 생산 완료 버튼 ( 아이템 획득 )
    [SerializeField] private List<PurifierSlot> _purifier_Slots;                // 제작 레시피 슬롯

    [Header("Select Objects")]
    [SerializeField] public Purifier            _purifier_Selected;             // 현재 선택된 오브젝트 ( 정제기 )
    [SerializeField] public Tanks               _Tank_Selected;                 // 현재 선택된 오브잭트 ( 물탱 / 산소탱 )

    private int[] _recipeNumber = { 0, 1, 2 };
    private void Start()
    {
        _inventorySystem = ItemManager.Instance.inventorySystem;
        _purifier_ResultButton.onClick.AddListener(F_ProductionCompletFunction);    // 생산 완료 및 획득 버튼

        F_InitPurifier();                               // 정제기 관련 초기화
        StartCoroutine(C_PurifierProgressUpdate());     // 정제기 UI 업데이트 코루틴
        
        StartCoroutine(C_TankUIUpdate());
    }

    #region Purifier
    private void F_InitPurifier()
    {
        _purifier_Slots = new List<PurifierSlot>();

        for (int index = 0; index < _recipeNumber.Length; index++)
        {
            int recipeIndex = _recipeNumber[index];

            // Slot 생성 및 초기화
            GameObject slot_object = Instantiate(_purifier_RecipeSlotPrefab, _purifier_RecipeSlotParent);
            PurifierSlot slot_component = slot_object.GetComponent<PurifierSlot>();

            slot_component.F_InitRecipe(recipeIndex);

            // 데이터 저장
            _purifier_Slots.Add(slot_component);
        }
    }

    /// <summary> 선택된 정제기의 생산 시작 함수</summary>
    public void F_PurifierStartProduction(int v_ResultItemCode, int v_stuffItemCode, int v_stuffItemCount)
    {
        // 예외처리
        if (_purifier_Selected == null)
            return;

        // 1. 사용한 아이템 인벤토리에서 삭제
        ItemManager.Instance.inventorySystem.F_CraftingItem(v_stuffItemCode,v_stuffItemCount);
        // 2. 현재 선택한 정제기의 작업시작 함수 호출
        _purifier_Selected.F_StartingProgress(v_ResultItemCode);
        // 3. 레시피 슬롯 UI 업데이트
        F_UpdateSlotUI();
    }

    /// <summary> 현재 정제기의 상태 UI를 업데이트 하는 코루틴 </summary>
    IEnumerator C_PurifierProgressUpdate()
    {
        while (true)
        {
            yield return null;

            // 예외처리
            if (_purifier_Selected == null)
                continue;

            // 현재 UI가 꺼져있는 상태일때는 UI를 업데이트하지않음.
            if (!UIManager.Instance.onPurifier)
                continue;

            F_UpdatePurifierUI();
        }
    }

    /// <summary> 슬롯의 정보를 업데이트하는 함수 </summary>
    public void F_UpdateSlotUI()
    {
        foreach(PurifierSlot slot in _purifier_Slots)
        {
            slot.F_UpdateCraftButton();     // 버튼 사용 가능 여부 업데이트
            slot.F_UpdateCounterTEXT();     // 현재 아이템 개수 여부 업데이트
        }
    }

    /// <summary> 정제기의 상태UI를 업데이트하는 함수</summary>
    public void F_UpdatePurifierUI()
    {
        // 전기 공급이 없을때
        if (!_purifier_Selected.onEnergy)
        {
            _purifier_ResultImage.gameObject.SetActive(false);      // 생산중 아이템 이미지 비활성화
            _purifier_ProgressTEXT.text = "Power OFF";              // 현재 정제기의 상태 메세지
            _purifier_ResultButton.gameObject.SetActive(false);     // 생산 완료 버튼 비활성화
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

    /// <summary> 완료 버튼 함수 </summary>
    private void F_ProductionCompletFunction()
    {
        // 아이템 추가 시도 ( 성공 )
        if(_inventorySystem.F_GetItem(_purifier_Selected.resultItemCode))
        {
            _purifier_Selected.F_InitPurifierData();    // 정제기 초기화 ( 초기 상태 = 생산 전 )
            F_UpdateSlotUI();                           // 슬롯 ui 업데이트 ( 상단
            F_UpdatePurifierUI();                       // 정제기 ui 업데이트 ( 하단

            _inventorySystem.F_InventoryUIUpdate();     // 인벤토리 업데이트
        }
    }
    #endregion

    #region tanks
    IEnumerator C_TankUIUpdate()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            
            // 오브젝트가 선택된 상태
            if (_Tank_Selected == null)
                continue;

            // UI가 켜져있지않을때
            if (!UIManager.Instance.onTank)
                continue;


            UIManager.Instance.F_OnTankUI(_Tank_Selected.tankType,_Tank_Selected.onEnergy, _Tank_Selected.onFilter, true);
            UIManager.Instance.F_UpdateTankGauge(_Tank_Selected.gaugeAmount, _Tank_Selected.gaugeText);
        }
    }
    #endregion
}