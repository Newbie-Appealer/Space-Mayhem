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
    private int _blockTypeNum;                  // 블럭 type num
    private int _blockDetailNum;                // 블럭 detail num
    private ConnectorGroup _blockConnectorGroup;           // 몇번째 connector group을 사용하는지
    private Vector3 _blockRotation;             // 'r' input시 얼마나 회전 할 것인지 
    private int _blockHp;                       // hp
    private int _blockMaxHp;                    // max hp
    private Sprite _blockSprite;                // ui상 사용할 이미지
    private string _blockName;                  // ui상 사용할 block 이름
    private string _blockToopTip;               // ui상 사용할 block 설명 

    // 재료 담아놓는 liSt < 아이템 번호, 필요한 갯수 >
    public List<Tuple<int, int>> _sourceList = new List<Tuple<int, int>>();

    // 프로퍼티
    public int blockTypeNum { get => _blockTypeNum; }
    public int blockDetailNum { get => _blockDetailNum; }
    public ConnectorGroup blockConnectorGroup { get => _blockConnectorGroup; }
    public Vector3 blockRotation { get => _blockRotation; }
    public int blockHp { get => _blockHp; }
    public int blockMaxHp { get => _blockMaxHp; }
    public Sprite blockSprite { get => _blockSprite; }
    public string lockName { get=> _blockName; }
    public string blockToolTip { get => _blockToopTip; }

    // block 초기화 
    public void F_InitBlock( string[] v_data , int v_idx ) 
    {
        // [0] block type num ( string -> int )
        // [1] block detial num
        // [2] connectorgroup
        // [3] blockRotation
        // [4] block hp
        // [5] block max hp
        // [6] block name
        // [7] block tool tip
        // [8] item num [9] item count
        // [10] item num [11] item count
        // [12] item num [13] item count 

        this._blockTypeNum          = (int)Enum.Parse(typeof(SelectedBuildType), v_data[0]);
        this._blockDetailNum        = int.Parse(v_data[1]);
        this._blockConnectorGroup   = (ConnectorGroup)Enum.Parse(typeof(ConnectorGroup), v_data[2]);
        string[] _rotatParts        = v_data[3].Split('_');
        this._blockRotation         = new Vector3( float.Parse(_rotatParts[0]) , float.Parse(_rotatParts[1]) , float.Parse(_rotatParts[2]) );
        this._blockHp               = int.Parse(v_data[4]);
        this._blockMaxHp            = int.Parse(v_data[5]);
        this._blockName             = v_data[6];
        this._blockToopTip          = v_data[7];

        // idx에 따른 sprite 초기화 ( blocksprite 리스트는 블럭 순서대로 )
        _blockSprite = BuildMaster.Instance._blockSprite[v_idx];                        

        for (int i = 8; i < v_data.Length; i+=2)     
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



}
