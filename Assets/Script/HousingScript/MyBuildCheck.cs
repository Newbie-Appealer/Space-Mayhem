using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MyBuildCheck : MonoBehaviour
{
    [Header("==== Housing Progress Ui ====")]
    [SerializeField] List<Image> _sp;                       // 건축 시 중간에 뜨는 재료 이미지 list
    [SerializeField] List<Image> _backSprite;               // 배경 스프라이트 
    [SerializeField] List<TextMeshProUGUI> _sourcetext;     // 건축 시 중간에 뜨는 재료 갯수 text
    [SerializeField] Sprite _redSprite;                     // 재료가 부족하면? 빨강
    [SerializeField] Sprite _noneSprite;                    // 재료가 충분하면? 기본

    // BuildMaster에서 받아온 HousingBlock 데이터 
    [SerializeField] public HousingBlock _myblock;

    // 현재 재료 갯수
    private List<int> _currSourceCount = new List<int>();
    // 충분하면 true,  아니면 false 
    private List<bool> _isEnough = new List<bool>();

    // building Manager에서 인벤에 있는 블럭 정보 check와 ui 설정 
    public void F_BuildingStart() 
    {
        // 0. 정보 담기
        F_CheckMyBlockSource();

        // 1. 담겨진 정보에 대해 ui 업데이트
        F_UpdateProgressUI();
    }

    // inven에 아이템 갯수 업데이트 ( Building manager에서 item을 build했을 때 사용해야함 )
    public void F_UpdateInvenToBuilding() 
    {
        for (int i = 0; i < _myblock._sourceList.Count; i++)
        {
            ItemManager.Instance.inventorySystem.F_UpdateItemUsing
                (_myblock._sourceList[i].Item1, _myblock._sourceList[i].Item2);
        }

    }

    // 건축하는데 필요한 재료 검사 후, list에 정보 담기
    private void F_CheckMyBlockSource()
    {
        // Housing UI에서 설치할 때 , BuildMaster에 저장한 HousingBlock 가져오기
        _myblock = BuildMaster.Instance.currBlockData;

        _currSourceCount.Clear();
        _isEnough.Clear();

        int itemidx, needCnt;

        for (int i = 0; i < _myblock._sourceList.Count; i++)
        {
            itemidx = _myblock._sourceList[i].Item1;        // 아이템 num
            needCnt = _myblock._sourceList[i].Item2;        // 아이템 필요갯수

            // 현재 count가 필요한 갯수보다 많으면
            if (ItemManager.Instance.itemCounter[itemidx] >= needCnt)
                _isEnough.Add(true);        // true저장
            else
                _isEnough.Add(false);       // false 저장

            // 현재 인벤이 해당 아이템이 얼마나 들어있는지 저장 
            _currSourceCount.Add(ItemManager.Instance.itemCounter[itemidx]);
        }
    }

    // 재료가 다 충분히 있는지 검사
    public bool F_WholeSourseIsEnough() 
    {
        int n = 0;
        for( int i = 0; i< _myblock._sourceList.Count; i++) 
        {
            if (_isEnough[i] == true)
                n++;
        }

        if ( n == _myblock._sourceList.Count)
            return true;
        return false;
    }

    private void F_UpdateProgressUI() 
    {
        // 1. 초기화
        // ui 업데이트 할 때 , 전체 재료의 바탕 , 이미지 ,텍스트는 초기화 해줘야함
        for (int i = 0; i < _sp.Count; i++) 
        {
            _sp[i].gameObject.SetActive(false);
            _backSprite[i].gameObject.SetActive(false);
            _sourcetext[i].text = "";
        }

        // 2. ui ON 
        for (int i = 0; i < _myblock._sourceList.Count; i++) 
        {
            // 2-1. ui 켜기 
            _sp[i].gameObject.SetActive(true);
            _backSprite[i].gameObject.SetActive(true);
            
            // 재료충분이 true 이면 ?
            if (_isEnough[i] == true)
                F_UpdateInBuldingMode(_noneSprite, i);
            
            // 재료충분이 false 이면?
            else
                F_UpdateInBuldingMode(_redSprite, i);
            
        }
    }

    private void F_UpdateInBuldingMode( Sprite v_sp , int v_idx ) 
    {
        int itemIDX = _myblock._sourceList[v_idx].Item1; // 아이템num

        // 현재 재료 sp를 item 스프라이트로 
        _sp[v_idx].sprite = ResourceManager.Instance.F_GetInventorySprite(itemIDX);

        // 백그라운드 스프라이트를 매개변수 스프라이트로
        _backSprite[v_idx].sprite = v_sp;

        // 텍스트를 "현재갯수/필요갯수"로
        _sourcetext[v_idx].text = _currSourceCount[v_idx].ToString() +  "/"  + _myblock._sourceList[v_idx].Item2.ToString();

    }
}
