using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolData : UnCountableData , UsableItem
{
    //durability 수치가 0이 이하가 되면 해당 아이템은 삭제됩니다.
    public float _durability;
    public ToolData(int v_code, string v_name, string v_description, float v_durability) : base(v_code, v_name, v_description)
    { 
        _durability = v_durability;
    }

    public void F_UseItem()
    {
        // 플레이어가 아이템을 사용(착용) 하면 PlayerManager 또는 Controller 에 착용관련 함수 호출하고, 
        // 호출한 함수에서 아이템 기능을 활성화시켜주면 될거같습니다.
    }
}
