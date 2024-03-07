using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolData : UnCountableData
{
    //durability 수치가 0이 이하가 되면 해당 아이템은 삭제됩니다.
    public float _durability;
    public ToolData(int v_code, string v_name, string v_description, float v_durability) : base(v_code, v_name, v_description)
    { 
        _durability = v_durability;
    }
}
