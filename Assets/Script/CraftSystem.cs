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
}
public class CraftSystem : MonoBehaviour
{
    [SerializeField] private List<Recipe> _recipes;
    // ���� �ý��� ������ ��¼�� ?
}
