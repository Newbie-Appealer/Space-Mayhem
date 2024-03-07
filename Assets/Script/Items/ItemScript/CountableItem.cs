using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableItem : Item
{
    public CountableData _countableData;
    public CountableItem(CountableData v_data) : base(v_data)
    {
        _countableData = v_data;
    }

    //TODO:아이템스택 관련 함수는 여기서 ( Countable )
}
