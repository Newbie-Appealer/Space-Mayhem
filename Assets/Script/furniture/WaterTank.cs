using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class WaterTank : Tanks
{

    [Header("=== Water Tank LEVEL===")]
    [SerializeField] private WaterTankLevel _tankLEVEL;

    protected override void F_InitTankData()
    {
        _tankButtonEvent = () => F_ClickEvent();

        _tankMaxAmount = ((int)_tankLEVEL + 1) * 100;   // [100,200,300,400,500]
        _tankAmount = 0;                                // �����ؾ��Ұ�. ( �ʱⰪ 0 ���� �����ϱ� )
        _chargingSpeed = 20 / ((int)_tankLEVEL + 1);    // [20 10 6 5 4]

        _tankType = TankType.WATER;

        _canClickButton = true;

        StartCoroutine(C_ProduceWater());
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
            }
        }
    }

    #region UI ��ư �̺�Ʈ �Լ�
    private void F_ClickEvent()
    {
        if(_canClickButton)
        {
            _canClickButton = false;
            StartCoroutine(C_HealWater());
        }
    }

    IEnumerator C_HealWater()
    {
        int useFilterHP = 0;
        float canHealAmount = 100 - PlayerManager.Instance.F_GetStat(1);
        
        // �÷��̾ ȸ���Ҽ��ִ� ��ġ����  ��ũ�� �� ���� ���������
        if(canHealAmount <= _tankAmount)
        {
            // ȸ���Ҽ��ִ� ��ġ��ŭ�� ���Ƹ���.
            for(int i =0; i < canHealAmount; i++)
            {
                _tankAmount--;
                useFilterHP++;
                PlayerManager.Instance.F_HealWater(1);

                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.WATER);  // �÷��̾� ���� ��ġ UI ������Ʈ
                UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);   // Tank UI ������ ������Ʈ

                yield return new WaitForSeconds( 1 /  canHealAmount);

                if (!UIManager.Instance.onTank)
                    break;
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
                useFilterHP++;
                PlayerManager.Instance.F_HealWater(1);

                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.WATER);  // �÷��̾� ���� ��ġ UI ������Ʈ
                UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);   // Tank UI ������ ������Ʈ

                yield return new WaitForSeconds(1 / tmpAmount);

                if (!UIManager.Instance.onTank)
                    break;
            }
        }
        Filter connectFilter = F_SearchFilter();
        if (connectFilter != null)
            connectFilter.F_UseHP(useFilterHP);
        _canClickButton = true;
    }

    private Filter F_SearchFilter()
    {
        /*
         ���Ͱ� ��� ��ũ ������ ����� ����������, ��ũ�� ȸ���� �ȵ�
         ���͸� ��ġ�ϸ� ȸ���� ������, ��ũ ���� ������ HP�� ����
         ������ HP�� ���̸� ���ͱⰡ �ν�����, ȸ���� ���� ���� ��ġ�ؾ���.
         */

        Filter retFilter = null;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3.5f, _filterLayer);
        
        foreach(Collider collider in colliders)
        {
            retFilter = collider.GetComponent<Filter>();

            if(retFilter != null)
                return retFilter;
        }
        return null;
    }
    #endregion
}
