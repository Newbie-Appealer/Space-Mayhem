using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HousingDataMaker : MonoBehaviour
{
    public static HousingDataMaker instance;

    [SerializeField]
    List<HousingData> _floorDataList;

    private void Awake()
    {
        instance = this;

        InitFloor();
    }

    public void InitFloor() 
    {
        _floorDataList = new List<HousingData>
        {
            new HousingData( 10, "평범한 바닥"),
            new HousingData( 20, "화려한 바닥"),
            new HousingData( 30, "단단한 바닥")
        };
    }

    internal void F_NowSelectDataIdx(int v_idx)
    {
        Debug.Log("현재 선택 된 데이터는" + _floorDataList[v_idx].Name); 
    }
}
