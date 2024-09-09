using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    [HideInInspector] public Vector3 _defalutPostion_teleport;
    [HideInInspector] public Vector3 _defalutPostion_player;

    [SerializeField] PlanetManager planetManager;
    private Transform _playerPos;

    private void Start()
    {
        // �ʱ�ȭ
        _playerPos = PlayerManager.Instance.playerTransform;
        _defalutPostion_teleport = new Vector3(0, 0.3f, 0);
        _defalutPostion_player = new Vector3(0, 1f, 0);
    }

    public IEnumerator F_TeleportPlayer()
    {
        // �÷��̾� Rigidbody Kinematic �Ӽ��� false(�����浹 OFF)
        PlayerManager.Instance.PlayerController.F_OnKinematic(true);
        UIManager.Instance.F_PlayerMessagePopupTEXT("�༺���� �̵���");

        // �ε� ON
        UIManager.Instance.F_OnLoading(true);
        yield return new WaitForSeconds(1f);

        // [ �༺ -> ���ּ� ]
        if (planetManager.joinPlanet)
        {
            // �༺ ������¸� false�� ��ȯ
            planetManager.joinPlanet = false;

            // 1. �÷��̾� �̵� �� ��Ȱ
            _playerPos.position = _defalutPostion_player;
            PlayerManager.Instance.PlayerController._isPlayerDead = false;
            yield return new WaitForSeconds(0.5f);

            // 2. ��Ż �̵�
            planetManager.teleport.position = _defalutPostion_teleport;
            yield return new WaitForSeconds(0.5f);

            // 3. ��/������Ʈ/���� ����
            OutsideMapManager.Instance.F_ExitOutsideMap();          //�ܺθ� ����
            yield return new WaitForSeconds(0.5f);
            InsideMapManager.Instance.F_DestroyInsideMap();         //���θ� ����
            yield return new WaitForSeconds(0.5f);
            EnemyManager.Instance.F_RemoveEnemy();                  //���� ����
            yield return new WaitForSeconds(0.25f);
            ItemManager.Instance.dropItemSystem.F_RemoveObjects();  //�����ۿ�����Ʈ ����
            yield return new WaitForSeconds(0.25f);

            // 4. �༺ ������Ʈ �ı�
            planetManager.F_DeletePlanet();
            yield return new WaitForSeconds(0.5f);

            // 5. ���� ���� ( �༺ -> ���ּ� ) ���ּ� BGM �����̼� ���
            SoundManager.Instance.F_StartSpaceShipBGM();
        }

        // [ ���ּ� -> �༺ ]
        else if (!planetManager.joinPlanet)
        {
            // �༺ ������¸� true ��ȯ
            planetManager.joinPlanet = true;

            // 1. �༺ ����
            OutsideMapManager.Instance.F_CreateOutsideMap();    //�ܺθ� ����
            yield return new WaitForSeconds(0.5f);
            InsideMapManager.Instance.F_GenerateInsideMap();    //���θ� ����
            yield return new WaitForSeconds(0.5f);


            // 2. �÷��̾� �̵�
            _playerPos.position = OutsideMapManager.Instance.playerTeleportPosition;
            yield return new WaitForSeconds(0.5f);

            // 3. ��Ż �̵�
            planetManager.teleport.position = OutsideMapManager.Instance.playerTeleportPosition;
            yield return new WaitForSeconds(0.5f);

            // 4. ���� ���� ( ���ּ� -> �༺ ) �ܺθ� BGM �����̼� ���
            SoundManager.Instance.F_StartOUTSideBGM();
        }

        // �ε� OFF �� �÷��̾� Rigidbody Kinematic �Ӽ��� false ( �����浹 ON )
        yield return new WaitForSeconds(0.5f);
        PlayerManager.Instance.PlayerController.F_OnKinematic(false);
        UIManager.Instance.F_OnLoading(false);
    }
}
