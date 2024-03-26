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

    // �Ͽ�¡ ������
    [SerializeField]
    HousingDataManager _housingObject;

    // �Ͽ�¡ canvas
    [Header("Building canvas")]
    [SerializeField]
    GameObject _craftCanvas;

    // �Ͽ�¡ �� info
    [Space]
    [Header("Housing Block Info")]
    [SerializeField]
    Image _infoBlockSprite;
    [SerializeField]
    TextMeshProUGUI _infoBlockName;             // ������ �̸�
    [SerializeField]
    TextMeshProUGUI _infoBlockToolTip;          // ������ ���� (����)
    [SerializeField]
    List< Image > _itemNeedImage;               // ��� �̹���   // ù��° plastic, �ι�° scrap�� ���� , ����°�� index�� �°� �ٲٱ�
    [SerializeField]
    List< TextMeshProUGUI > _itemNeedText;      // ��� �̸� �ؽ�Ʈ
    [SerializeField]
    List<TextMeshProUGUI> _itemnNeedCount;      // ��� ���� �ؽ�Ʈ
    [SerializeField]
    List<Sprite> _otherSprite;                  // 0. ���� 2. ����         

    // ī�װ� �����
    [Space]
    [Header("Slot")]
    [SerializeField]
    GameObject _slotUiPrefab;           // ���� ui ������
    [SerializeField]
    GameObject[] _craftSlotList;        // ī�װ� ����
    [SerializeField]
    Sprite[] _craftSlotsSprite;         // ī�װ� ����� ���Կ� ���� ��������Ʈ

    // ī�װ� detail 
    [Space]
    [Header("Slot Detail Panel")]
    [SerializeField]
    GameObject[] _slotDetailPanel;      // ���� detail �г�
    [SerializeField]
    GameObject[] _detailPanelContent;   // detail �г� �� ��ũ�� �� ���� content 

    // ���� ���� �� type, detail �г� index
    [Space]
    [Header("now Open Panel & Detail Panel")]
    [SerializeField]
    private int _nowOpenPanel;          // ���� ���� panel �˻�
    [SerializeField]
    private int _nowOpenDetailSlot;     // ���� ���� �� panel �� slot idx ����

    private void Start()
    {
        F_InitCraftSlotIdx();       // ī�װ� ���� �ʱ� ����
        F_ClontSlotInDetail();      // detail â ���� Slot ����

        F_UpdateHousingInfo( 0 , 0 );     // �ʱ� Info â ����
    }
    // housing UI On Off 
    private void Update()
    {
        // 0. Player�� State�� building�϶��� ����
        if (PlayerManager.Instance.playerState == PlayerState.BUILDING)
        {
            if (Input.GetMouseButton(1))        // ��Ŭ���� �ϰ� �ִ� ����
                F_WhenHousingUiOn();
            else if (Input.GetMouseButtonUp(1)) // ��Ŭ�� ����
            {
                // 0. UI�� off ���� ��
                F_WhenHousingUiOff();
            }
        }
    }

    private void F_WhenHousingUiOn() 
    {
        // 0. inventory �ʱ�ȭ
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

        // BuildMaanger�� index �ű��
        MyBuildManager.Instance.F_GetbuildType(_nowOpenPanel, _nowOpenDetailSlot % 10);

        // 2. ���� ���� �� block�� ���� ��Ḧ inventory���� �˻�
        // ��ᰡ ������ BuildingManager ���� , �ƴϸ� ��ġ x 
        //if (F_CheckMyBlockSource())
        //{
            // BuildMaanger�� index �ű��
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
            itemidx = _myblock._sourceList[i].Item1;        // ������ num
            needCnt = _myblock._sourceList[i].Item2;        // ������ �ʿ䰹��

            // ���� ������ �������� ������ > build ����
            if (ItemManager.Instance.itemCounter[itemidx] >= needCnt) 
            {
                _CanBuild++;
            } 
        }

        if (_CanBuild == _myblock._sourceList.Count)
        {
            // #TODO �κ��丮������ ���� �߰�

            return true;
        }
        return false;
    }

    // On Off builgind panel
    private void F_OnOffCraftCanvas(bool v_check)
    {
        // 1. panel OnOff
        _craftCanvas.SetActive(v_check);

        // 2. Ŀ��
        GameManager.Instance.F_SetCursor(v_check);
    }

    // ī�װ� ���� �ʱ� ����
    private void F_InitCraftSlotIdx() 
    {
        for (int i = 0; i < _craftSlotList.Length; i++) 
        {
            CraftSlot _cf = _craftSlotList[i].GetComponent<CraftSlot>();
            _cf.idx = i;
            _cf.F_SetImageIcon(_craftSlotsSprite[i]);
        }
    }

    // detail Panel �� Itemslot �߰�
    private void F_ClontSlotInDetail() 
    {
        // #TODO
        // HousingDataManager���� ���� �߰��ϰ� Ȯ���ʿ�
        for (int i = 0; i < _housingObject._blockDataList.Count; i++) 
        {
            for (int j = 0; j < _housingObject._blockDataList[i].Count; j++)
            {
                // 0. Slot ����
                GameObject _cloneSlot = Instantiate(_slotUiPrefab);
                // 1. detail �г� ���� scroll view�� content �ؿ� �߰�
                _cloneSlot.transform.parent = _detailPanelContent[i].transform;

                CraftSlot _cslot = _cloneSlot.GetComponent<CraftSlot>();
                _cslot.idx = ((i + 1) * 10) + j;                                            // detail �� slot�� index�� ������� 10,11,12,13....
                _cslot.F_SetImageIcon(_housingObject._blockDataList[i][j].BlockSprite);     // �� �������� ��������Ʈ �����ͼ� ����

            }
        }   
    }

    // ������Ʈ�� ���� ��
    private void OnEnable()
    {
        _nowOpenPanel = -1;     // ���� �����ִ� panel idx�� -1��
    }

    // Panel ���� �ѱ�
    public void F_OnOffDtailPanel(int v_idx , bool v_flag) 
    {
        // idx�� 10 �̻� detail Panel ���� slot �� -> ���� �ε��� ���常
        if (v_idx >= 10) 
        {
            _nowOpenDetailSlot = v_idx;

            F_UpdateHousingInfo(_nowOpenPanel, v_idx % 10);                // panel���� Slot�� idx�� NN �̴ϱ� ������ �˻��ؼ� upate�ϱ�
        }
        // 10 �̸� idx�� �ٱ��� slot
        else 
        {
            F_CheckDeialPanel();    // �ٸ� �г� �˻�

            _nowOpenPanel = v_idx;
            _slotDetailPanel[v_idx].gameObject.SetActive(v_flag);
        }
    }

    // �����ִ� �г� �˻�
    private void F_CheckDeialPanel() 
    {
        // ���� ���� �ִ� detail Panel �� �ִ��� �˻�, ������ ����
        if (_nowOpenPanel >= 0)  // ���� �ִ� �г��� ���� ��
            _slotDetailPanel[_nowOpenPanel].gameObject.SetActive(false);    // ���� �ִ� �г� ����

    }

    // Ui�� Info ������Ʈ
    private void F_UpdateHousingInfo(int v_type ,  int v_idx) 
    {
        if (_housingObject._blockDataList[v_type][v_idx] == null)
            return;

        HousingBlock _myblock = _housingObject._blockDataList[v_type][v_idx];        // �� ��° Ÿ���� idx �� �ش��ϴ� ��

        _infoBlockSprite.sprite  = _myblock.BlockSprite;                             // ui�� ���� ���� , sprite ����
        _infoBlockName.text      = _myblock.BlockName;                               // ui�� ���� ����, name ����
        _infoBlockToolTip.text   = _myblock.BlockToolTip;                            // ui�� ���� ����, tooltip ����

        _itemNeedImage[ _itemNeedImage.Count -1 ].gameObject.SetActive(false);      // 3��° ��� �ʱ�ȭ
        _itemNeedText[_itemNeedImage.Count - 1].text = "";                          // 3��° �̸� �ʱ�ȭ
        _itemnNeedCount[_itemNeedImage.Count - 1].text = "";                        // 3��° count �ʱ�ȭ

        // _blockDataList�� idx�� ����Ǿ� �ִ� HousingBlock��ũ��Ʈ�� _sourceList�� �����ؼ�
        // ��� ��������
        for (int i = 0; i < _myblock._sourceList.Count; i++) 
        {
            bool _flag = false;
            // 0. ��� slot ON
            _itemNeedImage[i].gameObject.SetActive(true);

            // 1. ��� ���� ���� ( ����,������ �ƴϸ� ������ ���� �ʿ� x )
            if (_myblock._sourceList[i].Item1 == 9)     // ������ ��ȣ�� '����'
            { 
                _itemNeedImage[i].sprite = _otherSprite[0];
                _flag = true;
            
            }
            else if (_myblock._sourceList[i].Item1 == 16)   // ������ ��ȣ�� '����'
            { 
                _itemNeedImage[i].sprite = _otherSprite[1];
                _flag = true;
            
            }

            // 2. ��� �̸� ���� > ������ �ε����� ������ �̸��� ���� (ItemManager)
            if(_flag)
                _itemNeedText[i].text = ItemManager.Instance.ItemDatas[_myblock._sourceList[i].Item1]._itemName;

            // 3. �κ��丮�� ������ ���� / �ʿ��� �������� ���� �� ����
            _itemnNeedCount[i].text  = ItemManager.Instance.itemCounter[_myblock._sourceList[i].Item1] + " / " + _myblock._sourceList[i].Item2.ToString();

        }
    }

}
