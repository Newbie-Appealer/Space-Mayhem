using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HousingBlock
{
    /// <summary>
    /// Housing ��ũ��Ʈ �Դϴ�! 
    /// </summary>

    // �ʵ�
    private int _blockCode;
    private int _blockTypeNum;
    private int _blockHp;
    private int _blockMaxHp;
    private Sprite _blockSprite;
    private string _blockName;
    private string _blockToopTip;

    // ��� ��Ƴ��� liSt < ������ ��ȣ, �ʿ��� ���� >
    public List<Tuple<int, int>> _sourceList = new List<Tuple<int, int>>();

    // ������Ƽ
    public int BlockTypeNum { get => _blockTypeNum; }
    public int BlockHp { get => _blockHp; }
    public int BlockMaxHp { get => _blockMaxHp; }
    public Sprite BlockSprite { get => _blockSprite; }
    public string BlockName { get=> _blockName; }
    public string BlockToolTip { get => _blockToopTip; }

    // block �ʱ�ȭ 
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

        F_SetBlockSprite( int.Parse(v_data[0]) );       // block num�� ���� sprite �ʱ�ȭ

        for (int i = 6; i < v_data.Length; i+=2)     
        {
            // 1. ��ᰡ ���� �� , ����
            if (int.Parse(v_data[i]) == -1)
                return;

            // 2. ��ᰡ ���� �� , sourse List �� �ֱ� > item num / item count
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
        // #TODO housingDataManger�� ������ͼ� �ű��� _blockSprite
        if (v_idx != -1)
            _blockSprite = BuildMaster.Instance._blockSprite[ v_idx ];
        // 2. -1�̸� ����,�ı� ����
        else
            _blockSprite = ResourceManager.Instance.F_GetInventorySprite(23);
    }


}
