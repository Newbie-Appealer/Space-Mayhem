using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    public float foodValue => _foodValue;
    [SerializeField] private float _foodValue;
    public Food(ItemData data) : base(data) 
    {
        _maxStack = 32;
        _foodValue = data._foodValue;
        _itemType = data._itemType;

        _playerState = data._playerState;
    }
    public override void F_UseItem()
    {
        Debug.Log("���� ������ ���");
        //PlayerManager.Instance.F_HealHunger();  // �Ű������� _foodValue �ѱ��
        F_AddStack(-1);
    }
}
