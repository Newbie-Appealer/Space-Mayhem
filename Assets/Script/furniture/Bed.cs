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
        //TODO:침대 상호작용시 연출 있으면 좋을듯!

        Debug.Log("save data");
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
