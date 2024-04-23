using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OxygenTank : Furniture
{
    Action _TankButtonEvent;

    [Header("=== Oxygen Tank Information ===")]
    [SerializeField] private int _tankAmount;     // 현재 수치
    [SerializeField] private int _tankMaxAmount;  // 최대 수치

    private bool _canClickButton;
    private float gaugeAmount => (float)_tankAmount / (float)_tankMaxAmount;
    private string gaugeText => _tankAmount + " / " + _tankMaxAmount;

    protected override void F_InitFurniture()
    {
        _TankButtonEvent = () => F_ClickEvent();

        _tankMaxAmount = 100;
        _tankAmount = 100;      // 저장해야할것.

        _canClickButton = true;
    }

    #region UI 버튼 이벤트 함수
    public void F_ClickEvent()
    {
        if (_canClickButton)
        {
            _canClickButton = false;
            StartCoroutine(C_HealOxygen());
            UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);
        }
    }

    IEnumerator C_HealOxygen()
    {
        float canHealAmount = 100 - PlayerManager.Instance.F_GetStat(0);

        // 플레이어가 회복할수있는 수치보다  탱크에 더 많이 들어있으면
        if (canHealAmount <= _tankAmount)
        {
            // 회복할수있는 수치만큼만 빨아먹음.
            for (int i = 0; i < canHealAmount; i++)
            {
                _tankAmount--;
                PlayerManager.Instance.F_HealOxygen(1);
                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.OXYGEN); // UI 업데이트
                UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);

                yield return new WaitForSeconds(1 / canHealAmount);
            }
        }
        // 플레이어가 회복할수있는 수치보다 탱크에 더 적게 들어있으면
        else
        {
            // 전부다 빨아먹음
            int tmpAmount = _tankAmount;
            while (_tankAmount > 0)
            {
                _tankAmount--;
                PlayerManager.Instance.F_HealOxygen(1);
                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.OXYGEN); // UI 업데이트
                UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);

                yield return new WaitForSeconds(1 / tmpAmount);
            }
        }

        _canClickButton = true;
    }
    #endregion

    #region 상호작용 함수
    public override void F_Interaction()
    {
        // 정제기 / 인벤토리 UI 활성화
        UIManager.Instance.OnInventoryUI();         // 인벤토리 UI 활성화

        UIManager.Instance.F_OnTankUI(TankType.OXYGEN, _onEnergy, _onFilter, true);
        UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);
        UIManager.Instance.F_BindingTankUIEvent(_TankButtonEvent);
    }
    #endregion

    #region 저장 / 불러오기 관련
    public override string F_GetData()
    {
        return "NONE";
    }

    public override void F_SetData(string v_data)
    {

    }
    #endregion

    // #TODO:필터기 범위구현하기
    //  전기 + 필터기 범위안에있을떄 _tankAmount 회복하는거 만들기
}
