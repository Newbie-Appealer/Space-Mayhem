using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableData : ItemData
{
    public int maxStack => _maxStack;
    protected int _maxStack = 32;
    public CountableData(int v_code, string v_name, string v_description) : base(v_code, v_name, v_description)
    { }
}
