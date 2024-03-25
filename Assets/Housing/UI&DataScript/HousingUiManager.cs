using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HousingUiManager : MonoBehaviour
{
    public static HousingUiManager instance;
    [SerializeField]
    HousingDataManager _housingObject;

    [Header("Building canvas")]
    [SerializeField]
    GameObject _craftCanvas;

    [Space]
    [Header("Housing Block Info")]
    [SerializeField]
    Image _infoBlockSprite;
    [SerializeField]
    TextMeshProUGUI _infoBlockName;     // 아이템 이름
    [SerializeField]
    TextMeshProUGUI _infoBlockToolTip;  // 아이템 툴팁 (설명)
    [SerializeField]
    List< Image > _itemNeedImage;             // 재료 이미지
    [SerializeField]
    List< TextMeshProUGUI > _itemNeedText;    // 재료 이름 텍스트
    [SerializeField]
    List<TextMeshProUGUI> _itemnNeedCount;  // 재료 수량 텍스트

    [Space]
    [Header("Slot")]
    [SerializeField]
    GameObject _slotUiPrefab;           // 슬롯 ui 프리팹
    [SerializeField]
    GameObject[] _craftSlotList;        // 카테고리 슬롯
    [SerializeField]
    Sprite[] _craftSlotsSprite;         // 카테고리 슬롯에 넣을 스프라이트

    [Space]
    [Header("Slot Detail Panel")]
    [SerializeField]
    GameObject[] _slotDetailPanel;      // 슬롯 detail 패널
    [SerializeField]
    GameObject[] _detailPanelContent;   // detail 패널 밑 스크롤 뷰 밑의 content 

    [Space]
    [Header("now Open Panel & Detail Panel")]
    [SerializeField]
    private int _nowOpenPanel;          // 현재 열린 panel 검사
    [SerializeField]
    private int _nowOpenDetailSlot;     // 현재 선택 된 panel 안 slot idx 저장

    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        F_SetMouseMove(false);

        F_InitCraftSlotIdx();       // 카테고리 슬롯 초기 설정
        F_ClontSlotInDetail();      // detail 창 안의 Slot 생성

        F_UpdateHousingInfo( 0 , 0 );     // 초기 Info 창 설정
    }
    // housing UI On Off 
    private void Update()
    {
        if (Input.GetMouseButton(1))        // 우클릭을 하고 있는 동안
            F_OnOffCraftCanvas(true);       // canvas 보이게
        else if (Input.GetMouseButtonUp(1)) // 우클릭 떼면
        {
            F_OnOffCraftCanvas(false);      // cavas 안보이게

            // #TODO
            MyBuildManager.instance.F_GetbuildType( _nowOpenPanel , _nowOpenDetailSlot % 10 );
        }
    }
    // On Off builgind panel
    private void F_OnOffCraftCanvas(bool v_check)
    {
        _craftCanvas.SetActive(v_check);
        F_SetMouseMove(v_check);
    }

    // 플레이어 커서 모드
    private void F_SetMouseMove(bool v_mode)
    {
        if ( v_mode == false )
        {
            Cursor.lockState = CursorLockMode.Locked;        // 커서를 '화면 정중앙'에 고정시킴
            Cursor.visible = false;                          // 커서 안 보이게
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;             // 커서 원래대로
            Cursor.visible = true;                              // 커서 보이게
        }
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

    // detail Panel 에 Itemslot 추가하기
    private void F_ClontSlotInDetail() 
    {
        // Housing object 관리하는 스크립트에서, 저장되어있는 obj 만큼 slot 추가 
        // 나중에 전체 초기화 할 때 _housingObject._housingObjList.Count; 만큼 for문 더 돌리면 됨 

        for (int i = 0; i < _housingObject._blockDataList.Count; i++) 
        {
            for (int j = 0; j < _housingObject._blockDataList[i].Count; j++)
            {
                // Slot 생성
                GameObject _cloneSlot = Instantiate(_slotUiPrefab);
                // detail 패널 밑의 scroll view의 content 밑에 추가
                _cloneSlot.transform.parent = _detailPanelContent[i].transform;

                CraftSlot _cslot = _cloneSlot.GetComponent<CraftSlot>();
                _cslot.idx = ((i + 1) * 10) + j;           // detail 밑 slot의 index는 순서대로 10,11,12,13....
                _cslot.F_SetImageIcon(_housingObject._blockDataList[i][j].BlockSprite);

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
        // idx 10 넘는거는 detail Panel 안의 slot 임 -> 슬롯 인덱스 저장만
        if (v_idx >= 10) 
        {
            _nowOpenDetailSlot = v_idx;

            F_UpdateHousingInfo(_nowOpenPanel, v_idx % 10);                // panel안의 Slot은 idx가 NN 이니까 나머지 검사해서 upate하기
        }
        // 10 이하 idx는 바깥쪽 slot
        else 
        {
            F_CheckDeialPanel();    // 다른 패널 검사

            _nowOpenPanel = v_idx;
            _slotDetailPanel[v_idx].gameObject.SetActive(v_flag);
        }
    }

    // 켜져있는 패널 검산
    private void F_CheckDeialPanel() 
    {
        // 현재 켜져 있는 detail Panel 이 있는지 검사, 있으면 끄기
        if (_nowOpenPanel >= 0)  // 열려 있는 패널이 있을 때
            _slotDetailPanel[_nowOpenPanel].gameObject.SetActive(false);    // 열려 있는 패널 끄기

    }

    // Ui의 Info 업데이트
    private void F_UpdateHousingInfo(int panel ,  int v_idx) 
    {
        if (_housingObject._blockDataList[panel][v_idx] == null)
            Debug.Log( panel + " / " + v_idx);

        HousingBlock _myblock = _housingObject._blockDataList[panel][v_idx];

        _infoBlockSprite.sprite  = _myblock.BlockSprite;
        _infoBlockName.text      = _myblock.BlockName;
        _infoBlockToolTip.text   = _myblock.BlockToolTip;

        // _blockDataList의 idx의 저장되어 있는 HousingBlock스크립트의 _sourceList에 접근해서
        // 재료 가져오기
        for (int i = 0; i < _myblock._sourceList.Count; i++) 
        {
            _itemNeedImage[i].gameObject.SetActive(true);
            _itemNeedText[i].text    = _myblock._sourceList[i].Item1;      // 재료 이름에 접근
            _itemnNeedCount[i].text  = "0" + " / " + _myblock._sourceList[i].Item2.ToString();
            // #TODO
            // 아이템 갯수 "0" 부분을 inventory의 내가 가지고 있는 아이템 수량으로 설정해야함
        }
    }

}
