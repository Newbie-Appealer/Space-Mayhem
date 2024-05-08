using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class OxygenTank : Tanks
{
    protected override void F_InitTankData()
    {
        _tankButtonEvent = () => F_ClickEvent();

        _tankMaxAmount = 100;           // 100 ����
        _tankAmount = 0;                // �����ؾ��Ұ�. ( �ʱⰪ 0 ���� �����ϱ� )
        _chargingSpeed = 12;            // 1 ���� �ҿ�ð�

        _tankType = TankType.OXYGEN;

        _canClickButton = true;

        StartCoroutine(C_ProduceOxygen());
    }

    IEnumerator C_ProduceOxygen()
    {
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
    }

    #region UI ��ư �̺�Ʈ �Լ�
    private void F_ClickEvent()
    {
        if(_canClickButton)
        {
            _canClickButton = false;
            StartCoroutine(C_HealOxygen());
            SoundManager.Instance.F_PlaySFX(SFXClip.HEAL);
        }
    }

    IEnumerator C_HealOxygen()
    {
        int useFilterHP = 0;
        float canHealAmount = 100 - PlayerManager.Instance.F_GetStat(0);

        // �÷��̾ ȸ���Ҽ��ִ� ��ġ����  ��ũ�� �� ���� ���������
        if (canHealAmount <= _tankAmount)
        {
            // ȸ���Ҽ��ִ� ��ġ��ŭ�� ���Ƹ���.
            for (int i = 0; i < canHealAmount; i++)
            {
                _tankAmount--;
                useFilterHP++;
                PlayerManager.Instance.F_HealOxygen(1);

                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.OXYGEN);  // �÷��̾� ���� ��ġ UI ������Ʈ
                UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);   // Tank UI ������ ������Ʈ

                yield return new WaitForSeconds(1 / canHealAmount);

                if(!UIManager.Instance.onTank)
                    break;
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
                useFilterHP++;
                PlayerManager.Instance.F_HealOxygen(1);

                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.OXYGEN);  // �÷��̾� ���� ��ġ UI ������Ʈ
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

        foreach (Collider collider in colliders)
        {
            retFilter = collider.GetComponent<Filter>();

            if (retFilter != null)
                return retFilter;
        }
        return null;
    }

    #endregion
}
