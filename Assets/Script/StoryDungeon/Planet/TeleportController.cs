using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    [SerializeField] PlanetManager planetManager;
    private bool _isTeleporting; //�༺���� �����ϸ� true
    public bool IsTeleporting => _isTeleporting;
    private Transform _playerPos;

    private void Start()
    {
        _playerPos = PlayerManager.Instance.playerTransform;
        _isTeleporting = false;
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
        if (_isTeleporting)
        {
            _playerPos.position = new Vector3(0, 1, 0);
            transform.position = new Vector3(0, 0.3f, 0);
            _isTeleporting = false; //isTeleporting = ���ּ����� �̵�

            planetManager._currentTime = planetManager._destroyCycle;
        }
        else if (!_isTeleporting)
        {
            _playerPos.position = OutsideMapManager.Instance.playerTeleportPosition; //�÷��̾� ��ġ �̵�
            transform.position = OutsideMapManager.Instance.playerTeleportPosition; //��Ż ��ġ �̵�

            _isTeleporting = true; //!isTeleporting = �༺���� �̵�
        }
    }
}
