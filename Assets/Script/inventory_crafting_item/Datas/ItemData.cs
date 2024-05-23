using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemData
{
    public int _itemCode;                   // ������ ���� ��ȣ
    public string _itemName;                // ������ �̸�
    public string _itemDescription;         // ������ ����

    public float _foodValue;                  // ���� ȸ�� ��ġ
    public float _toolDurability;             // ���� ������
    public float _installHP;                  // ��ġ�� HP

    public int _toolCode;                   // ���� ���� ��ȣ
    public int _installCode;                // ��ġ�� ���� ��ȣ

    public PlayerState _playerState;        // ������ ���� �÷��̾��� ���º�ȯ
    public ItemType _itemType;              // ������ Ÿ��\
    public HealType _healType;              // ���� ȸ�� Ÿ��

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
