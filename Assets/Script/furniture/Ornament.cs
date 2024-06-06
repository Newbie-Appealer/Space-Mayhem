using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ornament : Furniture
{
    protected override void F_InitFurniture()
    {
        // 초기화할거없음! 그냥 장식품임!
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
