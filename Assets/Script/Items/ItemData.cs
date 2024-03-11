using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemData
{
    public int _itemCode;                   // ������ ���� ��ȣ
    public string _itemName;                // ������ �̸�
    public string _itemDescription;         // ������ ����

    public int _foodValue;                  // ���� ȸ�� ��ġ
    public int _toolDurability;             // ���� ������
    public int _installHP;                  // ��ġ�� HP

    public ItemType _itemType;              // ������ Ÿ��
}
