using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filter : Furniture
{
    [SerializeField] private float _filterRange;         // 필터기 범위

    [Header("=== Filter Information ===")]
    [SerializeField] LayerMask _layerMask;

    [Header("Check Range")]
    [SerializeField] private GameObject _rangeObject;

    private Furniture _tmpFurniture;
    protected override void F_InitFurniture()
    {
        _rangeObject.SetActive(false);

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

    /// <summary> 범위안에 있는 모든 설치물의 필터상태 변환</summary>
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

    public override void F_Interaction()
    {
        _rangeObject.SetActive(!_rangeObject.activeSelf);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _filterRange);
    }

    public override string F_GetData()
    {
        string jsonData = "NONE";
        return jsonData;
    }

    public override void F_SetData(string v_data)
    {

    }



}
