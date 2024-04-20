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
    private List<float> _energyRanges;          // ���� ������
    private float _generatorRange;              // �ش� ������Ʈ�� ������ ����

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
        // 1. ���� 1ȸ ���� ��ġ���� ���� �ֱ�
        yield return new WaitForSeconds(0.5f);
        F_OnEnergyFurnitures();
        while(true)
        {
            // 2. 15�ʿ� �ѹ� ���� ��ġ���� ���� �ֱ�
            yield return new WaitForSeconds(15f);
            F_OnEnergyFurnitures();
        }
    }

    /// <summary> �����ȿ� �ִ� ��� ��ġ���� ���� �ֱ� </summary>
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

    #region ������ ���� / �ҷ����� ( �¾翭������� ������ )
    public override string F_GetData()
    {
        // �¾翭 ������� ������ �����Ұ� ����!
        string jsonData = "NONE";
        return jsonData;
    }

    public override void F_SetData(string v_data) 
    { 
        // �¾翭 ������� ������ �����Ұ� ����!
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _generatorRange);
    }

        //#TODO:tmp_FX_LightRay
        // ��ȣ�ۿ� : ���� �����ֱ� ( n �ʰ� )
        // ��ġ�ҋ� : ���� �����ֱ� ( �����信 �ٿ��α� ? )
        // ������ ������ ���缭 ��� ����???? ????? 
}
