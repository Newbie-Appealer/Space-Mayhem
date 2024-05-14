using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    [SerializeField] GameObject _interactionUI;
    [SerializeField] TextMeshProUGUI _interactionText;

    private void OnTriggerEnter(Collider other)
    {
        Transform playerPos = PlayerManager.Instance.playerTransform;
        if (other.gameObject.CompareTag("Player"))
        {
            //��ȣ�ۿ� UI �ͱ۾�� ��
            _interactionUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) && playerPos.position.y > 100)
            {
                playerPos.position = Vector3.zero;
                //E ������ �÷��̾� ���̰� 100 ����� Vector3.zero
            }
            else if (Input.GetKeyDown(KeyCode.E) && playerPos.position.y < 100)
            {
                playerPos.position = new Vector3(20, 1010, 20);
                //E ������ �÷��̾� ���̰�100 �Ʒ���� �ܺ� �༺ ��ġ��
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _interactionUI.SetActive(false);

        }
    }
}
