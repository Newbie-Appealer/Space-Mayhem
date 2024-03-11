using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Recipe
{
    public int _itemCode;       // ���۵Ǵ� ������ ��ȣ
    public ItemType ItemType;   // ���۵Ǵ� �������� Ÿ��

    public int[] _neededCode;   // ���տ� �ʿ��� ������ ��ȣ
    public int[] _needCount;    // ���տ� �ʿ��� �������� ����
}


public class CraftSystem : MonoBehaviour
{
    [SerializeField] private List<Recipe> _recipes;

    // ���ۿ� �ʿ��� �������� ���� �����ؾ���
    private InventorySystem _inventory;
    private void Start()
    {
        _inventory = ItemManager.Instance.inventorySystem;
        // ���� ���۽� ���� ������ �����۵� �����͸� ������ UI �����
        // ���� ���� ���δ� �κ��丮 �������� ������Ʈ
    }
}
