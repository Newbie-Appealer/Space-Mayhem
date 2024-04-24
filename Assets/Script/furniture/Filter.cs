using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filter : Furniture
{
    [SerializeField] private float _filterRange;        // ���ͱ� ����
    [SerializeField] private float _filterMaxHP;           // ���ͱ� ��밡�� Ƚ�� ( �ʱ� )
    [SerializeField] private float _filterCurrentHP;           // ���ͱ� ��밡�� Ƚ�� ( ���� )

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
        F_OnFilterFurnitures();

        while(true)
        {
            yield return new WaitForSeconds(15f);
            F_OnFilterFurnitures();
        }
    }

    /// <summary> �����ȿ� �ִ� ��� ��ġ���� ���ͻ��� ��ȯ</summary>
    private void F_OnFilterFurnitures()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _filterRange, _layerMask);

        foreach(Collider collider in colliders)
        {
            try
            {
                _tmpFurniture = collider.transform.parent.GetComponent<Furniture>();

                if (_tmpFurniture == null)
                    continue;

                _tmpFurniture.F_ChangeFilterState(true);
            }
            catch(Exception ex)
            {
                Debug.Log(ex + "Null Component");
            }
        }
    }

    public void F_UseHP()
    {
        // TODO:����ü�°��� �� ü��0 ->
        // ������Ʈ �ı� ->
        // ���� ���ͱⰡ �ʿ��� ������Ʈ���� Filter ���¸� false�� ����
    }

    #region ��ȣ�ۿ� �Լ�
    public override void F_Interaction()
    {
        _rangeObject.SetActive(!_rangeObject.activeSelf);
    }
    #endregion

    #region ����/ �ҷ����� ����
    public override string F_GetData()
    {
        string jsonData = "NONE";
        return jsonData;
    }

    public override void F_SetData(string v_data)
    {

    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _filterRange);
    }
}
