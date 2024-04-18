using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PurifierLEVEL
{
    NONE,           // 0
    SIMPLE,         // 1
}

public enum PurifierState
{
    DONOTHING,      // 생산 전
    INPROGRESS,     // 생산 중
    END             // 생산 끝
}

public class Purifier : Furniture
{
    [SerializeField] private ProduceSystem _produceSystem;

    [Header("=== Purifier LEVEL ===")]
    [SerializeField] private PurifierLEVEL _purifierLevel;  // 정제기의 수준 ( 간이, ~~, ~~ )
    [SerializeField] private int _defaultTime;              // 정제기 생산에 필요한 시간

    [Header("=== Purifier States ===")]
    [SerializeField] private PurifierState _purifierState;  // 정제기 현재 상태 ( 생산 전,후,끝 )
    [SerializeField] private int _resultItemCode;           // 생산중인 아이템 번호
    [SerializeField] private int _leftTime;                 // 생산 완료까지 남은 시간 ( 기본값 -1 )
    [SerializeField] private bool _onEnergy;                // 전기 연결 상태

    #region get
    public PurifierState purifierState => _purifierState;
    public PurifierLEVEL purifierLevel => _purifierLevel;
    public bool onEnergy => _onEnergy;
    public int resultItemCode => _resultItemCode;
    public int leftTime => _leftTime;   
    #endregion

    public void F_StartingProgress(int v_resultCode)
    {
        _purifierState = PurifierState.INPROGRESS;
        _resultItemCode = v_resultCode;
        _leftTime = _defaultTime;
        StartCoroutine(C_ProductionTimer());
    }

    IEnumerator C_ProductionTimer()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            if (!onEnergy)                              // 전기 공급 없을때
                continue;

            _leftTime--;
            if (_leftTime <= 0)                         // 남은시간 없을때
            {
                _purifierState = PurifierState.END;     // 상태 = 생산 끝
                break;                                  // 코루틴 탈출
            }
        }
            
    }

    /// <summary> 생산 완료 후 아이템 획득 으로 인한 초기화 </summary>
    public void F_InitPurifierData()
    {
        _purifierState = PurifierState.DONOTHING;
        _resultItemCode = 0;
        _leftTime = 0;
    }

    #region 초기화 함수 
    protected override void F_InitFurniture()
    {
        _produceSystem = ItemManager.Instance.produceSystem;
        _defaultTime = 10;
    }
    #endregion

    #region 상호작용 함수
    public override void F_Interaction()
    {
        // 정제기 / 인벤토리 UI 활성화
        UIManager.Instance.OnInventoryUI();         // 인벤토리 UI 활성화
        UIManager.Instance.F_OnPurifierUI(true);    // 정제기 UI 활성화

        _produceSystem._purifier_Selected = this;   // 선택된 정제기 업데이트
        _produceSystem.F_UpdateSlotUI();            // 레시피 UI 업데이트 ( 상단 
        _produceSystem.F_UpdatePurifierUI();        // 정제기 UI 업데이트 ( 하단 
    }
    #endregion

    #region 저장/불러오기 관련 함수
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
