using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBlock
{
    public  HousingBlock(Sprite v_sp, string v_name, string v_tool)
    {
        this._bloclSprite = v_sp;
        this._blockName = v_name;
        this._blockToopTip = v_tool;
    }

    protected Sprite _bloclSprite;
    protected string _blockName;
    protected string _blockToopTip;

    // ui에서 housingBlock에 접근해서 표시 하면 될 듯 

    // 프로퍼티
    public Sprite BlockSprite { get => _bloclSprite; }
    public string BlockName { get=> _blockName; }

    public string BlockToolTip { get => _blockToopTip; }


}
