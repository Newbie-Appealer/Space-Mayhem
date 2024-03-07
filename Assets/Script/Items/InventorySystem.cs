using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int _inventorySize = 28;
    [SerializeField] private Item[] _inventory;

    private void Awake()
    {
        // 0~7  -> 퀵 슬롯
        // 8~27 -> 인벤토리 슬롯
        _inventory = new Item[_inventorySize];
        for(int i = 0; i < _inventorySize; i++)
        {
            Debug.Log(_inventory[i]);
        }
    }
   

    public bool F_AddItem(Item v_newitem)
    {
        // 셀수없는 아이템 ( 도구 / 설치류 )
        if (v_newitem is UnCountableItem)
        {
            // 1. 인벤토리 선형탐색
            // 2. 빈 슬롯 탐색
            // 3. 있으면 슬롯에 아이템 등록 후 true 반환
            // 4. 없으면 슬롯에 아이템 등록하지않고 false 반환
        }

        // 셀수있는 아이템 ( 재료 / 소비 )
        else if (v_newitem is CountableItem)
        {
            // 1. 인벤토리 선형탐색
            // 2. 동일한 아이템이 있는지 확인
            //   - 있으면 아이템 스택++
            //   - 최대 스택을 넘어가면 다른 슬롯 탐색
            // 3. 동일한 아이템이 없으면 빈 슬롯 탐색
            // 4. 빈슬롯이 있으면 아이템 등록 후 true 리턴
            // 5. 빈슬롯이 없으면 아이템 등록하지않고 false 리턴
        }
        return false;
    }

    public void F_InventoryUIUpdate()
    {
        Food f = _inventory[0] as Food;
    }
}
