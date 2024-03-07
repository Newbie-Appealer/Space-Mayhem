using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodData : CountableData
{
    public float _value;
    public FoodData(int v_code, string v_name, string v_description,float v_value) : base(v_code, v_name, v_description)
    {
        _value = v_value;
    }
}
