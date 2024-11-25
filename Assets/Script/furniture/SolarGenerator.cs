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
        F_OnEnergyFurnitures(true);
        while(true)
        {
            // 2. 10초에 한번 주위 설치물에 전원 넣기
            yield return new WaitForSeconds(10f);
            F_OnEnergyFurnitures(true);
        }
    }

    /// <summary> 범위안에 있는 모든 설치물의 전원 설정 </summary>
    private void F_OnEnergyFurnitures(bool v_bValue)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _generatorRange, _layerMask);
        
        foreach(Collider collider in colliders) 
        {
            try
            {
                _tmpFurniture = collider.transform.parent.GetComponent<Furniture>();

                if (_tmpFurniture == null)
                    continue;

                _tmpFurniture.F_ChangeEnergyState(v_bValue);
            }
            catch(Exception ex)
            {
                Debug.Log(ex + "Null Compnent ");
            }
        }
    }

    #region 상호작용 함수
    public override void F_Interaction()
    {
        SoundManager.Instance.F_PlaySFX(SFXClip.OPEN);              // 오픈 사운드
        PlayerManager.Instance.PlayerController.F_PickupMotion();   // 애니메이션 재생

        _rangeObject.SetActive(!_rangeObject.activeSelf);
    }

    public override void F_TakeFurniture()
    {
        if (ItemManager.Instance.inventorySystem.F_GetItem(_itemCode))   // 인벤토리에 아이템 추가 시도
        {
            SoundManager.Instance.F_PlaySFX(SFXClip.USEHAND);           // 회수 사운드
            PlayerManager.Instance.PlayerController.F_PickupMotion();   // 회수 애니메이션

            F_OnEnergyFurnitures(false);                                // 범위내 모든 설치물 전원 OFF
            // 다른 발전기와 함께 붙어있는 오브젝트는 0~10초 내 다시 켜질거임
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();

            Destroy(this.gameObject);                                   // 아이템 획득 성공
        }
    }
    #endregion

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
}
