using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;       // regex ��� 
using UnityEngine;

// Ŀ���� ����ü 
[System.Serializable]
public struct Connector
{
    public string name;
    private List< Tuple<ConnectorType , Vector3 >> _connectorTypeList;         // Ŀ���͵��� Ÿ�� ,  ��ġ 

    public List<Tuple<ConnectorType, Vector3>> connectorList => _connectorTypeList;   

    public void F_SetConector(List<Tuple<ConnectorType, Vector3>> v_typeList)
    {
        this._connectorTypeList = v_typeList;
    }
}

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

    private void Awake()
    {
        F_InitHousingBLockList();           // ����Ʈ �ʱ�ȭ 
        F_InitHousingBlockData();           // building block ������ �ʱ�ȭ
    }

    public void F_InitConnectorInfo()
    {
        Connector _floor = new Connector();
        Connector _celling = new Connector();
        Connector _wall = new Connector();
        Connector _rotatedWall = new Connector();

        // 1. floor ������ ���� 
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

        // 2. celling ������ ���� 
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

        // 3. wall ������ ����
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

        // 4. rotated wall ������ ����
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

        // 4. Connector�� �� ä��� 
        _floor.F_SetConector(_connType1);
        _celling.F_SetConector(_connType2);
        _wall.F_SetConector(_connType3);
        _rotatedWall.F_SetConector(_connTyp4);

        _floor.name = "floor";
        _celling.name = "celling";
        _wall.name = "basic wall";
        _rotatedWall.name = "ratated wall";

        // 0. destory ��ũ��Ʈ�� connectorArr�� ���
        BuildMaster.Instance.housingRepairDestroy.F_SetConnArr(_floor , _celling , _wall , _rotatedWall);
    }

    // Data List �ʱ�ȭ 
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

    // Data �ҷ����� 
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
