using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SolarGeneratorLEVEL
{
    SIMPLE,

}

public class SolarGenerator : Furniture
{
    private List<float> _energyRanges;          // 범위 데이터
    private float _generatorRange;              // 해당 오브젝트의 발전기 범위

    [Header("=== Solar Generator LEVEL ===")]
    [SerializeField] private SolarGeneratorLEVEL _GeneratorLevel;

    [Header("=== Solar Generator Information ===")]
    [SerializeField] LayerMask _layerMask;

    private Furniture _tmpFurniture;

    [Header("Check range")]
    [SerializeField] private GameObject _rangeObject;

    protected override void F_InitFurniture()
    {
        _energyRanges = new List<float>();
        _energyRanges.Add(8.5f);

        _generatorRange = _energyRanges[(int)_GeneratorLevel];

        _rangeObject.SetActive(false);

        StartCoroutine(C_CheckFurnitures());
    }

    IEnumerator C_CheckFurnitures()
    {
        // 1. 최초 1회 주위 설치물에 전원 넣기
        yield return new WaitForSeconds(0.5f);
        F_OnEnergyFurnitures();
        while(true)
        {
            // 2. 15초에 한번 주위 설치물에 전원 넣기
            yield return new WaitForSeconds(15f);
            F_OnEnergyFurnitures();
        }
    }

    /// <summary> 범위안에 있는 모든 설치물의 전원 넣기 </summary>
    private void F_OnEnergyFurnitures()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _generatorRange, _layerMask);
        
        foreach(Collider collider in colliders) 
        {
            try
            {
                _tmpFurniture = collider.transform.parent.GetComponent<Furniture>();

                if (_tmpFurniture == null)
                    continue;

                _tmpFurniture.F_ChangeEnergyState(true);
            }
            catch(Exception ex)
            {
                Debug.Log(ex + "Null Compnent ");
            }
        }
    }

    public override void F_Interaction()
    {
        _rangeObject.SetActive(!_rangeObject.activeSelf);
    }

    #region 데이터 저장 / 불러오기 ( 태양열발전기는 사용안함 )
    public override string F_GetData()
    {
        // 태양열 발전기는 데이터 저장할거 없음!
        string jsonData = "NONE";
        return jsonData;
    }

    public override void F_SetData(string v_data) 
    { 
        // 태양열 발전기는 데이터 저장할거 없음!
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _generatorRange);
    }

        //#TODO:tmp_FX_LightRay
        // 상호작용 : 범위 보여주기 ( n 초간 )
        // 설치할떄 : 범위 보여주기 ( 프리뷰에 붙여두기 ? )
        // 발전기 범위에 맞춰서 어떻게 하지???? ????? 
}
