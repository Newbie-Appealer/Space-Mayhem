using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HousingUiManager : Singleton<HousingUiManager>
{
    protected override void InitManager()
    {
    }

    // 하우징 데이터
    [SerializeField]
    HousingDataManager _housingObject;

    // 하우징 canvas
    [Header("Building canvas")]
    [SerializeField]
    GameObject _craftCanvas;

    // 하우징 블럭 info
    [Space]
    [Header("Housing Block Info")]
    [SerializeField]
    Image _infoBlockSprite;
    [SerializeField]
    TextMeshProUGUI _infoBlockName;             // 아이템 이름
    [SerializeField]
    TextMeshProUGUI _infoBlockToolTip;          // 아이템 툴팁 (설명)
    [SerializeField]
    List< Image > _itemNeedImage;               // 재료 이미지   // 첫번째 plastic, 두번째 scrap은 고정 , 세번째만 index에 맞게 바꾸기
    [SerializeField]
    List< TextMeshProUGUI > _itemNeedText;      // 재료 이름 텍스트
    [SerializeField]
    List<TextMeshProUGUI> _itemnNeedCount;      // 재료 수량 텍스트
    [SerializeField]
    List<Sprite> _otherSprite;                  // 0. 유리 2. 전선         

    // 카테고리 썸네일
    [Space]
    [Header("Slot")]
    [SerializeField]
    GameObject _slotUiPrefab;           // 슬롯 ui 프리팹
    [SerializeField]
    GameObject[] _craftSlotList;        // 카테고리 슬롯
    [SerializeField]
    Sprite[] _craftSlotsSprite;         // 카테고리 썸네일 슬롯에 넣을 스프라이트

    // 카테고리 detail 
    [Space]
    [Header("Slot Detail Panel")]
    [SerializeField]
    GameObject[] _slotDetailPanel;      // 슬롯 detail 패널
    [SerializeField]
    GameObject[] _detailPanelContent;   // detail 패널 밑 스크롤 뷰 밑의 content 

    // 현재 열린 블럭 type, detail 패널 index
    [Space]
    [Header("now Open Panel & Detail Panel")]
    [SerializeField]
    private int _nowOpenPanel;          // 현재 열린 panel 검사
    [SerializeField]
    private int _nowOpenDetailSlot;     // 현재 선택 된 panel 안 slot idx 저장

    private void Start()
    {
        F_InitCraftSlotIdx();       // 카테고리 슬롯 초기 설정
        F_ClontSlotInDetail();      // detail 창 안의 Slot 생성

        F_UpdateHousingInfo( 0 , 0 );     // 초기 Info 창 설정
    }
    // housing UI On Off 
    private void Update()
    {
        // 0. Player의 State가 building일때만 동작
        if (PlayerManager.Instance.playerState == PlayerState.BUILDING)
        {
            if (Input.GetMouseButton(1))        // 우클릭을 하고 있는 동안
                F_WhenHousingUiOn();
            else if (Input.GetMouseButtonUp(1)) // 우클릭 떼면
            {
                // 0. UI가 off 됐을 때
                F_WhenHousingUiOff();
            }
        }
    }

    private void F_WhenHousingUiOn() 
    {
        // 0. inventory 초기화
        /*
        ItemManager.Instance.F_UpdateItemCounter();
        */

        // 1. canvas , cursor On
        F_OnOffCraftCanvas(true);  
    }
    private void F_WhenHousingUiOff() 
    {
        // 1. canvas , cursor OFF
        F_OnOffCraftCanvas(false);

        // BuildMaanger에 index 옮기기
        MyBuildManager.Instance.F_GetbuildType(_nowOpenPanel, _nowOpenDetailSlot % 10);

        // 2. 현재 선택 된 block에 대한 재료를 inventory에서 검사
        // 재료가 있으면 BuildingManager 실행 , 아니면 설치 x 
        //if (F_CheckMyBlockSource())
        //{
            // BuildMaanger에 index 옮기기
        //}

    }

    private bool F_CheckMyBlockSource() 
    {
        if(_nowOpenPanel < 0 || _nowOpenDetailSlot < 0) 
            return false;

        HousingBlock _myblock = _housingObject._blockDataList[_nowOpenPanel][_nowOpenDetailSlot % 10 ];
        int itemidx , needCnt ;
        int _CanBuild = 0;
 
        for (int i = 0; i < _myblock._sourceList.Count; i++) 
        {
            itemidx = _myblock._sourceList[i].Item1;        // 아이템 num
            needCnt = _myblock._sourceList[i].Item2;        // 아이템 필요갯수

            // 현재 아이템 갯수보다 많으면 > build 가능
            if (ItemManager.Instance.itemCounter[itemidx] >= needCnt) 
            {
                _CanBuild++;
            } 
        }

        if (_CanBuild == _myblock._sourceList.Count)
        {
            // #TODO 인벤토리에서의 동작 추가

            return true;
        }
        return false;
    }

    // On Off builgind panel
    private void F_OnOffCraftCanvas(bool v_check)
    {
        // 1. panel OnOff
        _craftCanvas.SetActive(v_check);

        // 2. 커서
        GameManager.Instance.F_SetCursor(v_check);
    }

    // 카테고리 슬롯 초기 설정
    private void F_InitCraftSlotIdx() 
    {
        for (int i = 0; i < _craftSlotList.Length; i++) 
        {
            CraftSlot _cf = _craftSlotList[i].GetComponent<CraftSlot>();
            _cf.idx = i;
            _cf.F_SetImageIcon(_craftSlotsSprite[i]);
        }
    }

    // detail Panel 에 Itemslot 추가
    private void F_ClontSlotInDetail() 
    {
        // #TODO
        // HousingDataManager에서 갯수 추가하고 확인필요
        for (int i = 0; i < _housingObject._blockDataList.Count; i++) 
        {
            for (int j = 0; j < _housingObject._blockDataList[i].Count; j++)
            {
                // 0. Slot 생성
                GameObject _cloneSlot = Instantiate(_slotUiPrefab);
                // 1. detail 패널 밑의 scroll view의 content 밑에 추가
                _cloneSlot.transform.parent = _detailPanelContent[i].transform;

                CraftSlot _cslot = _cloneSlot.GetComponent<CraftSlot>();
                _cslot.idx = ((i + 1) * 10) + j;                                            // detail 밑 slot의 index는 순서대로 10,11,12,13....
                _cslot.F_SetImageIcon(_housingObject._blockDataList[i][j].BlockSprite);     // 블럭 데이터의 스프라이트 가져와서 설정

            }
        }   
    }

    // 오브젝트가 켜질 때
    private void OnEnable()
    {
        _nowOpenPanel = -1;     // 현재 열려있는 panel idx를 -1로
    }

    // Panel 끄고 켜기
    public void F_OnOffDtailPanel(int v_idx , bool v_flag) 
    {
        // idx가 10 이상 detail Panel 안의 slot 임 -> 슬롯 인덱스 저장만
        if (v_idx >= 10) 
        {
            _nowOpenDetailSlot = v_idx;

            F_UpdateHousingInfo(_nowOpenPanel, v_idx % 10);                // panel안의 Slot은 idx가 NN 이니까 나머지 검사해서 upate하기
        }
        // 10 미만 idx는 바깥쪽 slot
        else 
        {
            F_CheckDeialPanel();    // 다른 패널 검사

            _nowOpenPanel = v_idx;
            _slotDetailPanel[v_idx].gameObject.SetActive(v_flag);
        }
    }

    // 켜져있는 패널 검사
    private void F_CheckDeialPanel() 
    {
        // 현재 켜져 있는 detail Panel 이 있는지 검사, 있으면 끄기
        if (_nowOpenPanel >= 0)  // 열려 있는 패널이 있을 때
            _slotDetailPanel[_nowOpenPanel].gameObject.SetActive(false);    // 열려 있는 패널 끄기

    }

    // Ui의 Info 업데이트
    private void F_UpdateHousingInfo(int v_type ,  int v_idx) 
    {
        if (_housingObject._blockDataList[v_type][v_idx] == null)
            return;

        HousingBlock _myblock = _housingObject._blockDataList[v_type][v_idx];        // 몇 번째 타입의 idx 에 해당하는 블럭

        _infoBlockSprite.sprite  = _myblock.BlockSprite;                             // ui상 오른 위쪽 , sprite 설정
        _infoBlockName.text      = _myblock.BlockName;                               // ui상 오른 위쪽, name 설정
        _infoBlockToolTip.text   = _myblock.BlockToolTip;                            // ui상 오른 위쪽, tooltip 설정

        _itemNeedImage[ _itemNeedImage.Count -1 ].gameObject.SetActive(false);      // 3번째 재료 초기화
        _itemNeedText[_itemNeedImage.Count - 1].text = "";                          // 3번째 이름 초기화
        _itemnNeedCount[_itemNeedImage.Count - 1].text = "";                        // 3번째 count 초기화

        // _blockDataList의 idx의 저장되어 있는 HousingBlock스크립트의 _sourceList에 접근해서
        // 재료 가져오기
        for (int i = 0; i < _myblock._sourceList.Count; i++) 
        {
            bool _flag = false;
            // 0. 재료 slot ON
            _itemNeedImage[i].gameObject.SetActive(true);

            // 1. 재료 사진 접근 ( 유리,구리가 아니면 아이콘 설정 필요 x )
            if (_myblock._sourceList[i].Item1 == 9)     // 아이템 번호가 '유리'
            { 
                _itemNeedImage[i].sprite = _otherSprite[0];
                _flag = true;
            
            }
            else if (_myblock._sourceList[i].Item1 == 16)   // 아이템 번호가 '구리'
            { 
                _itemNeedImage[i].sprite = _otherSprite[1];
                _flag = true;
            
            }

            // 2. 재료 이름 접근 > 아이템 인덱스로 아이템 이름에 접근 (ItemManager)
            if(_flag)
                _itemNeedText[i].text = ItemManager.Instance.ItemDatas[_myblock._sourceList[i].Item1]._itemName;

            // 3. 인벤토리의 아이템 갯수 / 필요한 아이템의 갯수 에 접근
            _itemnNeedCount[i].text  = ItemManager.Instance.itemCounter[_myblock._sourceList[i].Item1] + " / " + _myblock._sourceList[i].Item2.ToString();

        }
    }

}
