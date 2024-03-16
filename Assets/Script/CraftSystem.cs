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
    public int _itemCode;                       // ���� ������ ��ȣ
    public ItemType _itemType;                  // ���� ������ Ÿ��
    public installation _need_Installation;     // ���� Ȱ��ȭ�� �ʿ��� ��ġ��

    public int[] _recipeCode;                   // ���ۿ� �ʿ��� ������ ��ȣ
    public int[] _recipeCount;                  // ���ۿ� �ʿ��� ������ ����

    // _�������ڵ� [0] 
    // _������cnt  [0]
    // �ε��� ��ȣ�� ����
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
