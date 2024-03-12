using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum installation
{
    NONE,       // n
    TEST
}

[System.Serializable]
public struct Recipe
{
    public int _itemCode;                       // 제작 아이템 번호
    public ItemType _itemType;                  // 제작 아이템 타입
    public installation _need_Installation;     // 제작 활성화에 필요한 설치물

    public int[] _recipeCode;                   // 제작에 필요한 아이템 번호
    public int[] _recipeCount;                  // 제작에 필요한 아이템 개수
}
public class CraftSystem : MonoBehaviour
{
    [SerializeField] private List<Recipe> _recipes;
    // 제작 시스템 데이터 어쩌지 ?
}
