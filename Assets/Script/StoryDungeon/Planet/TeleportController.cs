using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    //[SerializeField] GameObject _interactionUI;
    //[SerializeField] TextMeshProUGUI _interactionText;

    private bool isPlayerInTrigger = false; // �÷��̾� ���� ���� ����
    private Transform playerPos;

    private void Update()
    {
        playerPos = PlayerManager.Instance.playerTransform;

        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (playerPos.position.y > 100)
            {
                playerPos.position = new Vector3(0, 1, 0);
                // E�� ������ �÷��̾� ���̰� 100 �̻��̸� Vector3.zero�� �̵�
            }
            else if (playerPos.position.y < 100)
            {
                playerPos.position = new Vector3(20, 1010, 20);
                // E�� ������ �÷��̾� ���̰� 100 �����̸� �ܺ� �༺ ��ġ�� �̵�
            }
        }
    }
    private void OnTriggerEnter(Collider other)
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
