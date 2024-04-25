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
        _tankAmount = 0;                                // 저장해야할것. ( 초기값 0 으로 변경하기 )
        _chargingSpeed = 20 / ((int)_tankLEVEL + 1);    // [20 10 6 5 4]

        _tankType = TankType.WATER;

        _canClickButton = true;

        StartCoroutine(C_ProduceWater());
    }
    
    IEnumerator C_ProduceWater()
    {
        while (true)
        {
            yield return new WaitForSeconds(_chargingSpeed);    // _chargingSpeed만큼 기다렸다가

            if (_onEnergy && _onFilter)                         // 에너지 , 필터 전부 있으면
            {
                if (_tankAmount >= _tankMaxAmount)              // 최대수치를 아직 넘지 않았으면
                    continue;

                _tankAmount++;                                  // 수치 1 회복
            }
        }
    }

    #region UI 버튼 이벤트 함수
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
        
        // 플레이어가 회복할수있는 수치보다  탱크에 더 많이 들어있으면
        if(canHealAmount <= _tankAmount)
        {
            // 회복할수있는 수치만큼만 빨아먹음.
            for(int i =0; i < canHealAmount; i++)
            {
                _tankAmount--;
                useFilterHP++;
                PlayerManager.Instance.F_HealWater(1);

                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.WATER);  // 플레이어 상태 수치 UI 업데이트
                UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);   // Tank UI 게이지 업데이트

                yield return new WaitForSeconds( 1 /  canHealAmount);

                if (!UIManager.Instance.onTank)
                    break;
            }
        }
        // 플레이어가 회복할수있는 수치보다 탱크에 더 적게 들어있으면
        else
        {
            // 전부다 빨아먹음
            int tmpAmount = _tankAmount;
            while(_tankAmount > 0)
            {
                _tankAmount--;
                useFilterHP++;
                PlayerManager.Instance.F_HealWater(1);

                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.WATER);  // 플레이어 상태 수치 UI 업데이트
                UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);   // Tank UI 게이지 업데이트

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
         필터가 없어도 탱크 아이템 사용이 가능하지만, 탱크의 회복이 안됨
         필터를 설치하면 회복이 되지만, 탱크 사용시 필터의 HP가 깍임
         필터의 HP가 깍이면 필터기가 부숴지고, 회복을 위해 새로 설치해야함.
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
