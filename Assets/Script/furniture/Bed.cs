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
        UIManager.Instance.F_PlayerMessagePopupTEXT("������ ������");
        SaveManager.Instance.GameDataSave();
    }

    #region ���� / �ҷ�����
    public override string F_GetData()
    {
        return "NONE";
    }

    public override void F_SetData(string v_data)
    {
        
    }
    #endregion
}
