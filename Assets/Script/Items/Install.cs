using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Install : Item, UsableItem
{
    public float hp => _hp;
    [SerializeField] private float _hp;
    public Install(ItemData data) : base(data)
    {
        _maxStack = 1;
        _hp = data._installHP;
        _itemType = data._itemType;
    }

    public void F_UseItem()
    {
        Debug.Log(" 도구의 아이템 사용은 아직 구현되지 않았다! ");

        // 설치모드를 관리하는 매니저를 만들어서 함수 호출 해야할듯.
        // 설치물의 번호를 매개변수로 보내야할듯.
    }
}
