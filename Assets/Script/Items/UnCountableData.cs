using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnCountableData : ItemData
{
    public int _maxStack;
    public UnCountableData(int v_code, string v_name, string v_description) : base(v_code, v_name, v_description)
    {
        _maxStack = 1;
    }
}
