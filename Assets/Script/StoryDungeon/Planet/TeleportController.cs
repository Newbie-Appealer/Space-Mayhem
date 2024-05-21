using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    [SerializeField] PlanetManager planetManager;
    [SerializeField] bool _joinPlanet; //�༺���� �����ϸ� true
    public bool JoinPlanet => _joinPlanet;
    private Transform _playerPos;

    private void Start()
    {
        _playerPos = PlayerManager.Instance.playerTransform;
        _joinPlanet = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UIManager.Instance.F_IntercationPopup(true, "Press E Teleport"); // ��ȣ�ۿ� UI �ѱ�

            if (Input.GetKeyDown(KeyCode.E))
            {
                F_TeleportPlayer();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UIManager.Instance.F_IntercationPopup(false, ""); // ��ȣ�ۿ� UI ����
        }
    }

    public void F_TeleportPlayer()
    {
        if (_joinPlanet)
        {
            _playerPos.position = new Vector3(0, 1, 0);
            transform.position = new Vector3(0, 0.3f, 0);

            _joinPlanet = false; //onPlanet = ���ּ����� �̵�

            planetManager.F_DeletePlanet();
        }
        else if (!_joinPlanet)
        {
            planetManager.F_CreatePlanet();

            _playerPos.position = OutsideMapManager.Instance.playerTeleportPosition; //�÷��̾� ��ġ �̵�
            transform.position = OutsideMapManager.Instance.playerTeleportPosition; //��Ż ��ġ �̵�

            _joinPlanet = true; //!onPlanet = �༺���� �̵�
        }
    }
}
