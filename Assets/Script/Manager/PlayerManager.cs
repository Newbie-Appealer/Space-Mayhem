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

[System.Serializable]
public class PlayerData
{
    public float _oxygen;
    public float _water;
    public float _hunger;
    
    public PlayerData(int v_oxygen, int v_water, int v_hunger)
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

    [Header(" === Drag and Drop === ")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _playerCameraTransform;
    public Transform playerTransform { get { return _playerTransform; } }

    [Header(" === Spear Fire ===")]
    public bool _isSpearFire = false;
    
    // 프로퍼티
    public Player_Controller PlayerController { get => _playerController;  }

    protected override void InitManager()
    {
        // TODO:플레이어 데이터 로드 및 생성 (저장 시스템) 적용하기.
        _playerData = new PlayerData(100, 100, 100);

        _playerController = _playerTransform.GetComponent<Player_Controller>();
        _playerController.F_initController();
    }

    private void Update()
    {
        // 커서가 꺼져있을때만 움직일수있도록 하기
        if (!Cursor.visible)
            _playerController.playerController();
         // 1. 플레이어의 움직임 함수를 Player_Controller에 선언
         // 2. 함수를 델리게이트 체인에 묶어두고 델리게이트를 호출함.
         // 3. 플레이어의 상태마다 함수를 추가하고 제거하며, 플레이어의 움직임 제어
    }


    public void F_ChangeState(PlayerState v_state, int v_uniqueCode)
    {
        _playerState = v_state;
        _playerController.F_ChangeState(v_state,v_uniqueCode);
    }
}
