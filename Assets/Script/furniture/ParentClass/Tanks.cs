using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaterTankLevel
{
    SIMPLE,
}

public enum TankType
{
    OXYGEN,
    WATER
}

public class TankWrapper
{
    public int _tankAmount;

    public TankWrapper(int tankAmount)
    {
        _tankAmount = tankAmount;
    }
}

public abstract class Tanks : Furniture
{
    protected Action _tankButtonEvent;

    [Header("=== Tank Information ===")]
    [SerializeField] protected int _tankAmount;         // 현재 수치
    [SerializeField] protected int _tankMaxAmount;      // 최대 수치
    [SerializeField] protected int _chargingSpeed;      // 충전 속도 ( second )
    [SerializeField] protected TankType _tankType;      // 탱크 타입 ( 산소 / 물 )
    [SerializeField] protected LayerMask _filterLayer;  // 필터 감지 레이어)
    protected bool _canClickButton;
    public float gaugeAmount => (float)_tankAmount / (float)_tankMaxAmount;
    public string gaugeText => _tankAmount + " / " + _tankMaxAmount;
    public TankType tankType => _tankType;
    protected override void F_InitFurniture()
    {
        F_InitTankData();
    }

    protected abstract void F_InitTankData();

    #region 상호작용
    public override void F_Interaction()
    {
        PlayerManager.Instance.PlayerController.F_PickupMotion();
        SoundManager.Instance.F_PlaySFX(SFXClip.OPEN);

        // 정제기 / 인벤토리 UI 활성화
        UIManager.Instance.OnInventoryUI();                                         // 인벤토리 UI 활성화
        ItemManager.Instance.produceSystem._Tank_Selected = this;                   // 선택한 탱크

        UIManager.Instance.F_OnTankUI(_tankType, _onEnergy, _onFilter, true);       // Tank UI 활성화 및 초기화
        UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);               // Tank UI 게이지 업데이트
        UIManager.Instance.F_BindingTankUIEvent(_tankButtonEvent);                  // Tank UI 버튼 이벤트 바인딩
    }
    #endregion

    #region 전원/필터기 상태 on/off
    public override void F_ChangeFilterState(bool v_state)
    {
        _onFilter = v_state;
    }

    public override void F_ChangeEnergyState(bool v_state)
    {
        _onEnergy = v_state;
    }
    #endregion

    #region 저장 / 불러오기 관련
    public override string F_GetData()
    {
        TankWrapper tankData = new TankWrapper(_tankAmount);
        string jsonData = JsonUtility.ToJson(tankData);
        string base64Data = GameManager.Instance.F_EncodeBase64(jsonData);
        return base64Data;
    }

    public override void F_SetData(string v_data)
    {
        string dataString = GameManager.Instance.F_DecodeBase64(v_data);

        TankWrapper data = JsonUtility.FromJson<TankWrapper>(dataString);

        _tankAmount = data._tankAmount;
    }
    #endregion
}
