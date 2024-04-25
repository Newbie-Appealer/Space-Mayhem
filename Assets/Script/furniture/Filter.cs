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
    [SerializeField] private float _filterRange;          // 필터기 범위
    [SerializeField] private int _filterMaxHP;            // 필터기 사용가능 횟수 ( 초기 )
    [SerializeField] private int _filterCurrentHP;        // 필터기 사용가능 횟수 ( 현재 )

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

    /// <summary> 범위안에 있는 모든 설치물의 필터상태 변환</summary>
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
        // TODO:FILTER 사용하는곳에서 잘 호출해줘야함..
        // 회복한 수치를 HP로 할까?

        _filterCurrentHP -= v_value;        // 1. 체력감소
        if(_filterCurrentHP <= 0)           // 2. 체력 확인 ( 체력이 다 깍였을때
        {
            F_OnFilterFurnitures(false);    // 3. 범위내 모든 설치물 필터 OFF
            Destroy(this.gameObject);       // 4. 아이템 파괴
        }
    }

    #region 상호작용 함수
    public override void F_Interaction()
    {
        _rangeObject.SetActive(!_rangeObject.activeSelf);
    }
    public override void F_TakeFurniture()
    {
        // 필터기는 회수없이 바로 파괴.

        F_OnFilterFurnitures(false);                                // 범위내 모든 설치물 필터 OFF
        // 다른 필터기와 함께 붙어있는 오브젝트는 0~10초 내 다시 켜질거임
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();

        Destroy(this.gameObject);                                   // 아이템 파괴
    }

    #endregion

    #region 저장/ 불러오기 관련
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
