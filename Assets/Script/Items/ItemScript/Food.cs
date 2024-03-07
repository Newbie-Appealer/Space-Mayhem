using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : CountableItem, UsableItem
{
    public FoodData _foodData;
    public Food(CountableData v_data) : base(v_data)
    { }

    public void F_UseItem()
    {
        // 플레이어 매니저 -> 플레이어 허기 수치 회복 기능
        // foodData의 _value 데이터 사용.
    }
}
