using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : CountableItem
{
    public FoodData _foodData;
    public Food(CountableData v_data) : base(v_data)
    { }
}
