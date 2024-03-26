using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MyBuildCheck : MonoBehaviour
{
    [SerializeField]
    List<Image> _sp;       // ���� �� �߰��� �ߴ� ��� �̹��� list
    [SerializeField]
    List<Image> _backSprite;   // ��� ��������Ʈ 
    [SerializeField]
    List<TextMeshProUGUI> _sourcetext;    // ���� �� �߰��� �ߴ� ��� ���� text
    [SerializeField]
    Sprite _redSprite;          // ��ᰡ �����ϸ�? ����
    [SerializeField]
    Sprite _noneSprite;         // ��ᰡ ����ϸ�? �⺻

    HousingBlock _myblock;

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
            InventorySystem.Instance.F_UpdateItemUsing
                (_myblock._sourceList[i].Item1, _myblock._sourceList[i].Item2);
        }

    }

    // �����ϴµ� �ʿ��� ��� �˻� ��, list�� ���� ���
    private void F_CheckMyBlockSource()
    {
        // Housing Ui ��ũ��Ʈ�� ����� _currHousingBlock�� �����ؼ� ��� �˻�
        _myblock = HousingUiManager.Instance._currHousingBlock;

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

        if (n == _myblock._sourceList.Count)
            return true;
        return false;
    }

    private void F_UpdateProgressUI() 
    {
        // ui ������Ʈ �� �� , 3��° ����� ���� , �̹��� ,�ؽ�Ʈ�� �ʱ�ȭ �������
        _sp[ _sp.Count - 1].sprite           = null;
        _backSprite[ _backSprite.Count - 1].sprite   = null;
        _sourcetext[ _sourcetext.Count - 1].text = "";

        for (int i = 0; i < _myblock._sourceList.Count; i++) 
        {
            if (_isEnough[i] == true)
            {
                F_UpdateInBuldingMode(_noneSprite, i);
            }
            else
            {
                F_UpdateInBuldingMode(_redSprite, i);
            }
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
