using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HousingUiManager : MonoBehaviour
{
    // 하우징 canvas
    [Header("===Building Panel===")]
    [SerializeField]
    public GameObject _buildingBlockSelectUi;     // BLock 선택 ui 
    [SerializeField] 
    public GameObject _buildingProgressUi;        // 빌딩 진행 ui ( houising ui가 꺼질 때 On , housingui가 켜질 때 Off)

    // 하우징 블럭 info
    [Space]
    [Header("===Housing Block Info===")]
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

    // 카테고리 썸네일
    [Space]
    [Header("===Slot===")]
    [SerializeField]
    GameObject _slotUiPrefab;           // 슬롯 ui 프리팹
    [SerializeField]
    GameObject[] _craftSlotList;        // 카테고리 슬롯
    [SerializeField]
    Sprite[] _craftSlotsSprite;         // 카테고리 썸네일 슬롯에 넣을 스프라이트

    // 카테고리 detail 
    [Space]
    [Header("===Slot Detail Panel===")]
    [SerializeField]
    GameObject[] _slotDetailPanel;      // 슬롯 detail 패널
    [SerializeField]
    GameObject[] _detailPanelContent;   // detail 패널 밑 스크롤 뷰 밑의 content 

    // 현재 열린 블럭 type, detail 패널 index
    [Space]
    [Header("===now Open Panel & Detail Panel===")]
    [SerializeField]
    private int _nowOpenPanel;          // 현재 열린 panel 검사
    [SerializeField]
    private int _nowOpenDetailSlot;     // 현재 선택 된 panel 안 slot idx 저장

    [Header("===Repair Text===")]
    [SerializeField] TextMeshProUGUI _repairToolText;

    private void Start()
    {
        F_InitCraftSlotIdx();       // 카테고리 슬롯 초기 설정
        F_ClontSlotInDetail();      // detail 창 안의 Slot 생성

        F_UpdateHousingInfo( 0 , 0 );     // 초기 Info 창 설정
    }

    // 오브젝트가 켜질 때
    private void OnEnable()
    {
        _nowOpenPanel = -1;     // 현재 열려있는 panel idx를 -1로
    }

    public void F_OnOffRepairText(MyBuildingBlock v_block ,bool v_flag)
    {
        // TODO : 하우징 repair 일 떄 text onoff 기능추가 
        _repairToolText.gameObject.SetActive(v_flag);

        if ( v_block != null && v_flag)
            _repairToolText.text = v_block.MyBlockHp.ToString() + " / " + v_block.MyBlockMaxHp.ToString();
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

    // Housing Ui On 
    private void F_WhenHousingUiOn() 
    {
        // 0. inventory 초기화
        ItemManager.Instance.F_UpdateItemCounter();
        
        // 1. canvas , cursor On
        F_OnOffCraftCanvas(true);

        // 2. progress ui 는 off
        _buildingProgressUi.gameObject.SetActive(false);
    }

    // Housing Ui Off
    private void F_WhenHousingUiOff() 
    {
        // 1. canvas , cursor OFF
        F_OnOffCraftCanvas(false);

        // 2. Build Master에 현재 block Data를 저장해놓기 
        if (_nowOpenPanel < 0 || _nowOpenDetailSlot < 0)        // idx가 0 미만이면 return 
            return;

        // 3. BuildMaster에 BlockData 넣어놓기 
        BuildMaster.Instance.
            F_SetBlockData( BuildMaster.Instance.housingDataManager.blockDataList[_nowOpenPanel][_nowOpenDetailSlot]);

        // BuildMaanger에 index 옮기기
        BuildMaster.Instance.myBuildManger.F_GetbuildType(_nowOpenPanel, _nowOpenDetailSlot);

    }

    // MyBuildingBlock 스크립트
    public void F_OnOFfBuildingProgressUi(bool v_flag) 
    {
        _buildingProgressUi.gameObject.SetActive(v_flag);
    }

    // On Off builgind panel
    private void F_OnOffCraftCanvas(bool v_check)
    {
        // 1. panel OnOff
        _buildingBlockSelectUi.SetActive(v_check);

        // 2. 커서
        GameManager.Instance.F_SetCursor(v_check);
    }

    // 카테고리 슬롯 초기 설정
    private void F_InitCraftSlotIdx() 
    {
        for (int i = 0; i < _craftSlotList.Length; i++)
        {
            CraftSlot _cf = _craftSlotList[i].GetComponent<CraftSlot>();
            _cf.typeNum = BuildMaster.Instance.housingDataManager.blockDataList[i][0].blockTypeNum;
            _cf.detialNum = BuildMaster.Instance.housingDataManager.blockDataList[i][0].blockDetailNum;

            // 1. 스프라이트 설정 
            _cf.F_SetImageIcon(_craftSlotsSprite[i]);
        }
    }

    // detail Panel 에 Itemslot 추가
    private void F_ClontSlotInDetail() 
    {
        // 하우징 Manager의
        for (int i = 0; i < BuildMaster.Instance.housingDataManager.blockDataList.Count; i++) 
        {
            for (int j = 0; j < BuildMaster.Instance.housingDataManager.blockDataList[i].Count; j++)
            {
                // 0. Slot 생성
                GameObject _cloneSlot = Instantiate(_slotUiPrefab);
                // 1. detail 패널 밑의 scroll view의 content 밑에 추가
                _cloneSlot.transform.parent = _detailPanelContent[i].transform;

                CraftSlot _cslot = _cloneSlot.GetComponent<CraftSlot>();
                // 0. 인덱스 설정 
                _cslot.typeNum = BuildMaster.Instance.housingDataManager.blockDataList[i][j].blockTypeNum;
                _cslot.detialNum = BuildMaster.Instance.housingDataManager.blockDataList[i][j].blockDetailNum;

                // 1. 이미지 설정 
                _cslot.F_SetImageIcon(BuildMaster.Instance.housingDataManager.blockDataList[i][j].blockSprite);     // 블럭 데이터의 스프라이트 가져와서 설정

            }
        }   
    }


    // Panel 끄고 켜기
    public void F_OnOffDtailPanel(int v_type , int v_detail) 
    {
        // 현재 openDetailSlot이랑, 새로 들어온 v_Type 같으면?
        if ( _nowOpenPanel ==  v_type ) 
            _nowOpenDetailSlot = v_detail;                               // detail 설정 

        // `` 다르면 ?
        else 
        {
            if(_nowOpenPanel >= 0 )
                _slotDetailPanel[_nowOpenPanel].gameObject.SetActive(false);      // 이미 켜져있던 panel 끄기 

            _nowOpenPanel = v_type;                                    // type 설정 
            _slotDetailPanel[v_type].gameObject.SetActive(true);       // detail 패널 켜기
        }

        F_UpdateHousingInfo(v_type, v_detail);                // panel안의 Slot은 idx가 NN 이니까 나머지 검사해서 upate하기
    }


    // Ui의 Info 업데이트
    private void F_UpdateHousingInfo(int v_type ,  int v_idx) 
    {
        if ( BuildMaster.Instance.housingDataManager.blockDataList[v_type][v_idx] == null )
            return;

        HousingBlock _myblock = BuildMaster.Instance.housingDataManager.blockDataList[v_type][v_idx];        // 몇 번째 타입의 idx 에 해당하는 블럭

        _infoBlockSprite.sprite  = _myblock.blockSprite;                             // ui상 오른 위쪽 , sprite 설정
        _infoBlockName.text      = _myblock.lockName;                                // ui상 오른 위쪽, name 설정
        _infoBlockToolTip.text   = _myblock.blockToolTip;                            // ui상 오른 위쪽, tooltip 설정

        // Ui상 오른 아래족, 아이템 source 3번째는 초기화 후 설정
        F_InitUnderLeftUi();

        // Ui상 오른 아래쪽, 아이템 source 설정
        for (int i = 0; i < _myblock._sourceList.Count; i++) 
        {
            // 0. 재료 slot ON
            _itemNeedImage[i].gameObject.SetActive(true);
            
            // 1. 재료 사진 넣기
            _itemNeedImage[i].sprite = ResourceManager.Instance.F_GetInventorySprite(_myblock._sourceList[i].Item1);

            // 2. 재료 이름 접근 > 아이템 인덱스로 아이템 이름에 접근 (ItemManager)
            _itemNeedText[i].text = ItemManager.Instance.ItemDatas[ _myblock._sourceList[i].Item1 ]._itemName;

            // 3. 인벤토리의 아이템 갯수 / 필요한 아이템의 갯수 에 접근
            _itemnNeedCount[i].text  = ItemManager.Instance.itemCounter[_myblock._sourceList[i].Item1] + " / " + _myblock._sourceList[i].Item2.ToString();

        }
    }

    private void F_InitUnderLeftUi()
    {
        for (int i = 0; i < 3; i++)
        {
            _itemNeedImage[i].gameObject.SetActive(false);
            _itemNeedText[i].text = "";
            _itemnNeedCount[i].text = "";
        }
    }

}
