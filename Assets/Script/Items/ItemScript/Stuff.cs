using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuff : CountableItem
{
    public StuffData data
    { get { return _itemdata as StuffData; } }

    public Stuff(StuffData v_data) : base(v_data) { }
}
