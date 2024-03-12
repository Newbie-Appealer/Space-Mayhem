using System.Collections;
using System.Collections.Generic;
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
        // TODO:�÷��̾� ������ �ε� �� ���� (���� �ý���)
        _playerData = new PlayerData();
        _playerData.F_healing();            // �ӽ� 
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
