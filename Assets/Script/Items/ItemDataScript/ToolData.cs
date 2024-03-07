using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolData : UnCountableData
{
    public float durability => _durability;
    protected float _durability;
    public ToolData(int v_code, string v_name, string v_description, float v_durability) : base(v_code, v_name, v_description)
    { 
        _durability = v_durability;
    }
}
