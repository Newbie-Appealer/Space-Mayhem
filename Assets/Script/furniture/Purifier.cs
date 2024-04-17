using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purifier : Furniture
{
    protected override void F_InitFurniture()
    {

    }

    public override void F_Interaction()
    {
        // 정제기 / 인벤토리 UI 활성화
        UIManager.Instance.OnInventoryUI();
        UIManager.Instance.F_OnPurifierUI(true);
    }

    public override string F_GetData()
    {
        string jsonData = "NONE";
        return jsonData;
    }

    public override void F_SetData(string v_data)
    {

    }

}
