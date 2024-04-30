using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Growth
{
    NEW,        // 새로 심어짐
    SPROUT,     // 새싹 
    GROWING,    // 자라는중
    END,        // 다 자람
}

public class PotatoGrower : Furniture
{
    string _tag_unInteraction = "unInteractionObject";
    string _tag_Interaction = "InteractionObject";
    int _potatoItemCode = 21;

    [Header("=== 인스펙터창에서 넣어줄것 ===")]
    [SerializeField] private Transform[] _potatoPlants;       // 감자 성장 풀떼기
    [SerializeField] private Vector3[] _pototaGrowScale;    // 감자 성장 크기 배열 [ 0, 0.3, 0.6, 1 ]

    [Header("=== PotatoGrower Information ===")]
    [SerializeField] private Growth _stageOfGrowth;                     // 성장 단계
    [SerializeField] private int _needGrowTime;                         // 성장 시간
    [SerializeField] private int _growTime;
    protected override void F_InitFurniture()
    {
        F_InitGrower();
    }

    private void F_GrowthPlants()
    {
        foreach (Transform plant in _potatoPlants)
        {
            plant.localScale = _pototaGrowScale[(int)_stageOfGrowth];
        }
    }

    IEnumerator C_Growing()
    {
        yield return new WaitForSeconds(0.2f);
        while(_stageOfGrowth != Growth.END)
        {
            yield return new WaitForSeconds(1f);
            _growTime++;

            // 성장에 필요한 시간을 다 채웠을때 -> 다음 단계로 성장
            if(_needGrowTime <= _growTime)
            {
                _stageOfGrowth++;   // 다음 단계로
                F_GrowthPlants();   // 감자 크기 성장
                _growTime = 0;      // 시간 초기화
            }
        }
        gameObject.tag = _tag_Interaction;
    }

    private void F_InitGrower()
    {
        gameObject.tag = _tag_unInteraction;        // 상호작용 방지

        _stageOfGrowth = Growth.NEW;                // 성장 단계 초기화
        F_GrowthPlants();                           // 감자 크기 초기화
        StartCoroutine(C_Growing());                // 감자 성장 코루틴 실행
    }

    #region 상호작용 ( 수확 )
    public override void F_Interaction()
    {
        if (_stageOfGrowth != Growth.END)
            return;

        if (ItemManager.Instance.inventorySystem.F_GetItem(_potatoItemCode))    // 아이템 획득 시도
        {
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); // 인벤트로 업데이트

            F_InitGrower();                                     // 성장 초기화
            UIManager.Instance.F_IntercationPopup(false, "");   // 상호작용 UI 가리기
        }
    }
    #endregion

    #region 저장 / 불러오기 
    public override string F_GetData()
    {
        string jsonData = "NONE";
        return jsonData;
    }

    public override void F_SetData(string v_data)
    {

    }
    #endregion
}
