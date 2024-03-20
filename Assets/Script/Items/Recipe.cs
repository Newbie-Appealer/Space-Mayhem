using System;
using UnityEngine;

public enum installation
{
    NONE,      
    TEST
}

public enum CraftCategory
{
    STUFF,
    FOOD,
    TOOL,
    INSTALL
}

[System.Serializable]
public struct Recipe
{
    public int _itemCode;                       // 제작 아이템 번호
    public CraftCategory _itemType;             // 제작 아이템 타입
    public installation _need_Installation;     // 제작 활성화에 필요한 설치물

    public int[] _recipeCode;                   // 제작에 필요한 아이템 번호
    public int[] _recipeCount;                  // 제작에 필요한 아이템 개수

    public bool F_InitRecipe(string[] v_data)
    {
        if (v_data.Length < 4)
            return false;
        _itemCode = int.Parse(v_data[0]);
        _itemType = (CraftCategory)Enum.Parse(typeof(CraftCategory), v_data[2]);
        _need_Installation = (installation)Enum.Parse(typeof(installation), v_data[3]);

        int recipeSize = 0;
        for(int i = 4; i < v_data.Length; i++)
        {
            if (v_data[i].Length > 2)
                recipeSize++;
        }
        _recipeCode  = new int[recipeSize];
        _recipeCount = new int[recipeSize];

        int recipeIndex = 0;
        for (int i = 4; i < v_data.Length; i++)
        {
            if (v_data[i].Length <= 2)
                break;

            string[] tmpRecipe = v_data[i].Split("_");
            _recipeCode[recipeIndex] = int.Parse(tmpRecipe[0]);
            _recipeCount[recipeIndex] = int.Parse(tmpRecipe[1]);
            recipeIndex++;
        }

        return true;
    }
}