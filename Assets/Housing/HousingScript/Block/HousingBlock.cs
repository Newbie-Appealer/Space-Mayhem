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

    // ui���� housingBlock�� �����ؼ� ǥ�� �ϸ� �� �� 

    // ������Ƽ
    public Sprite BlockSprite { get => _bloclSprite; }
    public string BlockName { get=> _blockName; }

    public string BlockToolTip { get => _blockToopTip; }


}
