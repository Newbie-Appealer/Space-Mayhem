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

    [Header("Housing Block Info")]
    [SerializeField]
    Image _infoBlockSprite;
    [SerializeField]
    TextMeshProUGUI _infoBlockName;
    [SerializeField]
    TextMeshProUGUI _infoBlockToolTip;

    [Header("Slot")]
    [SerializeField]
    GameObject _slotUiPrefab;           // ���� ui ������
    [SerializeField]
    GameObject[] _craftSlotList;        // ī�װ��� ����
    [SerializeField]
    Sprite[] _craftSlotsSprite;         // ī�װ��� ���Կ� ���� ��������Ʈ

    [Header("Slot Detail Panel")]
    [SerializeField]
    GameObject[] _slotDetailPanel;      // ���� detail �г�
    [SerializeField]
    GameObject[] _detailPanelContent;   // detail �г� �� ��ũ�� �� ���� content 

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
        
        F_InitCraftSlotIdx();       // ī�װ��� ���� �ʱ� ����
        F_ClontSlotInDetail();      // detail â ���� Slot ����

        F_UpdateHousingInfo( 0 ,0);     // �ʱ� Info â ����
    }

    // ī�װ��� ���� �ʱ� ����
    public void F_InitCraftSlotIdx() 
    {
        for (int i = 0; i < _craftSlotList.Length; i++) 
        {
            CraftSlot _cf = _craftSlotList[i].GetComponent<CraftSlot>();
            _cf.idx = i;
            _cf.F_SetImageIcon(_craftSlotsSprite[i]);
        }
    }

    // detail Panel �� Itemslot �߰��ϱ�
    public void F_ClontSlotInDetail() 
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
    public void F_CheckDeialPanel() 
    {
        // ���� ���� �ִ� detail Panel �� �ִ��� �˻�, ������ ����
        if (_nowOpenPanel >= 0)  // ���� �ִ� �г��� ���� ��
            _slotDetailPanel[_nowOpenPanel].gameObject.SetActive(false);    // ���� �ִ� �г� ����

    }

    // Ui�� Info ������Ʈ
    private void F_UpdateHousingInfo(int panel ,  int v_idx) 
    {
        _infoBlockSprite.sprite  = _housingObject._blockDataList[panel][v_idx].BlockSprite;
        _infoBlockName.text      = _housingObject._blockDataList[panel][v_idx].BlockName;
        _infoBlockToolTip.text   = _housingObject._blockDataList[panel][v_idx].BlockToolTip;
    }

}