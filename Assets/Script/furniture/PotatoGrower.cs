using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Growth
{
    NEW,        // ���� �ɾ���
    SPROUT,     // ���� 
    GROWING,    // �ڶ����
    END,        // �� �ڶ�
}

public class PotatoGrower : Furniture
{
    string _tag_unInteraction = "unInteractionObject";
    string _tag_Interaction = "InteractionObject";
    int _potatoItemCode = 21;

    [Header("=== �ν�����â���� �־��ٰ� ===")]
    [SerializeField] private Transform[] _potatoPlants;       // ���� ���� Ǯ����
    [SerializeField] private Vector3[] _pototaGrowScale;    // ���� ���� ũ�� �迭 [ 0, 0.3, 0.6, 1 ]

    [Header("=== PotatoGrower Information ===")]
    [SerializeField] private Growth _stageOfGrowth;                     // ���� �ܰ�
    [SerializeField] private int _needGrowTime;                         // ���� �ð�
    [SerializeField] private int _growTime;
    protected override void F_InitFurniture()
    {
        F_InitGrower();
    }

    private void F_GrowthPlants()
    {
        foreach (Transform plant in _potatoPlants)
        {
            plant.localScale = _pototaGrowScale[(int)_stageOfGrowth];
        }
    }

    IEnumerator C_Growing()
    {
        yield return new WaitForSeconds(0.2f);
        while(_stageOfGrowth != Growth.END)
        {
            yield return new WaitForSeconds(1f);
            _growTime++;

            // ���忡 �ʿ��� �ð��� �� ä������ -> ���� �ܰ�� ����
            if(_needGrowTime <= _growTime)
            {
                _stageOfGrowth++;   // ���� �ܰ��
                F_GrowthPlants();   // ���� ũ�� ����
                _growTime = 0;      // �ð� �ʱ�ȭ
            }
        }
        gameObject.tag = _tag_Interaction;
    }

    private void F_InitGrower()
    {
        gameObject.tag = _tag_unInteraction;        // ��ȣ�ۿ� ����

        _stageOfGrowth = Growth.NEW;                // ���� �ܰ� �ʱ�ȭ
        F_GrowthPlants();                           // ���� ũ�� �ʱ�ȭ
        StartCoroutine(C_Growing());                // ���� ���� �ڷ�ƾ ����
    }

    #region ��ȣ�ۿ� ( ��Ȯ )
    public override void F_Interaction()
    {
        if (_stageOfGrowth != Growth.END)
            return;

        if (ItemManager.Instance.inventorySystem.F_GetItem(_potatoItemCode))    // ������ ȹ�� �õ�
        {
            ItemManager.Instance.inventorySystem.F_InventoryUIUpdate(); // �κ�Ʈ�� ������Ʈ

            F_InitGrower();                                     // ���� �ʱ�ȭ
            UIManager.Instance.F_IntercationPopup(false, "");   // ��ȣ�ۿ� UI ������
        }
    }
    #endregion

    #region ���� / �ҷ����� 
    public override string F_GetData()
    {
        string jsonData = "NONE";
        return jsonData;
    }

    public override void F_SetData(string v_data)
    {

    }
    #endregion
}
