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
        //TODO:ħ�� ��ȣ�ۿ�� ���� ������ ������!

        Debug.Log("save data");
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
