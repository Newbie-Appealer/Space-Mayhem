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
    public float _oxygen;       // 100 -> 0 ( 20분 )
    public float _water;        // 100 -> 0 ( 12분 )
    public float _hunger;       // 100 -> 0 ( 16분 )
    
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

    [Header(" === Data balance( 값 넣어줘! ) ===")]
    [SerializeField] private float _dataMaximum;                // 산소/물/허기 최대수치
    [SerializeField] private int _datadecreaseCount;            // 100 > 0 으로 감소하는 횟수 ( ex) 1000 -> 0.1씩 감소
    [SerializeField] private int _oxygenDecreaseSecond;         // 산소 100 > 0 으로 감소까지 걸리는 시간
    [SerializeField] private int _waterDecreaseSecond;          // 산소 100 > 0 으로 감소까지 걸리는 시간
    [SerializeField] private int _hungerDecreaseSecond;         // 산소 100 > 0 으로 감소까지 걸리는 시간
    private float _amount;                                      // 틱당 감소 수치

    [Header(" === Drag and Drop === ")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _playerCameraTransform;
    public Transform playerTransform { get { return _playerTransform; } }

    [Header(" === Can Shoot Pistol ===")]
    public bool _canShootPistol = false;
    
    // 프로퍼티
    public Player_Controller PlayerController { get => _playerController;  }

    
    // 코루틴
    private IEnumerator _decreaseOxygen;
    private IEnumerator _decreaseWater;
    private IEnumerator _decreaseHunger;

    //회복 관련 bool
    private bool _canChangeOxygenUI = false;
    private bool _canChangeWaterUI = false;
    private bool _canChangeHungerUI = false;
    protected override void InitManager()
    {
        // PlayerController 초기화
        _playerController = _playerTransform.GetComponent<Player_Controller>();
        _playerController.F_initController();

        // 저장 델리게이트 등록
        SaveManager.Instance.GameDataSave += () => SaveManager.Instance.F_SavePlayerData(_playerData);

        // 1. 데이터 불러오기
        SaveManager.Instance.F_LoadPlayerData(ref _playerData);

        // 틱당 감소 수치 초기화
        _amount = _dataMaximum / _datadecreaseCount;
        
        // 코루틴
        _decreaseOxygen = C_DecreaseOxygen();
        _decreaseWater = C_DecreaseWater();
        _decreaseHunger = C_DecreaseHunger();
        StartCoroutine(_decreaseOxygen);
        StartCoroutine(_decreaseWater);
        StartCoroutine(_decreaseHunger);

    }

    private void Update()
    {
        // 커서가 꺼져있을때만 움직일수있도록 하기 / 로딩이 켜져있는동안에 움직임 방지
        // 1. 플레이어의 움직임 함수를 Player_Controller에 선언
        // 2. 함수를 델리게이트 체인에 묶어두고 델리게이트를 호출함.
        // 3. 플레이어의 상태마다 함수를 추가하고 제거하며, 플레이어의 움직임 제어
        if (!Cursor.visible && !UIManager.Instance.onLoading)
            _playerController.playerController();
    }


    #region 산소, 물, 허기 게이지 감소 코루틴
    IEnumerator C_DecreaseOxygen()
    {
        UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.OXYGEN);

        float tick = (float)_oxygenDecreaseSecond / (float)_datadecreaseCount;
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if(_playerData._oxygen >= 0.01f)
            {
                _playerData._oxygen -= _amount;
                if (_playerData._oxygen <= 25f && !_canChangeOxygenUI)
                {
                    UIManager.Instance.F_PlayerStatUIDanger(PlayerStatType.OXYGEN);
                    _canChangeOxygenUI = true;
                }
            }
            else
            {
                // 사망
            }

            UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.OXYGEN);
            yield return new WaitForSeconds(tick);
        }
    }
    IEnumerator C_DecreaseWater()
    {
        UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.WATER);

        float tick = (float)_waterDecreaseSecond / (float)_datadecreaseCount;
        yield return new WaitForSeconds(2f);
        while(true)
        {
            if (_playerData._water >= 0.01f)
            {
                _playerData._water -= _amount;
                if (_playerData._water <= 25f  && !_canChangeWaterUI)
                {
                    UIManager.Instance.F_PlayerStatUIDanger(PlayerStatType.WATER);
                    _canChangeWaterUI = true;
                }
            }
            else
                _playerData._oxygen -= _amount / 2;     // 물 감소수치의 절반만큼 산소 감소

            UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.WATER);
            yield return new WaitForSeconds(tick);
        }
    }
    IEnumerator C_DecreaseHunger()
    {
        UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.HUNGER);

        float tick = (float)_hungerDecreaseSecond / (float)_datadecreaseCount;
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (_playerData._hunger >= 0.01f)
            {
                _playerData._hunger -= _amount;
                if (_playerData._hunger <= 25f && !_canChangeHungerUI)
                {
                    UIManager.Instance.F_PlayerStatUIDanger(PlayerStatType.HUNGER);
                    _canChangeHungerUI = true;
                }
            }
            
            else
                _playerData._oxygen -= _amount / 2;     // 허기 감소수치의 절반만큼 산소 감소

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

    #region 산소, 물, 허기 게이지 회복 함수
    public void F_HealState(HealType _healType, float v_healValue) 
    { 
        switch(_healType)
        {
            case HealType.HUNGER:
                F_HealHunger(v_healValue);
                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.HUNGER);
                break;
            case HealType.WATER:
                F_HealWater(v_healValue);
                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.WATER);
                break;
            case HealType.OXYGEN:
                F_HealOxygen(v_healValue);
                UIManager.Instance.F_PlayerStatUIUpdate(PlayerStatType.OXYGEN);
                break;
        }
    }

    public void F_HealHunger(float v_healValue)
    {
        float tmpHunger = _playerData._hunger + v_healValue;

        if (tmpHunger > 25f && _canChangeHungerUI)
        {
            UIManager.Instance.F_PlayerStatUIRecover(PlayerStatType.HUNGER);
            _canChangeHungerUI = false;
        }

        if (tmpHunger >= 99.9f)
        {
            UIManager.Instance.F_PlayerMessagePopupTEXT("Hunger is Full");
            _playerData._hunger = 99.9f;
        }
        else
            _playerData._hunger = tmpHunger;
    }

    public void F_HealWater(float v_healValue)
    {
        _playerData._water += v_healValue;

        if(_playerData._water > 25f && _canChangeWaterUI)
        {
            UIManager.Instance.F_PlayerStatUIRecover(PlayerStatType.WATER);
            _canChangeWaterUI = false;
        }

        if (_playerData._water > 99.9f)
        {
            UIManager.Instance.F_PlayerMessagePopupTEXT("Water is Full");
            _playerData._water = 99.9f;
        }
    }

    public void F_HealOxygen(float v_healValue)
    {
        _playerData._oxygen += v_healValue;

        if (_playerData._oxygen > 25f && _canChangeOxygenUI)
        {
            UIManager.Instance.F_PlayerStatUIRecover(PlayerStatType.OXYGEN);
            _canChangeOxygenUI = false;
        }

        if (_playerData._oxygen > 99.9f)
        {
            UIManager.Instance.F_PlayerMessagePopupTEXT("Oxygen is Full");
            _playerData._oxygen = 99.9f;
        }
    }
    #endregion
}
