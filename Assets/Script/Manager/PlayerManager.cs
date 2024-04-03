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
    
    // ������Ƽ
    public Player_Controller PlayerController { get => _playerController;  }

    protected override void InitManager()
    {
        // TODO:�÷��̾� ������ �ε� �� ���� (���� �ý���) �����ϱ�.
        _playerData = new PlayerData(100, 100, 100);

        _playerController = _playerTransform.GetComponent<Player_Controller>();
        _playerController.F_initController();
    }

    private void Update()
    {
        // Ŀ���� ������������ �����ϼ��ֵ��� �ϱ�
        if (!Cursor.visible)
            _playerController.playerController();
         // 1. �÷��̾��� ������ �Լ��� Player_Controller�� ����
         // 2. �Լ��� ��������Ʈ ü�ο� ����ΰ� ��������Ʈ�� ȣ����.
         // 3. �÷��̾��� ���¸��� �Լ��� �߰��ϰ� �����ϸ�, �÷��̾��� ������ ����
    }


    public void F_ChangeState(PlayerState v_state, int v_uniqueCode)
    {
        _playerState = v_state;
        _playerController.F_ChangeState(v_state,v_uniqueCode);
    }
}
