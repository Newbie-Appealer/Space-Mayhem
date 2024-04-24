using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public enum WaterTankLevel
{
    SIMPLE,
}

public enum TankType
{
    WATER,
    OXYGEN
}

public class WaterTank : Furniture
{
    Action _TankButtonEvent;

    [Header("=== Water Tank LEVEL===")]
    [SerializeField] private WaterTankLevel _tankLEVEL;

    [Header("=== Water Tank Information ===")]
    [SerializeField] private int _tankAmount;       // ���� ��ġ
    [SerializeField] private int _tankMaxAmount;    // �ִ� ��ġ
    [SerializeField] private int _chargingSpeed;    // ���� �ӵ� ( second )
    private bool _canClickButton;       
    private float gaugeAmount => (float)_tankAmount / (float)_tankMaxAmount;
    private string gaugeText => _tankAmount + " / " + _tankMaxAmount;

    protected override void F_InitFurniture()
    {
        _TankButtonEvent = () => F_ClickEvent();

        _tankMaxAmount = ((int)_tankLEVEL + 1) * 100;   // [100,200,300,400,500]
        _tankAmount = 30;                               // �����ؾ��Ұ�.
        _chargingSpeed = 25 / ((int)_tankLEVEL + 1);    // [25 12 8 6 5]

        _canClickButton = true;

        StartCoroutine(C_ProduceWater());
    }

    public override void F_ChangeFilterState(bool v_state)
    {
        _onFilter = v_state;
    }

    public override void F_ChangeEnergyState(bool v_state)
    {
        _onEnergy = v_state;
    }

    IEnumerator C_ProduceWater()
    {
        while (true)
        {
            yield return new WaitForSeconds(_chargingSpeed);    // _chargingSpeed��ŭ ��ٷȴٰ�

            if (_onEnergy && _onFilter)                         // ������ , ���� ���� ������
            {
                if (_tankAmount >= _tankMaxAmount)              // �ִ��ġ�� ���� ���� �ʾ�����
                    continue;

                _tankAmount++;                                  // ��ġ 1 ȸ��

                if (UIManager.Instance.onTank)
                {
                    UIManager.Instance.F_OnTankUI(TankType.WATER, _onEnergy, _onFilter, true);
                    UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);
                }
            }
        }
    }

    public void F_UseFilter()
    {

    }

    #region UI ��ư �̺�Ʈ �Լ�
    public void F_ClickEvent()
    {
        if(_canClickButton)
        {
            _canClickButton = false;
            StartCoroutine(C_HealWater());
            UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);
            F_UseFilter();
        }
    }

    IEnumerator C_HealWater()
    {
        float canHealAmount = 100 - PlayerManager.Instance.F_GetStat(1);
        
        // �÷��̾ ȸ���Ҽ��ִ� ��ġ����  ��ũ�� �� ���� ���������
        if(canHealAmount <= _tankAmount)
        {
            // ȸ���Ҽ��ִ� ��ġ��ŭ�� ���Ƹ���.
            for(int i =0; i < canHealAmount; i++)
            {
                _tankAmount--;
                PlayerManager.Instance.F_HealWater(1);
                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.WATER); // UI ������Ʈ
                UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);

                yield return new WaitForSeconds( 1 /  canHealAmount);
            }
        }
        // �÷��̾ ȸ���Ҽ��ִ� ��ġ���� ��ũ�� �� ���� ���������
        else
        {
            // ���δ� ���Ƹ���
            int tmpAmount = _tankAmount;
            while(_tankAmount > 0)
            {
                _tankAmount--;
                PlayerManager.Instance.F_HealWater(1);
                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.WATER); // UI ������Ʈ
                UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);

                yield return new WaitForSeconds(1 / tmpAmount);
            }
        }

        _canClickButton = true;
    }

    #endregion

    #region ��ȣ�ۿ� �Լ�
    public override void F_Interaction()
    {
        // ������ / �κ��丮 UI Ȱ��ȭ
        UIManager.Instance.OnInventoryUI();         // �κ��丮 UI Ȱ��ȭ

        UIManager.Instance.F_OnTankUI(TankType.WATER, _onEnergy, _onFilter, true);
        UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);
        UIManager.Instance.F_BindingTankUIEvent(_TankButtonEvent);
    }
    #endregion

    #region ���� / �ҷ����� ����
    public override string F_GetData()
    {
        return "NONE";
    }

    public override void F_SetData(string v_data)
    {

    }
    #endregion
}
