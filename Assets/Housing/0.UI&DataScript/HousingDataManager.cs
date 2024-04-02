using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class HousingDataManager : MonoBehaviour
{
    // csv �Ľ� ���Խ� 
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    [Header("Housing Block List ")]
    public List<List<HousingBlock>> _blockDataList;
    private List<HousingBlock> _Floor;        // 0. �ٴ� 
    private List<HousingBlock> _Celling;      // 1. ����
    private List<HousingBlock> _Wall;         // 2. ��
    private List<HousingBlock> _Door;         // 3. ��
    private List<HousingBlock> _Window;       // 4. â��
    private List<HousingBlock> _Repair;       // 5. ��������

    [Header("Block Sprite")]
    [SerializeField] List<Sprite> _blockSprite;     // Housing Block�� block num���� ���� 

    private void Awake()
    {
        F_InitHousingBLockList();           // ����Ʈ �ʱ�ȭ 
        F_InitHousingBlockData();           // building block ������ �ʱ�ȭ
    }

    private void F_InitHousingBLockList() 
    {
        _Floor = new List<HousingBlock>();
        _Celling = new List<HousingBlock>();
        _Wall = new List<HousingBlock>();
        _Door = new List<HousingBlock>();
        _Window = new List<HousingBlock>();
        _Repair = new List<HousingBlock>();

        _blockDataList = new List<List<HousingBlock>>
        {
            _Floor,
            _Celling,
            _Wall,
            _Door,
            _Window,
            _Repair
        };
    }

    private void F_InitHousingBlockData() 
    {
        TextAsset textAsset = Resources.Load("BuildingBlockDataEn") as TextAsset;        // csv ������ �ؽ�Ʈ ����Ÿ������ �ҷ����� 
        string[] lines = Regex.Split( textAsset.text , LINE_SPLIT_RE);                   // �� ������ �ڸ��� ( ���Խ��� �������� )
        string[] header = Regex.Split(lines[0] , SPLIT_RE);                              // �� ó����, �ܾ�� �ڸ��� ( ���Խ��� �������� )

        for (int i = 1; i < lines.Length; i++) 
        {
            string[] values = Regex.Split(lines[i] , SPLIT_RE );                        // �� �پ� , �ܾ�� �ڸ���

            // 1. �� ����, �ʱ�ȭ
            HousingBlock _newBlock = new HousingBlock();
            _newBlock.F_InitBlock( values );

            // 2. ���� Ÿ�Կ� ���� list�� �߰�
            _blockDataList[ _newBlock.BlockTypeNum ].Add(_newBlock);

        }
    }

   
}
