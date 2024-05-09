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

public class PurifierWrapper
{
    public int _purifierState;
    public int _resultItemCode;
    public int _leftTime;

    public PurifierWrapper(int v_purifierState, int v_resultItemCode, int v_leftTime)
    {
        _purifierState = v_purifierState;
        _resultItemCode = v_resultItemCode;
        _leftTime = v_leftTime;
    }
}

public class Purifier : Furniture
{
    [Space]
    [Header("=== SYSTEM ===")]
    [SerializeField] private ProduceSystem _produceSystem;

    [Header("=== Purifier LEVEL ===")]
    [SerializeField] private PurifierLEVEL _purifierLevel;  // 정제기의 수준 ( 간이, ~~, ~~ )
    [SerializeField] private int _defaultTime;              // 정제기 생산에 필요한 시간

    [Header("=== Purifier States ===")]
    [SerializeField] private PurifierState _purifierState;  // 정제기 현재 상태 ( 생산 전,후,끝 )
    [SerializeField] private int _resultItemCode;           // 생산중인 아이템 번호
    [SerializeField] private int _leftTime;                 // 생산 완료까지 남은 시간 ( 기본값 -1 )

    #region get
    public PurifierState purifierState => _purifierState;
    public PurifierLEVEL purifierLevel => _purifierLevel;
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

    public override void F_ChangeEnergyState(bool v_state)
    {
        _onEnergy = v_state;
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
        SoundManager.Instance.F_PlaySFX(SFXClip.OPEN);              // 오픈 사운드
        PlayerManager.Instance.PlayerController.F_PickupMotion();   // 애니메이션 재생

        // 정제기 / 인벤토리 UI 활성화
        UIManager.Instance.OnInventoryUI();         // 인벤토리 UI 활성화
        UIManager.Instance.F_OnPurifierUI(true);    // 정제기 UI 활성화

        _produceSystem._purifier_Selected = this;   // 선택된 정제기 업데이트
        _produceSystem.F_UpdateSlotUI();            // 레시피 UI 업데이트 ( 상단 
        _produceSystem.F_UpdatePurifierUI();        // 정제기 UI 업데이트 ( 하단 
    }

    public override void F_TakeFurniture()
    {
        // 동작하고있지않을때만 회수 가능
        if(_purifierState == PurifierState.DONOTHING)
        {
            if (ItemManager.Instance.inventorySystem.F_GetItem(_itemCode))   // 인벤토리에 아이템 추가 시도
            {
                SoundManager.Instance.F_PlaySFX(SFXClip.USEHAND);           // 회수 사운드 재생
                PlayerManager.Instance.PlayerController.F_PickupMotion();   // 회수 애니메이션 재생

                ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();

                Destroy(this.gameObject);                                   // 아이템 획득 성공
            }
        }
    }
    #endregion

    #region 저장/불러오기 관련 함수
    public override string F_GetData()
    {
        PurifierWrapper purifierData = new PurifierWrapper((int)_purifierState, _resultItemCode, _leftTime);
        string jsonData = JsonUtility.ToJson(purifierData);
        string Base64Data = GameManager.Instance.F_EncodeBase64(jsonData);
        return Base64Data;
    }

    public override void F_SetData(string v_data)
    {
        string dataString = GameManager.Instance.F_DecodeBase64(v_data);

        PurifierWrapper data = JsonUtility.FromJson<PurifierWrapper>(dataString);

        _purifierState = (PurifierState)data._purifierState;
        _resultItemCode = data._resultItemCode;
        _leftTime = data._leftTime;

        if (_purifierState == PurifierState.INPROGRESS)
            StartCoroutine(C_ProductionTimer());
    }
    #endregion
}
