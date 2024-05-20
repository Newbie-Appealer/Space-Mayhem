using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    private bool _isPlayerInTrigger = false; // �÷��̾� ���� ���� ����
    private bool _isTeleporting = false; //�༺���� �����ϸ� true
    public bool isTeleporting => _isTeleporting;
    private Transform _playerPos;
    [SerializeField] PlanetManager _planetManager;

    private void Start()
    {
        _playerPos = PlayerManager.Instance.playerTransform;
    }

    private void Update()
    {
        if (_isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (_isTeleporting)
            {
                _playerPos.localPosition = new Vector3(0, 1, 0);
                transform.localPosition = Vector3.zero;
                _isTeleporting = false;
                //isTeleporting = ���ּ����� �̵�
                _planetManager._planetTime = _planetManager._deleteTime;
                _planetManager.F_DestroyPlanet();
            }
            else if (!_isTeleporting)
            {
                _playerPos.localPosition = OutsideMapManager.Instance.playerTeleportPosition; //�÷��̾� ��ġ �̵�
                transform.localPosition = OutsideMapManager.Instance.playerTeleportPosition; //��Ż ��ġ �̵�

                Debug.Log(OutsideMapManager.Instance.playerTeleportPosition);

                _isTeleporting = true;
                //!isTeleporting = �༺���� �̵�
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerInTrigger = true;
            // ��ȣ�ۿ� UI �ѱ�
            UIManager.Instance.F_IntercationPopup(true, "Press E Teleport");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerInTrigger = false;
            // ��ȣ�ۿ� UI ����
            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }
}
