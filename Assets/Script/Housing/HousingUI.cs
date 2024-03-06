using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingUI : MonoBehaviour
{
    public static HousingUI instance;

    [SerializeField]
    GameObject[] _floorSlot;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        F_InitFloor();
    }

    public void F_InitFloor() 
    { 
        for(int i = 0; i < _floorSlot.Length; i++) 
        {
            if (_floorSlot[i].GetComponent<HousingDetailSlot>() == null)
                _floorSlot[i].AddComponent<HousingDetailSlot>();

            _floorSlot[i].GetComponent<HousingDetailSlot>().Idx = i;
        }
    }

    // ���콺 �� �� ����
    public void F_PointerInSlot(int v_idx) 
    {
        // housing �����Ϳ� ����
        HousingDataMaker.instance.F_NowSelectDataIdx(v_idx);
    }

}
