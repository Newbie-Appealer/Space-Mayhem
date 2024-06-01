using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;       // regex 사용 
using UnityEngine;

// 커넥터 구조체 
[System.Serializable]
public struct Connector
{
    public string name;
    private List< Tuple<ConnectorType , Vector3 >> _connectorTypeList;         // 커넥터들의 타입 ,  위치 

    public List<Tuple<ConnectorType, Vector3>> connectorList => _connectorTypeList;   

    public void F_SetConector(List<Tuple<ConnectorType, Vector3>> v_typeList)
    {
        this._connectorTypeList = v_typeList;
    }
}

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

    private void Awake()
    {
        F_InitHousingBLockList();           // 리스트 초기화 
        F_InitHousingBlockData();           // building block 데이터 초기화
    }

    public void F_InitConnectorInfo()
    {
        Connector _floor = new Connector();
        Connector _celling = new Connector();
        Connector _wall = new Connector();
        Connector _rotatedWall = new Connector();

        // 1. floor 데이터 지정 
        List<Tuple<ConnectorType, Vector3>> _connType1 = new List<Tuple<ConnectorType, Vector3>>
        {
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3(-2.5f , 2.5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3(0, 2.5f , 2.5f)),
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3(2.5f , 2.5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3(0, 2.5f , -2.5f)),
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3(-2.5f , -2.5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3(0, -2.5f , 2.5f) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3(2.5f , -2.5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3(0, -2.5f , -2.5f) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.FloorConnector, new Vector3(-5 , 0, 0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.FloorConnector, new Vector3(0,0, 5) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.FloorConnector, new Vector3(5 ,0,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.FloorConnector, new Vector3(0,0, -5) )
        };

        // 2. celling 데이터 지정 
        List<Tuple<ConnectorType, Vector3>> _connType2 = new List<Tuple<ConnectorType, Vector3>>
         {
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3(-2.5f , 2.5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3(0, 2.5f , 2.5f)),
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3(2.5f , 2.5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3(0, 2.5f , -2.5f)),
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3(-2.5f , -2.5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3(0, -2.5f , 2.5f) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3(2.5f , -2.5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3(0, -2.5f , -2.5f) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3(-5 , 0, 0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3(0,0, 5) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3(5 ,0,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3(0,0, -5) )
         };

        // 3. wall 데이터 지정
        List<Tuple<ConnectorType, Vector3>> _connType3 = new List<Tuple<ConnectorType, Vector3>>()
        {
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3( 0, 5f , 0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3( 5f, 0 , 0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3( 0 , -5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.BasicWallConnector, new Vector3( -5f, 0, 0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3( 0f ,2.5f, -2.5f) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3( 0f, 2.5f, 2.5f) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3( 0 , -2.5f ,2.5f) ),    
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3( 0 , -2.5f, -2.5f) ),
         };

        // 4. rotated wall 데이터 지정
        List<Tuple<ConnectorType, Vector3>> _connTyp4 = new List<Tuple<ConnectorType, Vector3>>()
        {
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3( 0, 5f , 0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3( 0, 0 , 5f)  ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3( 0 , -5f ,0) ), 
             new Tuple<ConnectorType, Vector3>( ConnectorType.RotatedWallConnector, new Vector3( 0 , 0, -5f) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3( 2.5f , 2.5f, 0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3(-2.5f , 2.5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3( -2.5f , -2.5f ,0) ),
             new Tuple<ConnectorType, Vector3>( ConnectorType.CellingConnector, new Vector3( 2.5f , -2.5f, 0) ),
        };

        // 4. Connector에 값 채우기 
        _floor.F_SetConector(_connType1);
        _celling.F_SetConector(_connType2);
        _wall.F_SetConector(_connType3);
        _rotatedWall.F_SetConector(_connTyp4);

        _floor.name = "floor";
        _celling.name = "celling";
        _wall.name = "basic wall";
        _rotatedWall.name = "ratated wall";

        // 0. destory 스크립트의 connectorArr에 담기
        BuildMaster.Instance.housingRepairDestroy.F_SetConnArr(_floor , _celling , _wall , _rotatedWall);
    }

    // Data List 초기화 
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

    // Data 불러오기 
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
