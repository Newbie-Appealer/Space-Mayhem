using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ornament : Furniture
{
    protected override void F_InitFurniture()
    {
        // �ʱ�ȭ�Ұž���! �׳� ���ǰ��!
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
