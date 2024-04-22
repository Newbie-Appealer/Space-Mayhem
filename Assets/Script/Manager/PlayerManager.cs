using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum PlayerState
{
    NONE,
    FARMING,
    BUILDING,
    INSTALL
}

public enum PlayerStatType
{
    OXYGEN,
    WATER,
    HUNGER
}

[System.Serializable]
public class PlayerData
{
    public float _oxygen;       // 100 -> 0 ( 20�� )
    public float _water;        // 100 -> 0 ( 12�� )
    public float _hunger;       // 100 -> 0 ( 16�� )
    
    public PlayerData(float v_oxygen, float v_water, float v_hunger)
    {
        _oxygen = v_oxygen;
        _water = v_water;
        _hunger = v_hunger;
    }
    public void F_healing()
    {
        _oxygen = 100f;
        _water = 100f;
        _hunger = 100f;
    }
}

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("== Player State ==")]
    [SerializeField] PlayerState _playerState;
    public PlayerState playerState => _playerState;


    [Header(" === Player Data === ")]
    [SerializeField] private PlayerData _playerData;
    private Player_Controller _playerController;

    [Header(" === Data balance( �� �־���! ) ===")]
    [SerializeField] private float _dataMaxumum;                // ���/��/��� �ִ��ġ
    [SerializeField] private int _datadecreaseCount;            // 100 > 0 ���� �����ϴ� Ƚ�� ( ex) 1000 -> 0.1�� ����
    [SerializeField] private int _oxygenDecreaseSecond;         // ��� 100 > 0 ���� ���ұ��� �ɸ��� �ð�
    [SerializeField] private int _waterDecreaseSecond;          // ��� 100 > 0 ���� ���ұ��� �ɸ��� �ð�
    [SerializeField] private int _hungerDecreaseSecond;         // ��� 100 > 0 ���� ���ұ��� �ɸ��� �ð�
    private float _amount;                                      // ƽ�� ���� ��ġ

    [Header(" === Drag and Drop === ")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _playerCameraTransform;
    public Transform playerTransform { get { return _playerTransform; } }

    [Header(" === Can Shoot Pistol ===")]
    public bool _canShootPistol = false;
    
    // ������Ƽ
    public Player_Controller PlayerController { get => _playerController;  }

    
    // �ڷ�ƾ
    private IEnumerator _decreaseOxygen;
    private IEnumerator _decreaseWater;
    private IEnumerator _decreaseHunger;
    protected override void InitManager()
    {
        // TODO:�÷��̾� ������ �ε� �� ���� (���� �ý���) �����ϱ�.
        _playerData = new PlayerData(_dataMaxumum, _dataMaxumum, _dataMaxumum);
        _amount = _dataMaxumum / _datadecreaseCount;

        _decreaseOxygen = C_DecreaseOxygen();
        _decreaseWater = C_DecreaseWater();
        _decreaseHunger = C_DecreaseHunger();

        StartCoroutine(_decreaseOxygen);
        StartCoroutine(_decreaseWater);
        StartCoroutine(_decreaseHunger);

        // PlayerController �ʱ�ȭ
        _playerController = _playerTransform.GetComponent<Player_Controller>();
        _playerController.F_initController();
    }

    private void Update()
    {
        // Ŀ���� ������������ �����ϼ��ֵ��� �ϱ�
        // 1. �÷��̾��� ������ �Լ��� Player_Controller�� ����
        // 2. �Լ��� ��������Ʈ ü�ο� ����ΰ� ��������Ʈ�� ȣ����.
        // 3. �÷��̾��� ���¸��� �Լ��� �߰��ϰ� �����ϸ�, �÷��̾��� ������ ����
        if (!Cursor.visible)
            _playerController.playerController();
    }


    #region ���, ��, ��� ������ ���� �ڷ�ƾ
    IEnumerator C_DecreaseOxygen()
    {
        float tick = (float)_oxygenDecreaseSecond / (float)_datadecreaseCount;
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if(_playerData._oxygen >= 0.01f)
                _playerData._oxygen -= _amount;

            UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.OXYGEN);
            yield return new WaitForSeconds(tick);
        }
    }
    IEnumerator C_DecreaseWater()
    {
        float tick = (float)_waterDecreaseSecond / (float)_datadecreaseCount;
        yield return new WaitForSeconds(2f);
        while(true)
        {
            if(_playerData._water >= 0.01f)
                _playerData._water -= _amount;

            UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.WATER);
            yield return new WaitForSeconds(tick);
        }
    }
    IEnumerator C_DecreaseHunger()
    {
        float tick = (float)_hungerDecreaseSecond / (float)_datadecreaseCount;
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (_playerData._hunger >= 0.01f)
                _playerData._hunger -= _amount;

            UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.HUNGER);
            yield return new WaitForSeconds(tick);
        }
    }
    #endregion

    public void F_ChangeState(PlayerState v_state, int v_uniqueCode)
    {
        _playerState = v_state;
        _playerController.F_ChangeState(v_state,v_uniqueCode);
    }

    public float F_GetStat(int v_num)
    {
        if (v_num == 0)
            return _playerData._oxygen;
        if (v_num == 1)
            return _playerData._water;
        if (v_num == 2)
            return _playerData._hunger;

        return 0;
    }
}
