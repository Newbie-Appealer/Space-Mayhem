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

    // _레시피코드 [0] 
    // _레시피cnt  [0]
    // 인덱스 번호로 대응
}

public class CraftSystem : MonoBehaviour
{
    [SerializeField] private List<Recipe> _recipes;

    [Header("Prefabs")]
    [SerializeField] private GameObject _CraftSlot;
    [SerializeField] private GameObject _StuffSlot;

    [Header("Content")]
    [SerializeField] private Transform _stuffs;
    [SerializeField] private Transform _foods;
    [SerializeField] private Transform _tools;
    [SerializeField] private Transform _installs;
}
