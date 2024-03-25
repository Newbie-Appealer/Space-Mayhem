using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBlock
{
    private Sprite _bloclSprite;
    private string _blockName;
    private string _blockToopTip;

    public List<Tuple < string , int>> _sourceList;       // 재료 담아놓는 liSt
    // #TODO 
    // string부분을 Item으로 바꾸기, Item안에 아이템 이름이랑 스프라이트가 있어야함

    public  HousingBlock(Sprite v_sp, string v_name, string v_tool)
    {
        this._bloclSprite = v_sp;
        this._blockName = v_name;
        this._blockToopTip = v_tool;

        _sourceList = new List< Tuple< string  , int>>();
    }

    public void F_SetSource(string v_s, int v_i) 
    {
        Tuple<string, int> newItem = new Tuple<string, int>(v_s, v_i);
        _sourceList.Add(newItem);
    }


    // ui에서 housingBlock에 접근해서 표시 하면 될 듯 

    // 프로퍼티
    public Sprite BlockSprite { get => _bloclSprite; }
    public string BlockName { get=> _blockName; }

    public string BlockToolTip { get => _blockToopTip; }


}
