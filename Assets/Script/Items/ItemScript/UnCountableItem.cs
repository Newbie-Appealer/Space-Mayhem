using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnCountableItem : Item
{
    protected UnCountableData _unCountableData
    { get; private set; }
    public UnCountableItem(UnCountableData v_data) : base(v_data)
    {
        _unCountableData = v_data;
    }
}
