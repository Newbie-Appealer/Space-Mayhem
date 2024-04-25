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
        F_OnEnergyFurnitures(true);
        while(true)
        {
            // 2. 10�ʿ� �ѹ� ���� ��ġ���� ���� �ֱ�
            yield return new WaitForSeconds(10f);
            F_OnEnergyFurnitures(true);
        }
    }

    /// <summary> �����ȿ� �ִ� ��� ��ġ���� ���� ���� </summary>
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

    #region ��ȣ�ۿ� �Լ�
    public override void F_Interaction()
    {
        _rangeObject.SetActive(!_rangeObject.activeSelf);
    }

    public override void F_TakeFurniture()
    {
        if (ItemManager.Instance.inventorySystem.F_GetItem(_itemCode))   // �κ��丮�� ������ �߰� �õ�
        {
            F_OnEnergyFurnitures(false);                                // ������ ��� ��ġ�� ���� OFF
            // �ٸ� ������� �Բ� �پ��ִ� ������Ʈ�� 0~10�� �� �ٽ� ��������
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();

            Destroy(this.gameObject);                                   // ������ ȹ�� ����
        }
    }
    #endregion

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
}
