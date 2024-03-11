using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingDataManager : MonoBehaviour
{
    [Header(" ������Ʈ�� 1 �� 1 �����Ǵ� Housing Block ��ũ��Ʈ")]

    public List<List<HousingBlock>> _blockDataList;

    private List<HousingBlock> _Floor;        // �ٴ� 
    private List<HousingBlock> _Celling;      // ����
    private List<HousingBlock> _Pillar;       // ���
    private List<HousingBlock> _Wall;         // ��
    private List<HousingBlock> _Door;         // ��
    private List<HousingBlock> _Stairs;       // ���
    private List<HousingBlock> _Repair;       // ��������
    

    [Header("Floor Sprite")]
    [SerializeField]
    List<Sprite> _floorSprite;
    [SerializeField]
    List<Sprite> _cellingSprite;

    private void Awake()
    {
        _Floor    = new List<HousingBlock>();
        _Celling = new List<HousingBlock>();
        _Pillar   = new List<HousingBlock>();
        _Wall     = new List<HousingBlock>();
        _Door     = new List<HousingBlock>();
        _Stairs = new List<HousingBlock>();
        _Repair   = new List<HousingBlock>();

        _blockDataList = new List< List<HousingBlock> >
        {
            _Floor,
            _Celling
            /*
            ,
            _Pillar,
            _Wall,
            _Door,
            _Stairs,
            _Repair
            */
        };

        F_InitFloorContent();
        F_InitCellingContent();
    }


    public void F_PlayerSelectHousingObj( int v_category , int v_type) 
    {
        // �Ͽ�¡ ī�װ� , ī�װ� �� ���° idx ����
        //GameObject _obj = Instantiate(_housingObj[v_category][v_type] );

        //return _obj;
    }

    public void F_InitFloorContent() 
    {
        _Floor.Add(new HousingBlock(_floorSprite[0], "Ordinary floor", "It's the most basic, plain floor"));

        _Floor[0].F_SetSource("Scrap", 3);
        _Floor[0].F_SetSource("Plastic" , 2);
    }

    public void F_InitCellingContent()
    {
        _Celling.Add(new HousingBlock(_cellingSprite[0], "Ordinary floor", "It's the most basic, plain floor"));

        _Celling[0].F_SetSource("Scrap", 4);
        _Celling[0].F_SetSource("Plastic", 3);
    }
}
