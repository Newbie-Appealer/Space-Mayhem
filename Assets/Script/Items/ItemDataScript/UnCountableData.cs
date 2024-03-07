using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnCountableData : ItemData
{
    protected int _maxStack = 1;
    public UnCountableData(int v_code, string v_name, string v_description) : base(v_code, v_name, v_description)
    {
        
    }
}
