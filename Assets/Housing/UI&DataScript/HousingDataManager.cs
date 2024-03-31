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
    [Header(" ������Ʈ�� 1 �� 1 �����Ǵ� Housing Block ��ũ��Ʈ")]

    public List<List<HousingBlock>> _blockDataList;

    private List<HousingBlock> _Floor;        // �ٴ� 
    private List<HousingBlock> _Celling;      // ����
    private List<HousingBlock> _Wall;         // ��
    private List<HousingBlock> _Door;         // ��
    private List<HousingBlock> _Window;       // â��
    private List<HousingBlock> _Ladder;       // ���
    private List<HousingBlock> _Repair;       // ��������

    [Header("Block Sprite")]
    [SerializeField] List<Sprite> _floorSprite;     //�ٴ�
    [SerializeField] List<Sprite> _cellingSprite;   //õ��
    [SerializeField] List<Sprite> _wallSprite;      //��
    [SerializeField] List<Sprite> _doorSprite;      //��
    [SerializeField] List<Sprite> _windowSprite;    //â��
    [SerializeField] List<Sprite> _ladderSprite;    //��ٸ�
    [SerializeField] List<Sprite> _reapairSprite;   //��������

    [Header("������ index")]
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

        F_InitFloor();           // �ٴ� �ʱ�ȭ
        F_InitCelling();         // õ�� �ʱ�ȭ
        F_InitWall();            // �� �ʱ�ȭ
        F_InitDoor();
        F_IntWindow();
        F_InitLadder();
        F_InitRepair();
    }

    // Floor �ٴ� �ʱ�ȭ
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

    // Celling õ�� �ʱ�ȭ
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

    // Wall �� �ʱ�ȭ
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

    // Door �� �ʱ�ȭ
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
