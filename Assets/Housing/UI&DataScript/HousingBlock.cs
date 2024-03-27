using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBlock
{
    /// <summary>
    /// Housing 스크립트 입니다! 
    /// </summary>

    private Sprite _bloclSprite;
    private string _blockName;
    private string _blockToopTip;

    public List<Tuple<int, int>> _sourceList = new List<Tuple<int, int>>();
    // 재료 담아놓는 liSt < 아이템 번호, 필요한 갯수 >

    public  HousingBlock(Sprite v_sp, string v_name, string v_tool)
    {
        this._bloclSprite = v_sp;
        this._blockName = v_name;
        this._blockToopTip = v_tool;
    }

    public void F_SetSource(int v_itemNum, int v_i) 
    {
        Tuple<int, int> newItem = new Tuple<int, int>(v_itemNum, v_i);
        _sourceList.Add(newItem);
    }


    // ui에서 housingBlock에 접근해서 표시 하면 될 듯 

    // 프로퍼티
    public Sprite BlockSprite { get => _bloclSprite; }
    public string BlockName { get=> _blockName; }

    public string BlockToolTip { get => _blockToopTip; }


}
