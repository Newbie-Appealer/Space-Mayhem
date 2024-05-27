using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HousingUiManager : MonoBehaviour
{

    // �Ͽ�¡ canvas
    [Header("Building Panel")]
    [SerializeField]
    public GameObject _buildingCanvas;
    [SerializeField] 
    public GameObject _buildingProgressUi;        // ���� ���� ui ( houising ui�� ���� �� On , housingui�� ���� �� Off)

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
        ItemManager.Instance.F_UpdateItemCounter();
        
        // 1. canvas , cursor On
        F_OnOffCraftCanvas(true);

        // 2. progress ui �� off
        _buildingProgressUi.gameObject.SetActive(false);
    }
    private void F_WhenHousingUiOff() 
    {
        // 1. canvas , cursor OFF
        F_OnOffCraftCanvas(false);

        // 2. Build Master�� ���� block Data�� �����س��� 
        if (_nowOpenPanel < 0 || _nowOpenDetailSlot < 0)        // idx�� 0 �̸��̸� return 
            return;
        BuildMaster.Instance.
            F_SetBlockData( BuildMaster.Instance.housingDataManager._blockDataList[_nowOpenPanel][_nowOpenDetailSlot % 10]);

        // BuildMaanger�� index �ű��
        BuildMaster.Instance.myBuildManger.F_GetbuildType(_nowOpenPanel, _nowOpenDetailSlot % 10);
    }

    // MyBuildingBlock ��ũ��Ʈ
    public void F_OnOFfBuildingProgressUi(bool v_flag) 
    {
        _buildingProgressUi.gameObject.SetActive(v_flag);
    }

    // On Off builgind panel
    private void F_OnOffCraftCanvas(bool v_check)
    {
        // 1. panel OnOff
        _buildingCanvas.SetActive(v_check);

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
        // �Ͽ�¡ Manager��
        for (int i = 0; i < BuildMaster.Instance.housingDataManager._blockDataList.Count; i++) 
        {
            for (int j = 0; j < BuildMaster.Instance.housingDataManager._blockDataList[i].Count; j++)
            {
                // 0. Slot ����
                GameObject _cloneSlot = Instantiate(_slotUiPrefab);
                // 1. detail �г� ���� scroll view�� content �ؿ� �߰�
                _cloneSlot.transform.parent = _detailPanelContent[i].transform;

                CraftSlot _cslot = _cloneSlot.GetComponent<CraftSlot>();
                _cslot.idx = ((i + 1) * 10) + j;                                            // detail �� slot�� index�� ������� 10,11,12,13....
                _cslot.F_SetImageIcon(BuildMaster.Instance.housingDataManager._blockDataList[i][j].BlockSprite);     // �� �������� ��������Ʈ �����ͼ� ����

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
        if (BuildMaster.Instance.housingDataManager._blockDataList[v_type][v_idx] == null)
            return;

        HousingBlock _myblock = BuildMaster.Instance.housingDataManager._blockDataList[v_type][v_idx];        // �� ��° Ÿ���� idx �� �ش��ϴ� ��

        _infoBlockSprite.sprite  = _myblock.BlockSprite;                             // ui�� ���� ���� , sprite ����
        _infoBlockName.text      = _myblock.BlockName;                               // ui�� ���� ����, name ����
        _infoBlockToolTip.text   = _myblock.BlockToolTip;                            // ui�� ���� ����, tooltip ����

        // Ui�� ���� �Ʒ���, ������ source 3��°�� �ʱ�ȭ �� ����
        F_InitUnderLeftUi();

        // Ui�� ���� �Ʒ���, ������ source ����
        for (int i = 0; i < _myblock._sourceList.Count; i++) 
        {
            // 0. ��� slot ON
            _itemNeedImage[i].gameObject.SetActive(true);
            
            // 1. ��� ���� �ֱ�
            _itemNeedImage[i].sprite = ResourceManager.Instance.F_GetInventorySprite(_myblock._sourceList[i].Item1);

            // 2. ��� �̸� ���� > ������ �ε����� ������ �̸��� ���� (ItemManager)
            _itemNeedText[i].text = ItemManager.Instance.ItemDatas[ _myblock._sourceList[i].Item1 ]._itemName;

            // 3. �κ��丮�� ������ ���� / �ʿ��� �������� ���� �� ����
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
