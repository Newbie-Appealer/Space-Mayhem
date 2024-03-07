using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Install : UnCountableItem, UsableItem
{
    public InstallData _installData;
    public Install(InstallData v_data) : base(v_data)
    {
        _installData = v_data;
    }

    public void F_UseItem()
    {
        // ItemManager에서 설치류 아이템의 프리팹을 가져오고,
        // 설치 기능을 다른함수에서 구현하여 호출하면될것같습니다.
    }
}
