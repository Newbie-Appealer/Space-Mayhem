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

public class PurifierWrapper
{
    public int _purifierState;
    public int _resultItemCode;
    public int _leftTime;

    public PurifierWrapper(int v_purifierState, int v_resultItemCode, int v_leftTime)
    {
        _purifierState = v_purifierState;
        _resultItemCode = v_resultItemCode;
        _leftTime = v_leftTime;
    }
}

public class Purifier : Furniture
{
    [Space]
    [Header("=== SYSTEM ===")]
    [SerializeField] private ProduceSystem _produceSystem;

    [Header("=== Purifier LEVEL ===")]
    [SerializeField] private PurifierLEVEL _purifierLevel;  // �������� ���� ( ����, ~~, ~~ )
    [SerializeField] private int _defaultTime;              // ������ ���꿡 �ʿ��� �ð�

    [Header("=== Purifier States ===")]
    [SerializeField] private PurifierState _purifierState;  // ������ ���� ���� ( ���� ��,��,�� )
    [SerializeField] private int _resultItemCode;           // �������� ������ ��ȣ
    [SerializeField] private int _leftTime;                 // ���� �Ϸ���� ���� �ð� ( �⺻�� -1 )

    #region get
    public PurifierState purifierState => _purifierState;
    public PurifierLEVEL purifierLevel => _purifierLevel;
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

    public override void F_ChangeEnergyState(bool v_state)
    {
        _onEnergy = v_state;
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
        SoundManager.Instance.F_PlaySFX(SFXClip.OPEN);              // ���� ����
        PlayerManager.Instance.PlayerController.F_PickupMotion();   // �ִϸ��̼� ���

        // ������ / �κ��丮 UI Ȱ��ȭ
        UIManager.Instance.OnInventoryUI();         // �κ��丮 UI Ȱ��ȭ
        UIManager.Instance.F_OnPurifierUI(true);    // ������ UI Ȱ��ȭ

        _produceSystem._purifier_Selected = this;   // ���õ� ������ ������Ʈ
        _produceSystem.F_UpdateSlotUI();            // ������ UI ������Ʈ ( ��� 
        _produceSystem.F_UpdatePurifierUI();        // ������ UI ������Ʈ ( �ϴ� 
    }

    public override void F_TakeFurniture()
    {
        // �����ϰ������������� ȸ�� ����
        if(_purifierState == PurifierState.DONOTHING)
        {
            if (ItemManager.Instance.inventorySystem.F_GetItem(_itemCode))   // �κ��丮�� ������ �߰� �õ�
            {
                SoundManager.Instance.F_PlaySFX(SFXClip.USEHAND);           // ȸ�� ���� ���
                PlayerManager.Instance.PlayerController.F_PickupMotion();   // ȸ�� �ִϸ��̼� ���

                ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();

                Destroy(this.gameObject);                                   // ������ ȹ�� ����
            }
        }
    }
    #endregion

    #region ����/�ҷ����� ���� �Լ�
    public override string F_GetData()
    {
        PurifierWrapper purifierData = new PurifierWrapper((int)_purifierState, _resultItemCode, _leftTime);
        string jsonData = JsonUtility.ToJson(purifierData);
        string Base64Data = GameManager.Instance.F_EncodeBase64(jsonData);
        return Base64Data;
    }

    public override void F_SetData(string v_data)
    {
        string dataString = GameManager.Instance.F_DecodeBase64(v_data);

        PurifierWrapper data = JsonUtility.FromJson<PurifierWrapper>(dataString);

        _purifierState = (PurifierState)data._purifierState;
        _resultItemCode = data._resultItemCode;
        _leftTime = data._leftTime;

        if (_purifierState == PurifierState.INPROGRESS)
            StartCoroutine(C_ProductionTimer());
    }
    #endregion
}
