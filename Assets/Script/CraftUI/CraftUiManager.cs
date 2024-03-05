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

    // Slot의 index 설정
    public void F_InitCraftSlotIdx() 
    {
        for (int i = 0; i < _craftSlotList.Length; i++) 
        {
            if (_craftSlotList[i].GetComponent<CraftSlot>() == null)
                _craftSlotList[i].AddComponent<CraftSlot>();

            _craftSlotList[i].GetComponent<CraftSlot>().idx = i;
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
        F_CheckDeialPanel();    // 다른 패널 검사

        _nowOpenPanel = v_idx;
        _slotDetailPanel[v_idx].gameObject.SetActive(v_flag);
    }

    // 켜져있는 패널 검산
    public void F_CheckDeialPanel() 
    {
        // 현재 켜져 있는 detail Panel 이 있는지 검사, 있으면 끄기
        if (_nowOpenPanel >= 0)  // 열려 있는 패널이 있을 때
            _slotDetailPanel[_nowOpenPanel].gameObject.SetActive(false);    // 열려 있는 패널 끄기

    }


}
