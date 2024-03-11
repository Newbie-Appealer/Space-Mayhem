using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Recipe
{
    public int _itemCode;       // 제작되는 아이템 번호
    public ItemType ItemType;   // 제작되는 아이템의 타입

    public int[] _neededCode;   // 조합에 필요한 아이템 번호
    public int[] _needCount;    // 조합에 필요한 아이템의 개수
}


public class CraftSystem : MonoBehaviour
{
    [SerializeField] private List<Recipe> _recipes;

    // 제작에 필요한 아이템이 뭔지 저장해야함
    private InventorySystem _inventory;
    private void Start()
    {
        _inventory = ItemManager.Instance.inventorySystem;
        // 게임 시작시 제작 가능한 아이템들 데이터를 가지고 UI 만들기
        // 제작 가능 여부는 인벤토리 열때마다 업데이트
    }
}
