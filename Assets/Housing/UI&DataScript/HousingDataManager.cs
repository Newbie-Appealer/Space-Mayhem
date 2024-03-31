using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType 
{
    FLOOR,
    CELIING,
    WALL,
    DOOR,
    WINDOW,
    LADDER,
    REPAIR
}

public class HousingDataManager : MonoBehaviour
{
    [Header(" 오브젝트와 1 ㄷ 1 대응되는 Housing Block 스크립트")]

    public List<List<HousingBlock>> _blockDataList;

    private List<HousingBlock> _Floor;        // 바닥 
    private List<HousingBlock> _Celling;      // 지붕
    private List<HousingBlock> _Wall;         // 벽
    private List<HousingBlock> _Door;         // 문
    private List<HousingBlock> _Window;       // 창문
    private List<HousingBlock> _Ladder;       // 계단
    private List<HousingBlock> _Repair;       // 수리도구

    [Header("Block Sprite")]
    [SerializeField] List<Sprite> _floorSprite;     //바닥
    [SerializeField] List<Sprite> _cellingSprite;   //천장
    [SerializeField] List<Sprite> _wallSprite;      //벽
    [SerializeField] List<Sprite> _doorSprite;      //문
    [SerializeField] List<Sprite> _windowSprite;    //창문
    [SerializeField] List<Sprite> _ladderSprite;    //사다리
    [SerializeField] List<Sprite> _reapairSprite;   //수리도구

    [Header("아이템 index")]
    [HideInInspector] int _plasticNum = 0;
    [HideInInspector] int _scrapNum = 2;
    [HideInInspector] int _glassNum = 9;
    [HideInInspector] int _circuitNum = 16;

    private void Awake()
    {
        _Floor      = new List<HousingBlock>();
        _Celling    = new List<HousingBlock>();
        _Wall       = new List<HousingBlock>();
        _Door       = new List<HousingBlock>();
        _Window     = new List<HousingBlock>();
        _Ladder     = new List<HousingBlock>();
        _Repair     = new List<HousingBlock>();

        _blockDataList = new List< List<HousingBlock> >
        {
            _Floor,
            _Celling,
            _Wall,
            _Door,
            _Window,
            _Ladder,
            _Repair
        };

        F_InitFloor();           // 바닥 초기화
        F_InitCelling();         // 천장 초기화
        F_InitWall();            // 벽 초기화
        F_InitDoor();
        F_IntWindow();
        F_InitLadder();
        F_InitRepair();
    }

    // Floor 바닥 초기화
    public void F_InitFloor() 
    {
        // 1
        _Floor.Add(new HousingBlock(10, _floorSprite[0], "Ordinary floor" , "It's the most basic, plain floor"));

        _Floor[0].F_SetSource(_scrapNum , 3);
        _Floor[0].F_SetSource(_plasticNum , 2);

        // 2
        _Floor.Add(new HousingBlock(10, _floorSprite[1], "Ordinary floor", "It's the most basic, plain floor"));

        _Floor[1].F_SetSource(_scrapNum, 3);
        _Floor[1].F_SetSource(_plasticNum, 2);

        // 3
        _Floor.Add(new HousingBlock(10, _floorSprite[2], "Ordinary floor", "It's the most basic, plain floor"));

        _Floor[2].F_SetSource(_scrapNum, 3);
        _Floor[2].F_SetSource(_plasticNum, 2);


        // 4
        _Floor.Add(new HousingBlock(10, _floorSprite[3], "Ordinary floor", "It's the most basic, plain floor"));

        _Floor[3].F_SetSource(_scrapNum, 3);
        _Floor[3].F_SetSource(_plasticNum, 2);
    }

    // Celling 천장 초기화
    public void F_InitCelling()
    {
        // 1
        _Celling.Add(new HousingBlock(10, _cellingSprite[0], "Ordinary floor", "It's the most basic, plain floor"));

        _Celling[0].F_SetSource(_scrapNum, 4);
        _Celling[0].F_SetSource( _plasticNum, 3);

        // 2
        _Celling.Add(new HousingBlock(10, _cellingSprite[1], "Ordinary floor", "It's the most basic, plain floor"));

        _Celling[1].F_SetSource(_scrapNum, 4);
        _Celling[1].F_SetSource(_plasticNum, 3);

        // 3
        _Celling.Add(new HousingBlock(10, _cellingSprite[2], "Ordinary floor", "It's the most basic, plain floor"));

        _Celling[2].F_SetSource(_scrapNum, 4);
        _Celling[2].F_SetSource(_plasticNum, 3);

    }

    // Wall 벽 초기화
    public void F_InitWall() 
    {
        // 1
        _Wall.Add(new HousingBlock(10, _wallSprite[0], "Ordinary floor", "It's the most basic, plain floor"));

        _Wall[0].F_SetSource(_plasticNum, 1);
        _Wall[0].F_SetSource(_scrapNum, 1);

        // 2
        _Wall.Add(new HousingBlock(10, _wallSprite[1], "Ordinary floor", "It's the most basic, plain floor"));

        _Wall[1].F_SetSource(_plasticNum, 1);
        _Wall[1].F_SetSource(_scrapNum, 1);

        // 3
        _Wall.Add(new HousingBlock(10, _wallSprite[2], "Ordinary floor", "It's the most basic, plain floor"));

        _Wall[2].F_SetSource(_plasticNum, 1);
        _Wall[2].F_SetSource(_scrapNum, 1);

    }

    // Door 문 초기화
    public void F_InitDoor() 
    {
        // 1
        _Door.Add(new HousingBlock(10, _doorSprite[0], "Ordinary floor", "It's the most basic, plain floor"));

        _Door[0].F_SetSource(_plasticNum, 6);
        _Door[0].F_SetSource(_scrapNum, 6);

    }

    public void F_IntWindow() 
    {
        // 1
       _Window.Add(new HousingBlock(10, _windowSprite[0], "Ordinary floor", "It's the most basic, plain floor"));

        _Window[0].F_SetSource(_plasticNum, 6);
        _Window[0].F_SetSource(_scrapNum, 6);


        // 2
        _Window.Add(new HousingBlock(10, _windowSprite[1], "Ordinary floor", "It's the most basic, plain floor"));

        _Window[1].F_SetSource(_plasticNum, 6);
        _Window[1].F_SetSource(_scrapNum, 6);

    }

    public void F_InitLadder() 
    {
        // 1
        _Ladder.Add(new HousingBlock(10, _ladderSprite[0], "Ordinary floor", "It's the most basic, plain floor"));

        _Ladder[0].F_SetSource(_plasticNum, 6);
        _Ladder[0].F_SetSource(_scrapNum, 6);

        // 1
        _Ladder.Add(new HousingBlock(10, _ladderSprite[1], "Ordinary floor", "It's the most basic, plain floor"));

        _Ladder[1].F_SetSource(_plasticNum, 6);
        _Ladder[1].F_SetSource(_scrapNum, 6);

    }

    public void F_InitRepair()
    {
        // 1
        _Repair.Add(new HousingBlock(10, _reapairSprite[0], "Ordinary floor", "It's the most basic, plain floor"));

        _Repair[0].F_SetSource(_plasticNum, 6);
        _Repair[0].F_SetSource(_scrapNum, 6);

        // 1
        _Repair.Add(new HousingBlock(10, _reapairSprite[1], "Ordinary floor", "It's the most basic, plain floor"));

        _Repair[1].F_SetSource(_plasticNum, 6);
        _Repair[1].F_SetSource(_scrapNum, 6);

    }
}
