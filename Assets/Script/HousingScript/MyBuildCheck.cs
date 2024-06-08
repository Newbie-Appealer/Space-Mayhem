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
    [SerializeField] List<Image> _sp;                       // ���� �� �߰��� �ߴ� ��� �̹��� list
    [SerializeField] List<Image> _backSprite;               // ��� ��������Ʈ 
    [SerializeField] List<TextMeshProUGUI> _sourcetext;     // ���� �� �߰��� �ߴ� ��� ���� text
    [SerializeField] Sprite _redSprite;                     // ��ᰡ �����ϸ�? ����
    [SerializeField] Sprite _noneSprite;                    // ��ᰡ ����ϸ�? �⺻

    // BuildMaster���� �޾ƿ� HousingBlock ������ 
    [SerializeField] public HousingBlock _myblock;

    // ���� ��� ����
    private List<int> _currSourceCount = new List<int>();
    // ����ϸ� true,  �ƴϸ� false 
    private List<bool> _isEnough = new List<bool>();

    // building Manager���� �κ��� �ִ� �� ���� check�� ui ���� 
    public void F_BuildingStart() 
    {
        // 0. ���� ���
        F_CheckMyBlockSource();

        // 1. ����� ������ ���� ui ������Ʈ
        F_UpdateProgressUI();
    }

    // inven�� ������ ���� ������Ʈ ( Building manager���� item�� build���� �� ����ؾ��� )
    public void F_UpdateInvenToBuilding() 
    {
        for (int i = 0; i < _myblock._sourceList.Count; i++)
        {
            ItemManager.Instance.inventorySystem.F_UpdateItemUsing
                (_myblock._sourceList[i].Item1, _myblock._sourceList[i].Item2);
        }

    }

    // �����ϴµ� �ʿ��� ��� �˻� ��, list�� ���� ���
    private void F_CheckMyBlockSource()
    {
        // Housing UI���� ��ġ�� �� , BuildMaster�� ������ HousingBlock ��������
        _myblock = BuildMaster.Instance.currBlockData;

        _currSourceCount.Clear();
        _isEnough.Clear();

        int itemidx, needCnt;

        for (int i = 0; i < _myblock._sourceList.Count; i++)
        {
            itemidx = _myblock._sourceList[i].Item1;        // ������ num
            needCnt = _myblock._sourceList[i].Item2;        // ������ �ʿ䰹��

            // ���� count�� �ʿ��� �������� ������
            if (ItemManager.Instance.itemCounter[itemidx] >= needCnt)
                _isEnough.Add(true);        // true����
            else
                _isEnough.Add(false);       // false ����

            // ���� �κ��� �ش� �������� �󸶳� ����ִ��� ���� 
            _currSourceCount.Add(ItemManager.Instance.itemCounter[itemidx]);
        }
    }

    // ��ᰡ �� ����� �ִ��� �˻�
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
        // 1. �ʱ�ȭ
        // ui ������Ʈ �� �� , ��ü ����� ���� , �̹��� ,�ؽ�Ʈ�� �ʱ�ȭ �������
        for (int i = 0; i < _sp.Count; i++) 
        {
            _sp[i].gameObject.SetActive(false);
            _backSprite[i].gameObject.SetActive(false);
            _sourcetext[i].text = "";
        }

        // 2. ui ON 
        for (int i = 0; i < _myblock._sourceList.Count; i++) 
        {
            // 2-1. ui �ѱ� 
            _sp[i].gameObject.SetActive(true);
            _backSprite[i].gameObject.SetActive(true);
            
            // �������� true �̸� ?
            if (_isEnough[i] == true)
                F_UpdateInBuldingMode(_noneSprite, i);
            
            // �������� false �̸�?
            else
                F_UpdateInBuldingMode(_redSprite, i);
            
        }
    }

    private void F_UpdateInBuldingMode( Sprite v_sp , int v_idx ) 
    {
        int itemIDX = _myblock._sourceList[v_idx].Item1; // ������num

        // ���� ��� sp�� item ��������Ʈ�� 
        _sp[v_idx].sprite = ResourceManager.Instance.F_GetInventorySprite(itemIDX);

        // ��׶��� ��������Ʈ�� �Ű����� ��������Ʈ��
        _backSprite[v_idx].sprite = v_sp;

        // �ؽ�Ʈ�� "���簹��/�ʿ䰹��"��
        _sourcetext[v_idx].text = _currSourceCount[v_idx].ToString() +  "/"  + _myblock._sourceList[v_idx].Item2.ToString();

    }
}
