using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class HousingDataManager : MonoBehaviour
{
    // csv 파싱 정규식 
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    [Header("Housing Block List ")]
    public List<List<HousingBlock>> _blockDataList;
    private List<HousingBlock> _Floor;        // 0. 바닥 
    private List<HousingBlock> _Celling;      // 1. 지붕
    private List<HousingBlock> _Wall;         // 2. 벽
    private List<HousingBlock> _Door;         // 3. 문
    private List<HousingBlock> _Window;       // 4. 창문
    private List<HousingBlock> _Repair;       // 5. 수리도구

    [Header("Block Sprite")]
    [SerializeField] List<Sprite> _blockSprite;     // Housing Block의 block num으로 접근 

    private void Awake()
    {
        F_InitHousingBLockList();           // 리스트 초기화 
        F_InitHousingBlockData();           // building block 데이터 초기화
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
        TextAsset textAsset = Resources.Load("BuildingBlockDataEn") as TextAsset;        // csv 파일을 텍스트 에셋타입으로 불러오기 
        string[] lines = Regex.Split( textAsset.text , LINE_SPLIT_RE);                   // 줄 단위로 자르기 ( 정규식을 기준으로 )
        string[] header = Regex.Split(lines[0] , SPLIT_RE);                              // 맨 처음줄, 단어로 자르기 ( 정규식을 기준으로 )

        for (int i = 1; i < lines.Length; i++) 
        {
            string[] values = Regex.Split(lines[i] , SPLIT_RE );                        // 한 줄씩 , 단어로 자르기

            // 1. 블럭 생성, 초기화
            HousingBlock _newBlock = new HousingBlock();
            _newBlock.F_InitBlock( values );

            // 2. 블럭의 타입에 따라 list에 추가
            _blockDataList[ _newBlock.BlockTypeNum ].Add(_newBlock);

        }
    }

   
}
