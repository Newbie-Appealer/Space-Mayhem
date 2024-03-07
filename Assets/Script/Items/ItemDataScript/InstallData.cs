using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallData : UnCountableData
{
    public float _hp;
    public InstallData(int v_code, string v_name, string v_description, float v_hp) : base(v_code, v_name, v_description)
    {
        _hp = v_hp;
    }
}
