using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftUiManager : MonoBehaviour
{
    public static CraftUiManager intance;

    [Header("Slot")]
    [SerializeField]
    GameObject[] _craftSlotList;
    [SerializeField]
    Sprite[] _craftSlotsSprite;

    [Header("Slot Detail Panel")]
    private int _nowOpenPanel;
    [SerializeField]
    GameObject[] _slotDetailPanel;

    private void Awake()
    {
        intance = this;

        F_InitCraftSlotIdx();
    }

    // Slot�� index ����
    public void F_InitCraftSlotIdx() 
    {
        for (int i = 0; i < _craftSlotList.Length; i++) 
        {
            if (_craftSlotList[i].GetComponent<CraftSlot>() == null)
                _craftSlotList[i].AddComponent<CraftSlot>();

            _craftSlotList[i].GetComponent<CraftSlot>().idx = i;
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
        F_CheckDeialPanel();    // �ٸ� �г� �˻�

        _nowOpenPanel = v_idx;
        _slotDetailPanel[v_idx].gameObject.SetActive(v_flag);
    }

    // �����ִ� �г� �˻�
    public void F_CheckDeialPanel() 
    {
        // ���� ���� �ִ� detail Panel �� �ִ��� �˻�, ������ ����
        if (_nowOpenPanel >= 0)  // ���� �ִ� �г��� ���� ��
            _slotDetailPanel[_nowOpenPanel].gameObject.SetActive(false);    // ���� �ִ� �г� ����

    }


}
