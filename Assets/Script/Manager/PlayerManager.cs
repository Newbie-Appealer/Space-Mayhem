using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float _oxygen;
    public float _water;
    public float _hunger;

    public void F_healing()
    {
        _oxygen = 100f;
        _water = 100f;
        _hunger = 100f;
    }
}

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("Player Data")]
    [SerializeField] private PlayerData _playerData;

    [Header("Drag and Drop")]
    [SerializeField] private Transform _playerTransform;
    public Transform playerTransform { get { return _playerTransform; } }

    protected override void InitManager()
    {
        // TODO:플레이어 데이터 로드 및 생성 (저장 시스템)
        _playerData = new PlayerData();
        _playerData.F_healing();            // 임시 
    }

    private void Update()
    {
        F_ReduceStat();
        UIManager.Instance.F_PlayerStatUIUpdate();
    }

    private void F_ReduceStat()
    {
        _playerData._oxygen -= Time.deltaTime;
        _playerData._water -= Time.deltaTime;
        _playerData._hunger -= Time.deltaTime;
    }

    public float F_GetStat(int v_index)
    {
        switch (v_index)
        {
            case 0:
                return _playerData._oxygen;
                break;
            case 1:
                return _playerData._water;
                break;
            case 2:
                return _playerData._hunger;
                break;
        }
        return 0;
    }
    public void F_HealWater()
    { 

    }

    public void F_HealOxygen()
    {

    }

    public void F_HealHunger()
    {

    }
}
