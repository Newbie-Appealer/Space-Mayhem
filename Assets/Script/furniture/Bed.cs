using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Furniture
{
    protected override void F_InitFurniture()
    {
        
    }

    public override void F_Interaction()
    {
        UIManager.Instance.F_PlayerMessagePopupTEXT("데이터 저장중");
        SaveManager.Instance.GameDataSave();
    }

    #region 저장 / 불러오기
    public override string F_GetData()
    {
        return "NONE";
    }

    public override void F_SetData(string v_data)
    {
        
    }
    #endregion
}
