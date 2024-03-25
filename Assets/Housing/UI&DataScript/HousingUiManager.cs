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
    TextMeshProUGUI _infoBlockName;     // ������ �̸�
    [SerializeField]
    TextMeshProUGUI _infoBlockToolTip;  // ������ ���� (����)
    [SerializeField]
    List< Image > _itemNeedImage;             // ��� �̹���
    [SerializeField]
    List< TextMeshProUGUI > _itemNeedText;    // ��� �̸� �ؽ�Ʈ
    [SerializeField]
    List<TextMeshProUGUI> _itemnNeedCount;  // ��� ���� �ؽ�Ʈ

    [Space]
    [Header("Slot")]
    [SerializeField]
    GameObject _slotUiPrefab;           // ���� ui ������
    [SerializeField]
    GameObject[] _craftSlotList;        // ī�װ� ����
    [SerializeField]
    Sprite[] _craftSlotsSprite;         // ī�װ� ���Կ� ���� ��������Ʈ

    [Space]
    [Header("Slot Detail Panel")]
    [SerializeField]
    GameObject[] _slotDetailPanel;      // ���� detail �г�
    [SerializeField]
    GameObject[] _detailPanelContent;   // detail �г� �� ��ũ�� �� ���� content 

    [Space]
    [Header("now Open Panel & Detail Panel")]
    [SerializeField]
    private int _nowOpenPanel;          // ���� ���� panel �˻�
    [SerializeField]
    private int _nowOpenDetailSlot;     // ���� ���� �� panel �� slot idx ����

    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        F_SetMouseMove(false);

        F_InitCraftSlotIdx();       // ī�װ� ���� �ʱ� ����
        F_ClontSlotInDetail();      // detail â ���� Slot ����

        F_UpdateHousingInfo( 0 , 0 );     // �ʱ� Info â ����
    }
    // housing UI On Off 
    private void Update()
    {
        if (Input.GetMouseButton(1))        // ��Ŭ���� �ϰ� �ִ� ����
            F_OnOffCraftCanvas(true);       // canvas ���̰�
        else if (Input.GetMouseButtonUp(1)) // ��Ŭ�� ����
        {
            F_OnOffCraftCanvas(false);      // cavas �Ⱥ��̰�

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

    // �÷��̾� Ŀ�� ���
    private void F_SetMouseMove(bool v_mode)
    {
        if ( v_mode == false )
        {
            Cursor.lockState = CursorLockMode.Locked;        // Ŀ���� 'ȭ�� ���߾�'�� ������Ŵ
            Cursor.visible = false;                          // Ŀ�� �� ���̰�
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;             // Ŀ�� �������
            Cursor.visible = true;                              // Ŀ�� ���̰�
        }
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

    // detail Panel �� Itemslot �߰��ϱ�
    private void F_ClontSlotInDetail() 
    {
        // Housing object �����ϴ� ��ũ��Ʈ����, ����Ǿ��ִ� obj ��ŭ slot �߰� 
        // ���߿� ��ü �ʱ�ȭ �� �� _housingObject._housingObjList.Count; ��ŭ for�� �� ������ �� 

        for (int i = 0; i < _housingObject._blockDataList.Count; i++) 
        {
            for (int j = 0; j < _housingObject._blockDataList[i].Count; j++)
            {
                // Slot ����
                GameObject _cloneSlot = Instantiate(_slotUiPrefab);
                // detail �г� ���� scroll view�� content �ؿ� �߰�
                _cloneSlot.transform.parent = _detailPanelContent[i].transform;

                CraftSlot _cslot = _cloneSlot.GetComponent<CraftSlot>();
                _cslot.idx = ((i + 1) * 10) + j;           // detail �� slot�� index�� ������� 10,11,12,13....
                _cslot.F_SetImageIcon(_housingObject._blockDataList[i][j].BlockSprite);

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
        // idx 10 �Ѵ°Ŵ� detail Panel ���� slot �� -> ���� �ε��� ���常
        if (v_idx >= 10) 
        {
            _nowOpenDetailSlot = v_idx;

            F_UpdateHousingInfo(_nowOpenPanel, v_idx % 10);                // panel���� Slot�� idx�� NN �̴ϱ� ������ �˻��ؼ� upate�ϱ�
        }
        // 10 ���� idx�� �ٱ��� slot
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
    private void F_UpdateHousingInfo(int panel ,  int v_idx) 
    {
        if (_housingObject._blockDataList[panel][v_idx] == null)
            Debug.Log( panel + " / " + v_idx);

        HousingBlock _myblock = _housingObject._blockDataList[panel][v_idx];

        _infoBlockSprite.sprite  = _myblock.BlockSprite;
        _infoBlockName.text      = _myblock.BlockName;
        _infoBlockToolTip.text   = _myblock.BlockToolTip;

        // _blockDataList�� idx�� ����Ǿ� �ִ� HousingBlock��ũ��Ʈ�� _sourceList�� �����ؼ�
        // ��� ��������
        for (int i = 0; i < _myblock._sourceList.Count; i++) 
        {
            _itemNeedImage[i].gameObject.SetActive(true);
            _itemNeedText[i].text    = _myblock._sourceList[i].Item1;      // ��� �̸��� ����
            _itemnNeedCount[i].text  = "0" + " / " + _myblock._sourceList[i].Item2.ToString();
            // #TODO
            // ������ ���� "0" �κ��� inventory�� ���� ������ �ִ� ������ �������� �����ؾ���
        }
    }

}
