using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    NONE,       // �ƹ��͵� �ƴ�
    STUFF,      // ��� ������ 
    CONSUM,     // �Һ� ������
    TOOL,       // ���� ������
    INSTALL,    // ��ġ ������
}

[System.Serializable]
public class ItemData
{
    protected int _itemCode;
    protected string _itemName;
    protected string _itemDescription;


}
