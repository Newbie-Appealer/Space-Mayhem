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
        // �÷��̾� �Ŵ��� -> �÷��̾� ��� ��ġ ȸ�� ���
        // foodData�� _value ������ ���.
    }
}
