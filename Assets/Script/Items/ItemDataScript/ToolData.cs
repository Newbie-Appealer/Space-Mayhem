using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolData : UnCountableData
{
    //durability ��ġ�� 0�� ���ϰ� �Ǹ� �ش� �������� �����˴ϴ�.
    public float _durability;
    public ToolData(int v_code, string v_name, string v_description, float v_durability) : base(v_code, v_name, v_description)
    { 
        _durability = v_durability;
    }
}
