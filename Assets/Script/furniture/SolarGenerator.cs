using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarGenerator : Furniture
{
    protected override void F_InitFurniture()
    {

    }

    public override string F_GetData()
    {
        string jsonData = "NONE";
        return jsonData;
    }

    public override void F_SetData(string v_data)
    {
        Debug.Log("(¹Ì±¸Çö) data : " + v_data);
    }
}
