using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OxygenTank : Furniture
{
    Action _TankButtonEvent;

    [Header("=== Oxygen Tank Information ===")]
    [SerializeField] private int _tankAmount;     // ���� ��ġ
    [SerializeField] private int _tankMaxAmount;  // �ִ� ��ġ

    private bool _canClickButton;
    private float gaugeAmount => (float)_tankAmount / (float)_tankMaxAmount;
    private string gaugeText => _tankAmount + " / " + _tankMaxAmount;

    protected override void F_InitFurniture()
    {
        _TankButtonEvent = () => F_ClickEvent();

        _tankMaxAmount = 100;
        _tankAmount = 100;      // �����ؾ��Ұ�.

        _canClickButton = true;
    }

    #region UI ��ư �̺�Ʈ �Լ�
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

        // �÷��̾ ȸ���Ҽ��ִ� ��ġ����  ��ũ�� �� ���� ���������
        if (canHealAmount <= _tankAmount)
        {
            // ȸ���Ҽ��ִ� ��ġ��ŭ�� ���Ƹ���.
            for (int i = 0; i < canHealAmount; i++)
            {
                _tankAmount--;
                PlayerManager.Instance.F_HealOxygen(1);
                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.OXYGEN); // UI ������Ʈ
                UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);

                yield return new WaitForSeconds(1 / canHealAmount);
            }
        }
        // �÷��̾ ȸ���Ҽ��ִ� ��ġ���� ��ũ�� �� ���� ���������
        else
        {
            // ���δ� ���Ƹ���
            int tmpAmount = _tankAmount;
            while (_tankAmount > 0)
            {
                _tankAmount--;
                PlayerManager.Instance.F_HealOxygen(1);
                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.OXYGEN); // UI ������Ʈ
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

        UIManager.Instance.F_OnTankUI(TankType.OXYGEN, _onEnergy, _onFilter, true);
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

    // #TODO:���ͱ� ���������ϱ�
    //  ���� + ���ͱ� �����ȿ������� _tankAmount ȸ���ϴ°� �����
}
