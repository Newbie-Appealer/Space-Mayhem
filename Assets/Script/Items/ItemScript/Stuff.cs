using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuff : CountableItem
{
    public StuffData _stuffData;
    public Stuff(StuffData v_data) : base(v_data)
    { _stuffData = v_data; }
}
