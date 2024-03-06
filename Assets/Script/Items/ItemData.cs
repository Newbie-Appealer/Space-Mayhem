using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    NONE,       // 아무것도 아님
    STUFF,      // 재료 아이템 
    CONSUM,     // 소비 아이템
    TOOL,       // 도구 아이템
    INSTALL,    // 설치 아이템
}

[System.Serializable]
public class ItemData
{
    protected int _itemCode;
    protected string _itemName;
    protected string _itemDescription;


}
