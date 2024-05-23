using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemData
{
    public int _itemCode;                   // 아이템 고유 번호
    public string _itemName;                // 아이템 이름
    public string _itemDescription;         // 아이템 설명

    public float _foodValue;                  // 음식 회복 수치
    public float _toolDurability;             // 도구 내구도
    public float _installHP;                  // 설치물 HP

    public int _toolCode;                   // 도구 고유 번호
    public int _installCode;                // 설치류 고유 번호

    public PlayerState _playerState;        // 아이템 사용시 플레이어의 상태변환
    public ItemType _itemType;              // 아이템 타입\
    public HealType _healType;              // 음식 회복 타입

    public void F_initData(string[] v_datas)
    {
        try
        {
            _itemCode = int.Parse(v_datas[0]);                                          // 0 item Code
            _itemName = v_datas[1];                                                     // 1 item name
            _itemDescription = v_datas[2];                                              // 2 item Description
            _foodValue = float.Parse(v_datas[3]);                                       // 3 food value
            _toolDurability = float.Parse(v_datas[4]);                                  // 4 tool durability
            _installHP = float.Parse(v_datas[5]);                                       // 5 intsll hp
            _toolCode = int.Parse(v_datas[6]);                                          // 6 tool code
            _installCode = int.Parse(v_datas[7]);                                       // 7 intsll code
            _playerState = (PlayerState)Enum.Parse(typeof(PlayerState), v_datas[8]);    // 8 player state
            _itemType = (ItemType)Enum.Parse(typeof(ItemType), v_datas[9]);             // 9 item type
            _healType = (HealType)Enum.Parse(typeof(HealType), v_datas[10]);             // 10 heal type
        }
        catch(Exception e) 
        {
            Debug.LogException(e);
            Debug.LogError("Error ItemData Initialize");
        }

    }
}
