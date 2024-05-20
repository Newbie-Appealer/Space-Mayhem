using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    //[SerializeField] GameObject _interactionUI;
    //[SerializeField] TextMeshProUGUI _interactionText;

    private bool isPlayerInTrigger = false; // �÷��̾� ���� ���� ����
    public bool isTeleporting = false; //�༺���� �����ϸ� true
    private Transform playerPos;
    [SerializeField] PlanetManager planetManager;

    private void Start()
    {
        playerPos = PlayerManager.Instance.playerTransform;
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (isTeleporting)
            {
                playerPos.position = new Vector3(0, 1, 0);
                transform.position = Vector3.zero;
                isTeleporting = false;
                //isTeleporting = ���ּ����� �̵�
                planetManager._planetTime = planetManager._deleteTime;
                planetManager.F_DestroyPlanet();
            }
            else if (!isTeleporting)
            {
                playerPos.position = OutsideMapManager.Instance.playerTeleportPosition; //�÷��̾� ��ġ �̵�
                transform.position = OutsideMapManager.Instance.playerTeleportPosition; //��Ż ��ġ �̵�

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
            // ��ȣ�ۿ� UI�� �ѱ�
            UIManager.Instance.F_IntercationPopup(true, "Press E Teleport");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            // ��ȣ�ۿ� UI ����
            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }

}
