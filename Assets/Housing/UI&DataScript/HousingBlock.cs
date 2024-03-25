using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBlock
{
    private Sprite _bloclSprite;
    private string _blockName;
    private string _blockToopTip;

    public List<Tuple < string , int>> _sourceList;       // ��� ��Ƴ��� liSt
    // #TODO 
    // string�κ��� Item���� �ٲٱ�, Item�ȿ� ������ �̸��̶� ��������Ʈ�� �־����

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


    // ui���� housingBlock�� �����ؼ� ǥ�� �ϸ� �� �� 

    // ������Ƽ
    public Sprite BlockSprite { get => _bloclSprite; }
    public string BlockName { get=> _blockName; }

    public string BlockToolTip { get => _blockToopTip; }


}
