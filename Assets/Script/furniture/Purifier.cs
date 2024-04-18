using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PurifierLEVEL
{
    NONE,           // 0
    SIMPLE,         // 1
}

public enum PurifierState
{
    DONOTHING,      // ���� ��
    INPROGRESS,     // ���� ��
    END             // ���� ��
}

public class Purifier : Furniture
{
    [SerializeField] private ProduceSystem _produceSystem;

    [Header("=== Purifier LEVEL ===")]
    [SerializeField] private PurifierLEVEL _purifierLevel;  // �������� ���� ( ����, ~~, ~~ )
    [SerializeField] private int _defaultTime;              // ������ ���꿡 �ʿ��� �ð�

    [Header("=== Purifier States ===")]
    [SerializeField] private PurifierState _purifierState;  // ������ ���� ���� ( ���� ��,��,�� )
    [SerializeField] private int _resultItemCode;           // �������� ������ ��ȣ
    [SerializeField] private int _leftTime;                 // ���� �Ϸ���� ���� �ð� ( �⺻�� -1 )
    [SerializeField] private bool _onEnergy;                // ���� ���� ����

    #region get
    public PurifierState purifierState => _purifierState;
    public PurifierLEVEL purifierLevel => _purifierLevel;
    public bool onEnergy => _onEnergy;
    public int resultItemCode => _resultItemCode;
    public int leftTime => _leftTime;   
    #endregion

    public void F_StartingProgress(int v_resultCode)
    {
        _purifierState = PurifierState.INPROGRESS;
        _resultItemCode = v_resultCode;
        _leftTime = _defaultTime;
        StartCoroutine(C_ProductionTimer());
    }

    IEnumerator C_ProductionTimer()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            if (!onEnergy)                              // ���� ���� ������
                continue;

            _leftTime--;
            if (_leftTime <= 0)                         // �����ð� ������
            {
                _purifierState = PurifierState.END;     // ���� = ���� ��
                break;                                  // �ڷ�ƾ Ż��
            }
        }
            
    }

    /// <summary> ���� �Ϸ� �� ������ ȹ�� ���� ���� �ʱ�ȭ </summary>
    public void F_InitPurifierData()
    {
        _purifierState = PurifierState.DONOTHING;
        _resultItemCode = 0;
        _leftTime = 0;
    }

    #region �ʱ�ȭ �Լ� 
    protected override void F_InitFurniture()
    {
        _produceSystem = ItemManager.Instance.produceSystem;
        _defaultTime = 10;
    }
    #endregion

    #region ��ȣ�ۿ� �Լ�
    public override void F_Interaction()
    {
        // ������ / �κ��丮 UI Ȱ��ȭ
        UIManager.Instance.OnInventoryUI();         // �κ��丮 UI Ȱ��ȭ
        UIManager.Instance.F_OnPurifierUI(true);    // ������ UI Ȱ��ȭ

        _produceSystem._purifier_Selected = this;   // ���õ� ������ ������Ʈ
        _produceSystem.F_UpdateSlotUI();            // ������ UI ������Ʈ ( ��� 
        _produceSystem.F_UpdatePurifierUI();        // ������ UI ������Ʈ ( �ϴ� 
    }
    #endregion

    #region ����/�ҷ����� ���� �Լ�
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
