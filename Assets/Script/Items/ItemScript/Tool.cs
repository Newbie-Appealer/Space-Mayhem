using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : UnCountableItem, UsableItem
{
    public ToolData data
    { get { return _data as ToolData; } }
    public Tool(ToolData v_data) : base(v_data)
    { }
    public void F_UseItem()
    {
        // 플레이어가 아이템을 사용(착용) 하면 PlayerManager 또는 Controller 에 착용관련 함수 호출하고, 
        // 호출한 함수에서 아이템 기능을 활성화시켜주면 될거같습니다.
    }
}
