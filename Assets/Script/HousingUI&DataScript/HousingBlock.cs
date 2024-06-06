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
    private int _blockTypeNum;                  // �� type num
    private int _blockDetailNum;                // �� detail num
    private ConnectorGroup _blockConnectorGroup;           // ���° connector group�� ����ϴ���
    private Vector3 _blockRotation;             // 'r' input�� �󸶳� ȸ�� �� ������ 
    private int _blockHp;                       // hp
    private int _blockMaxHp;                    // max hp
    private Sprite _blockSprite;                // ui�� ����� �̹���
    private string _blockName;                  // ui�� ����� block �̸�
    private string _blockToopTip;               // ui�� ����� block ���� 

    // ��� ��Ƴ��� liSt < ������ ��ȣ, �ʿ��� ���� >
    public List<Tuple<int, int>> _sourceList = new List<Tuple<int, int>>();

    // ������Ƽ
    public int blockTypeNum { get => _blockTypeNum; }
    public int blockDetailNum { get => _blockDetailNum; }
    public ConnectorGroup blockConnectorGroup { get => _blockConnectorGroup; }
    public Vector3 blockRotation { get => _blockRotation; }
    public int blockHp { get => _blockHp; }
    public int blockMaxHp { get => _blockMaxHp; }
    public Sprite blockSprite { get => _blockSprite; }
    public string lockName { get=> _blockName; }
    public string blockToolTip { get => _blockToopTip; }

    // block �ʱ�ȭ 
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

        // idx�� ���� sprite �ʱ�ȭ ( blocksprite ����Ʈ�� �� ������� )
        _blockSprite = BuildMaster.Instance._blockSprite[v_idx];                        

        for (int i = 8; i < v_data.Length; i+=2)     
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



}
