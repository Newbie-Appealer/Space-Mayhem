using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaterTankLevel
{
    SIMPLE,
}

public enum TankType
{
    OXYGEN,
    WATER
}

public class TankWrapper
{
    public int _tankAmount;

    public TankWrapper(int tankAmount)
    {
        _tankAmount = tankAmount;
    }
}

public abstract class Tanks : Furniture
{
    protected Action _tankButtonEvent;

    [Header("=== Tank Information ===")]
    [SerializeField] protected int _tankAmount;         // ���� ��ġ
    [SerializeField] protected int _tankMaxAmount;      // �ִ� ��ġ
    [SerializeField] protected int _chargingSpeed;      // ���� �ӵ� ( second )
    [SerializeField] protected TankType _tankType;      // ��ũ Ÿ�� ( ��� / �� )
    [SerializeField] protected LayerMask _filterLayer;  // ���� ���� ���̾�)
    protected bool _canClickButton;
    public float gaugeAmount => (float)_tankAmount / (float)_tankMaxAmount;
    public string gaugeText => _tankAmount + " / " + _tankMaxAmount;
    public TankType tankType => _tankType;
    protected override void F_InitFurniture()
    {
        F_InitTankData();
    }

    protected abstract void F_InitTankData();

    #region ��ȣ�ۿ�
    public override void F_Interaction()
    {
        PlayerManager.Instance.PlayerController.F_PickupMotion();
        SoundManager.Instance.F_PlaySFX(SFXClip.OPEN);

        // ������ / �κ��丮 UI Ȱ��ȭ
        UIManager.Instance.OnInventoryUI();                                         // �κ��丮 UI Ȱ��ȭ
        ItemManager.Instance.produceSystem._Tank_Selected = this;                   // ������ ��ũ

        UIManager.Instance.F_OnTankUI(_tankType, _onEnergy, _onFilter, true);       // Tank UI Ȱ��ȭ �� �ʱ�ȭ
        UIManager.Instance.F_UpdateTankGauge(gaugeAmount, gaugeText);               // Tank UI ������ ������Ʈ
        UIManager.Instance.F_BindingTankUIEvent(_tankButtonEvent);                  // Tank UI ��ư �̺�Ʈ ���ε�
    }
    #endregion

    #region ����/���ͱ� ���� on/off
    public override void F_ChangeFilterState(bool v_state)
    {
        _onFilter = v_state;
    }

    public override void F_ChangeEnergyState(bool v_state)
    {
        _onEnergy = v_state;
    }
    #endregion

    #region ���� / �ҷ����� ����
    public override string F_GetData()
    {
        TankWrapper tankData = new TankWrapper(_tankAmount);
        string jsonData = JsonUtility.ToJson(tankData);
        string base64Data = GameManager.Instance.F_EncodeBase64(jsonData);
        return base64Data;
    }

    public override void F_SetData(string v_data)
    {
        string dataString = GameManager.Instance.F_DecodeBase64(v_data);

        TankWrapper data = JsonUtility.FromJson<TankWrapper>(dataString);

        _tankAmount = data._tankAmount;
    }
    #endregion
}
