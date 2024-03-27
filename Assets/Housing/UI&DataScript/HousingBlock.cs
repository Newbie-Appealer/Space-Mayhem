using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBlock
{
    /// <summary>
    /// Housing ��ũ��Ʈ �Դϴ�! 
    /// </summary>

    private int _blockHp;
    private Sprite _bloclSprite;
    private string _blockName;
    private string _blockToopTip;

    public List<Tuple<int, int>> _sourceList = new List<Tuple<int, int>>();
    // ��� ��Ƴ��� liSt < ������ ��ȣ, �ʿ��� ���� >

    // ������Ƽ
    public int BlockHp { get => _blockHp; }

    public  HousingBlock( int v_hp, Sprite v_sp, string v_name, string v_tool)
    {
        this._blockHp = v_hp;
        this._bloclSprite = v_sp;
        this._blockName = v_name;
        this._blockToopTip = v_tool;
    }

    public void F_SetSource(int v_itemNum, int v_i) 
    {
        Tuple<int, int> newItem = new Tuple<int, int>(v_itemNum, v_i);
        _sourceList.Add(newItem);
    }

    // ������Ƽ
    public Sprite BlockSprite { get => _bloclSprite; }
    public string BlockName { get=> _blockName; }

    public string BlockToolTip { get => _blockToopTip; }


}
