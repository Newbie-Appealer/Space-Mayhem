using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HousingBlock
{
    /// <summary>
    /// Housing 스크립트 입니다! 
    /// </summary>

    // 필드
    private int _blockCode;
    private int _blockTypeNum;
    private int _blockHp;
    private int _blockMaxHp;
    private Sprite _blockSprite;
    private string _blockName;
    private string _blockToopTip;

    // 재료 담아놓는 liSt < 아이템 번호, 필요한 갯수 >
    public List<Tuple<int, int>> _sourceList = new List<Tuple<int, int>>();

    // 프로퍼티
    public int BlockTypeNum { get => _blockTypeNum; }
    public int BlockHp { get => _blockHp; }
    public int BlockMaxHp { get => _blockMaxHp; }
    public Sprite BlockSprite { get => _blockSprite; }
    public string BlockName { get=> _blockName; }
    public string BlockToolTip { get => _blockToopTip; }

    // block 초기화 
    public void F_InitBlock(string[] v_data) 
    {
        // [0] block num
        // [1] block type num
        // [2] block hp
        // [3] block name
        // [4] block tool tip
        // [5] item num [6] item count
        // [7] item num [8] item count
        // [9] item num [10] item count 
        this._blockCode     = int.Parse(v_data[0]);
        this._blockTypeNum  = int.Parse(v_data[1]);
        this._blockHp       = int.Parse(v_data[2]);
        this._blockMaxHp    = int.Parse(v_data[3]);
        this._blockName     = v_data[4];
        this._blockToopTip  = v_data[5];

        F_SetBlockSprite( int.Parse(v_data[0]) );       // block num에 따른 sprite 초기화

        for (int i = 6; i < v_data.Length; i+=2)     
        {
            // 1. 재료가 없을 때 , 중지
            if (int.Parse(v_data[i]) == -1)
                return;

            // 2. 재료가 있을 때 , sourse List 에 넣기 > item num / item count
            F_SetSource(int.Parse(v_data[i]), int.Parse(v_data[i+1]) ) ;
        }

    }

    public  void F_SetSource(int v_itemNum, int v_i)
    {
        Tuple<int, int> newItem = new Tuple<int, int>(v_itemNum, v_i);
        _sourceList.Add(newItem);
    }

    private void F_SetBlockSprite( int v_idx) 
    {
        // #TODO housingDataManger을 가지고와서 거기의 _blockSprite
        if (v_idx != -1)
            _blockSprite = BuildMaster.Instance._blockSprite[ v_idx ];
        // 2. -1이면 수리,파괴 도구
        else
            _blockSprite = ResourceManager.Instance.F_GetInventorySprite(23);
    }


}
