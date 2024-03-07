using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallData : UnCountableData, UsableItem
{
    public float _hp;
    public InstallData(int v_code, string v_name, string v_description, float v_hp) : base(v_code, v_name, v_description)
    {
        _hp = v_hp;
    }

    public void F_UseItem()
    {
        // ItemManager에서 설치류 아이템의 프리팹을 가져오고,
        // 설치 기능을 다른함수에서 구현하여 호출하면될것같습니다.
    }
}
