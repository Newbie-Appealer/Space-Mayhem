using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Food : CountableItem, UsableItem
{
    public FoodData _foodData;
    public Food(FoodData v_data) : base(v_data)
    { _foodData = v_data; }

    public void F_UseItem()
    {
        // �÷��̾� �Ŵ��� -> �÷��̾� ��� ��ġ ȸ�� ���
        // foodData�� _value ������ ���.

    }
}
