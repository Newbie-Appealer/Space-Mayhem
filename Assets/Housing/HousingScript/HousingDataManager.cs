using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingDataManager : MonoBehaviour
{
    [Header(" ������Ʈ�� 1 �� 1 �����Ǵ� Housing Block ��ũ��Ʈ")]

    public List<List<HousingBlock>> _blockDataList;

    private List<HousingBlock> _FloorSc;        // �ٴ� 
    private List<HousingBlock> _RootSc;         // ����
    private List<HousingBlock> _PillarSc;       // ���
    private List<HousingBlock> _WallSc;         // ��
    private List<HousingBlock> _DoorSc;         // ��
    private List<HousingBlock> _StairsSc;       // ���
    private List<HousingBlock> _RepairSc;       // ��������
    

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
        // �Ͽ�¡ ī�װ� , ī�װ� �� ���° idx ����
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
