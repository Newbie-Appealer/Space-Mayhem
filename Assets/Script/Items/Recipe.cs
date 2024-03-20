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
    public int _itemCode;                       // ���� ������ ��ȣ
    public CraftCategory _itemType;             // ���� ������ Ÿ��
    public installation _need_Installation;     // ���� Ȱ��ȭ�� �ʿ��� ��ġ��

    public int[] _recipeCode;                   // ���ۿ� �ʿ��� ������ ��ȣ
    public int[] _recipeCount;                  // ���ۿ� �ʿ��� ������ ����

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