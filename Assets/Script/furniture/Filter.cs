using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterWrapper
{
    public int _filterHP;

    public FilterWrapper(int filterHP)
    {
        _filterHP = filterHP;
    }
}

public class Filter : Furniture
{
    [SerializeField] private float _filterRange;          // ���ͱ� ����
    [SerializeField] private int _filterMaxHP;            // ���ͱ� ��밡�� Ƚ�� ( �ʱ� )
    [SerializeField] private int _filterCurrentHP;        // ���ͱ� ��밡�� Ƚ�� ( ���� )

    [Header("=== Filter Information ===")]
    [SerializeField] LayerMask _layerMask;

    [Header("Check Range")]
    [SerializeField] private GameObject _rangeObject;

    private Furniture _tmpFurniture;
    protected override void F_InitFurniture()
    {
        _rangeObject.SetActive(false);

        _filterCurrentHP = _filterMaxHP;

        StartCoroutine(C_CheckFurnitures());
    }

    IEnumerator C_CheckFurnitures()
    {
        yield return new WaitForSeconds(0.5f);
        F_OnFilterFurnitures(true);

        while(true)
        {
            yield return new WaitForSeconds(10f);
            F_OnFilterFurnitures(true);
        }
    }

    /// <summary> �����ȿ� �ִ� ��� ��ġ���� ���ͻ��� ��ȯ</summary>
    private void F_OnFilterFurnitures(bool v_bValue)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _filterRange, _layerMask);

        foreach(Collider collider in colliders)
        {
            try
            {
                _tmpFurniture = collider.transform.parent.GetComponent<Furniture>();

                if (_tmpFurniture == null)
                    continue;

                _tmpFurniture.F_ChangeFilterState(v_bValue);
            }
            catch(Exception ex)
            {
                Debug.Log(ex + "Null Component");
            }
        }
    }

    public void F_UseHP(int v_value)
    {
        // TODO:FILTER ����ϴ°����� �� ȣ���������..
        // ȸ���� ��ġ�� HP�� �ұ�?

        _filterCurrentHP -= v_value;        // 1. ü�°���
        if(_filterCurrentHP <= 0)           // 2. ü�� Ȯ�� ( ü���� �� �￴����
        {
            F_OnFilterFurnitures(false);    // 3. ������ ��� ��ġ�� ���� OFF
            Destroy(this.gameObject);       // 4. ������ �ı�
        }
    }

    #region ��ȣ�ۿ� �Լ�
    public override void F_Interaction()
    {
        _rangeObject.SetActive(!_rangeObject.activeSelf);
    }
    public override void F_TakeFurniture()
    {
        // ���ͱ�� ȸ������ �ٷ� �ı�.

        F_OnFilterFurnitures(false);                                // ������ ��� ��ġ�� ���� OFF
        // �ٸ� ���ͱ�� �Բ� �پ��ִ� ������Ʈ�� 0~10�� �� �ٽ� ��������
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();

        Destroy(this.gameObject);                                   // ������ �ı�
    }

    #endregion

    #region ����/ �ҷ����� ����
    public override string F_GetData()
    {
        FilterWrapper filterData = new FilterWrapper(_filterCurrentHP);
        string jsonData = JsonUtility.ToJson(filterData);
        string base64Data = GameManager.Instance.F_EncodeBase64(jsonData);
        return base64Data;
    }

    public override void F_SetData(string v_data)
    {
        string dataString = GameManager.Instance.F_DecodeBase64(v_data);

        FilterWrapper data = JsonUtility.FromJson<FilterWrapper>(dataString);

        _filterCurrentHP = data._filterHP;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _filterRange);
    }
}
