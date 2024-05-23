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
        UIManager.Instance.F_PlayerMessagePopupTEXT("Teleport to Planet");

        // �ε� ON
        UIManager.Instance.F_OnLoading(true);
        yield return new WaitForSeconds(1f);

        // [ �༺ -> ���ּ� ]
        if (planetManager.joinPlanet)
        {
            // �༺ ������¸� false�� ��ȯ
            planetManager.joinPlanet = false;

            // 1. �÷��̾� �̵� ( ������ ��ġ�� �������� )
            F_Teleport(_playerPos, _defalutPostion_player);
            yield return new WaitForSeconds(0.5f);

            // 2. ��Ż �̵�
            planetManager.F_OnTeleport(true, _defalutPostion_teleport);
            yield return new WaitForSeconds(0.5f);

            // 3. �� ����
            OutsideMapManager.Instance.F_ExitOutsideMap();      //�ܺθ� ����
            yield return new WaitForSeconds(0.5f);
            InsideMapManager.Instance.F_DestroyInsideMap();     //���θ� ����
            yield return new WaitForSeconds(0.5f);

            // 4. �༺ ������Ʈ �ı�
            planetManager.F_DeletePlanet();
            yield return new WaitForSeconds(0.5f);
        }

        // [ ���ּ� -> �༺ ]
        else if (!planetManager.joinPlanet)
        {
            // �༺ ������¸� true ��ȯ
            planetManager.joinPlanet = true;

            // 1. �༺ ����
            OutsideMapManager.Instance.F_CreateOutsideMap();    //�ܺθ� ����
            yield return new WaitForSeconds(0.5f);
            InsideMapManager.Instance.F_GenerateMaze();         //���θ� ����
            yield return new WaitForSeconds(0.5f);


            // 2. �÷��̾� �̵�
            F_Teleport(_playerPos, OutsideMapManager.Instance.playerTeleportPosition);
            yield return new WaitForSeconds(0.5f);

            // 3. ��Ż �̵�
            planetManager.F_OnTeleport(true, OutsideMapManager.Instance.playerTeleportPosition);
            yield return new WaitForSeconds(0.5f);
        }

        // �ε� OFF
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.F_OnLoading(false);
    }

    private void F_Teleport(Transform v_targetObject, Vector3 v_targetPosition)
    {
        // Ÿ���� ��ǥ�������� �̵���Ŵ.
        v_targetObject.position = v_targetPosition;

        // Ÿ�� ������Ʈ, Ÿ����ġ�� �Ÿ��� 5 ���ϰ� �ɋ����� ( ������ ��ġ ��ó�� ���������� )
        while(Vector3.Distance(v_targetObject.position, v_targetPosition) >= 5f)
            v_targetObject.position = v_targetPosition;
        // �÷��̾� ������Ʈ�� �̵������ʴ� ���׸� ����..
    }
}
