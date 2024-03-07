using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingDataManager : MonoBehaviour
{
    [Header(" 오브젝트와 1 ㄷ 1 대응되는 Housing Block 스크립트")]

    public List<List<HousingBlock>> _blockDataList;

    private List<HousingBlock> _FloorSc;        // 바닥 
    private List<HousingBlock> _RootSc;         // 지붕
    private List<HousingBlock> _PillarSc;       // 기둥
    private List<HousingBlock> _WallSc;         // 벽
    private List<HousingBlock> _DoorSc;         // 문
    private List<HousingBlock> _StairsSc;       // 계단
    private List<HousingBlock> _RepairSc;       // 수리도구
    

    [Header("Floor Sprite")]
    [SerializeField]
    List<Sprite> _floorSprite;

    private void Awake()
    {
        _FloorSc    = new List<HousingBlock>();
        _RootSc     = new List<HousingBlock>();
        _PillarSc   = new List<HousingBlock>();
        _WallSc     = new List<HousingBlock>();
        _DoorSc     = new List<HousingBlock>();
        _StairsSc   = new List<HousingBlock>();
        _RepairSc   = new List<HousingBlock>();

        _blockDataList = new List< List<HousingBlock> >
        {
            _FloorSc
            /*
            ,
            _RootSc,
            _PillarSc,
            _WallSc,
            _DoorSc,
            _StairsSc,
            _RepairSc
            */

        };

        F_InitFloorContent();
    }


    public void F_PlayerSelectHousingObj( int v_category , int v_type) 
    {
        // 하우징 카테고리 , 카테고리 안 몇번째 idx 인지
        //GameObject _obj = Instantiate(_housingObj[v_category][v_type] );

        //return _obj;
    }

    public void F_InitFloorContent() 
    {
        _FloorSc.Add(new HousingBlock(_floorSprite[0], "Ordinary floor", "It's the most basic, plain floor"));
        _FloorSc.Add(new HousingBlock(_floorSprite[1], "Spectacular floor", " It's a more colorful floor than a normal floor"));
        _FloorSc.Add(new HousingBlock(_floorSprite[2], "Shining floor", "It's a floor with a light source"));
    }

}
