using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CountableData : ItemData
{
    public int _maxStack;
    public CountableData(int v_code, string v_name, string v_description) : base(v_code, v_name, v_description)
    {
        _maxStack = 32;
    }
}
