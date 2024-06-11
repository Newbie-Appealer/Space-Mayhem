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
    [Header("===Building Panel===")]
    [SerializeField]
    public GameObject _buildingBlockSelectUi;     // BLock ���� ui 
    [SerializeField] 
    public GameObject _buildingProgressUi;        // ���� ���� ui ( houising ui�� ���� �� On , housingui�� ���� �� Off)

    // �Ͽ�¡ �� info
    [Space]
    [Header("===Housing Block Info===")]
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
    [Header("===Slot===")]
    [SerializeField]
    GameObject _slotUiPrefab;           // ���� ui ������
    [SerializeField]
    GameObject[] _craftSlotList;        // ī�װ� ����
    [SerializeField]
    Sprite[] _craftSlotsSprite;         // ī�װ� ����� ���Կ� ���� ��������Ʈ

    // ī�װ� detail 
    [Space]
    [Header("===Slot Detail Panel===")]
    [SerializeField]
    GameObject[] _slotDetailPanel;      // ���� detail �г�
    [SerializeField]
    GameObject[] _detailPanelContent;   // detail �г� �� ��ũ�� �� ���� content 

    // ���� ���� �� type, detail �г� index
    [Space]
    [Header("===now Open Panel & Detail Panel===")]
    [SerializeField]
    private int _nowOpenPanel;          // ���� ���� panel �˻�
    [SerializeField]
    private int _nowOpenDetailSlot;     // ���� ���� �� panel �� slot idx ����

    [Header("===Repair Text===")]
    [SerializeField] TextMeshProUGUI _repairToolText;

    private void Start()
    {
        F_InitCraftSlotIdx();       // ī�װ� ���� �ʱ� ����
        F_ClontSlotInDetail();      // detail â ���� Slot ����

        F_UpdateHousingInfo( 0 , 0 );     // �ʱ� Info â ����
    }

    // ������Ʈ�� ���� ��
    private void OnEnable()
    {
        _nowOpenPanel = -1;     // ���� �����ִ� panel idx�� -1��
    }

    public void F_OnOffRepairText(MyBuildingBlock v_block ,bool v_flag)
    {
        // TODO : �Ͽ�¡ repair �� �� text onoff ����߰� 
        _repairToolText.gameObject.SetActive(v_flag);

        if ( v_block != null && v_flag)
            _repairToolText.text = v_block.MyBlockHp.ToString() + " / " + v_block.MyBlockMaxHp.ToString();
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

    // Housing Ui On 
    private void F_WhenHousingUiOn() 
    {
        // 0. inventory �ʱ�ȭ
        ItemManager.Instance.F_UpdateItemCounter();
        
        // 1. canvas , cursor On
        F_OnOffCraftCanvas(true);

        // 2. progress ui �� off
        _buildingProgressUi.gameObject.SetActive(false);
    }

    // Housing Ui Off
    private void F_WhenHousingUiOff() 
    {
        // 1. canvas , cursor OFF
        F_OnOffCraftCanvas(false);

        // 2. Build Master�� ���� block Data�� �����س��� 
        if (_nowOpenPanel < 0 || _nowOpenDetailSlot < 0)        // idx�� 0 �̸��̸� return 
            return;

        // 3. BuildMaster�� BlockData �־���� 
        BuildMaster.Instance.
            F_SetBlockData( BuildMaster.Instance.housingDataManager.blockDataList[_nowOpenPanel][_nowOpenDetailSlot]);

        // BuildMaanger�� index �ű��
        BuildMaster.Instance.myBuildManger.F_GetbuildType(_nowOpenPanel, _nowOpenDetailSlot);

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
        _buildingBlockSelectUi.SetActive(v_check);

        // 2. Ŀ��
        GameManager.Instance.F_SetCursor(v_check);
    }

    // ī�װ� ���� �ʱ� ����
    private void F_InitCraftSlotIdx() 
    {
        for (int i = 0; i < _craftSlotList.Length; i++)
        {
            CraftSlot _cf = _craftSlotList[i].GetComponent<CraftSlot>();
            _cf.typeNum = BuildMaster.Instance.housingDataManager.blockDataList[i][0].blockTypeNum;
            _cf.detialNum = BuildMaster.Instance.housingDataManager.blockDataList[i][0].blockDetailNum;

            // 1. ��������Ʈ ���� 
            _cf.F_SetImageIcon(_craftSlotsSprite[i]);
        }
    }

    // detail Panel �� Itemslot �߰�
    private void F_ClontSlotInDetail() 
    {
        // �Ͽ�¡ Manager��
        for (int i = 0; i < BuildMaster.Instance.housingDataManager.blockDataList.Count; i++) 
        {
            for (int j = 0; j < BuildMaster.Instance.housingDataManager.blockDataList[i].Count; j++)
            {
                // 0. Slot ����
                GameObject _cloneSlot = Instantiate(_slotUiPrefab);
                // 1. detail �г� ���� scroll view�� content �ؿ� �߰�
                _cloneSlot.transform.parent = _detailPanelContent[i].transform;

                CraftSlot _cslot = _cloneSlot.GetComponent<CraftSlot>();
                // 0. �ε��� ���� 
                _cslot.typeNum = BuildMaster.Instance.housingDataManager.blockDataList[i][j].blockTypeNum;
                _cslot.detialNum = BuildMaster.Instance.housingDataManager.blockDataList[i][j].blockDetailNum;

                // 1. �̹��� ���� 
                _cslot.F_SetImageIcon(BuildMaster.Instance.housingDataManager.blockDataList[i][j].blockSprite);     // �� �������� ��������Ʈ �����ͼ� ����

            }
        }   
    }


    // Panel ���� �ѱ�
    public void F_OnOffDtailPanel(int v_type , int v_detail) 
    {
        // ���� openDetailSlot�̶�, ���� ���� v_Type ������?
        if ( _nowOpenPanel ==  v_type ) 
            _nowOpenDetailSlot = v_detail;                               // detail ���� 

        // `` �ٸ��� ?
        else 
        {
            if(_nowOpenPanel >= 0 )
                _slotDetailPanel[_nowOpenPanel].gameObject.SetActive(false);      // �̹� �����ִ� panel ���� 

            _nowOpenPanel = v_type;                                    // type ���� 
            _slotDetailPanel[v_type].gameObject.SetActive(true);       // detail �г� �ѱ�
        }

        F_UpdateHousingInfo(v_type, v_detail);                // panel���� Slot�� idx�� NN �̴ϱ� ������ �˻��ؼ� upate�ϱ�
    }


    // Ui�� Info ������Ʈ
    private void F_UpdateHousingInfo(int v_type ,  int v_idx) 
    {
        if ( BuildMaster.Instance.housingDataManager.blockDataList[v_type][v_idx] == null )
            return;

        HousingBlock _myblock = BuildMaster.Instance.housingDataManager.blockDataList[v_type][v_idx];        // �� ��° Ÿ���� idx �� �ش��ϴ� ��

        _infoBlockSprite.sprite  = _myblock.blockSprite;                             // ui�� ���� ���� , sprite ����
        _infoBlockName.text      = _myblock.lockName;                                // ui�� ���� ����, name ����
        _infoBlockToolTip.text   = _myblock.blockToolTip;                            // ui�� ���� ����, tooltip ����

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
