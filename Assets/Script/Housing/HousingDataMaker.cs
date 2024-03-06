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
            new HousingData( 10, "����� �ٴ�"),
            new HousingData( 20, "ȭ���� �ٴ�"),
            new HousingData( 30, "�ܴ��� �ٴ�")
        };
    }

    internal void F_NowSelectDataIdx(int v_idx)
    {
        Debug.Log("���� ���� �� �����ʹ�" + _floorDataList[v_idx].Name); 
    }
}
