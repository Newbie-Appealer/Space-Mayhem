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
    }
   

    public bool F_AddItem(Item v_newitem)
    {
        // 셀수없는 아이템 ( 도구 / 설치류 )
        if (v_newitem is UnCountableItem)
        {
            for(int index = 0; index < _inventorySize; index++) // 인벤토리 탐색
            {
                if(_inventory[index] == null)                   // 빈 슬롯이 있을경우 인벤토리에 아이템 넣음
                {
                    _inventory[index] = v_newitem;
                    return true;                                // 인벤토리 아이템 넣기 성공
                }
            }
        }

        // 셀수있는 아이템 ( 재료 / 소비 )
        else if (v_newitem is CountableItem)
        {
            for(int index = 0; index < _inventorySize; index++)                     // 인벤토리 탐색
            {
                if (_inventory[index] == null)                                      // 비어있는 칸 넘어감
                    continue;

                if (!_inventory[index].F_CheckItemCode(v_newitem))                  // 동일한 아이템이 아니라면 넘어감
                    continue;
                                                                                    // 동일한 아이템인 경우
                if ((_inventory[index] as CountableItem).F_CheckItemStack())        // 현재 아이템 스택을 확인하고, 꽉찬상태가 아니라면 true를 반환함
                {
                    _inventory[index].F_AddStack(v_newitem.itemdata.itemStack);     // 꽉찬상태가 아닐때 현재 스택을 더해줌.
                    return true;                                                    // 아이템 넣기 성공
                }
            }
            for(int index = 0; index < _inventorySize; index++)                     // 인벤토리 탐색
            {
                if(_inventory[index] == null)                                       // 빈 슬롯이 있을경우 인벤토리에 아이템 넣음
                {
                    _inventory[index] = v_newitem;                                  
                    return true;                                                    // 인벤토리 아이템 넣기 성공
                }
            }
        }
        return false;                                                               // 인벤토리 아이템 넣기 실패
    }

    // TODO:인벤토리기능
    // 아이템 추가 ---- 완

    // 아이템 스왑/이동 ----
    // 아이템 삭제 ----
    // 아이템 사용 ----
    // 기능 추가해야함.

    public void F_InventoryUIUpdate()
    {
        Food f = _inventory[0] as Food;
        //인벤토리 배열에 있는 데이터를 UI에 출력하는 함수
    }
}
