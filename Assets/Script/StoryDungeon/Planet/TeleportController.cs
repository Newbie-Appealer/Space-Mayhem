using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    //[SerializeField] GameObject _interactionUI;
    //[SerializeField] TextMeshProUGUI _interactionText;

    private bool isPlayerInTrigger = false; // �÷��̾� ���� ���� ����
    private bool isTeleporting = false; //�༺���� �����ϸ� true
    private Transform playerPos;

    private void Update()
    {
        playerPos = PlayerManager.Instance.playerTransform;

        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (isTeleporting)
            {
                playerPos.localPosition = new Vector3(0, 1, 0);
                transform.localPosition = Vector3.zero;
                isTeleporting = false;
                //isTeleporting = ���ּ����� �̵�
            }
            else if (!isTeleporting)
            {
                playerPos.localPosition = OutsideMapManager.Instance.playerTeleportPosition;
                transform.localPosition = OutsideMapManager.Instance.playerTeleportPosition;

                Debug.Log(OutsideMapManager.Instance.playerTeleportPosition);

                isTeleporting = true;
                //!isTeleporting = �༺���� �̵�
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            // ��ȣ�ۿ� UI�� ������ ��
            //_interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            // ��ȣ�ۿ� UI ����
            //_interactionUI.SetActive(false);
        }
    }

}
