using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnCountableItem : Item
{
    public UnCountableData _unCountableData;
    public UnCountableItem(UnCountableData v_data) : base(v_data)
    {
        _unCountableData = v_data;
    }

    //TODO:아이템스택 관련 함수는 여기서 ( unCountable )
}
