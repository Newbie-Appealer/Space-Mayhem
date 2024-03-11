using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemData
{
    public int _itemCode;                   // 아이템 고유 번호
    public string _itemName;                // 아이템 이름
    public string _itemDescription;         // 아이템 설명

    public int _foodValue;                  // 음식 회복 수치
    public int _toolDurability;             // 도구 내구도
    public int _installHP;                  // 설치물 HP

    public ItemType _itemType;              // 아이템 타입
}
