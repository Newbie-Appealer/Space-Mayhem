using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    //[SerializeField] GameObject _interactionUI;
    //[SerializeField] TextMeshProUGUI _interactionText;

    private bool isPlayerInTrigger = false; // 플레이어 텔포 가능 유무
    private bool isTeleporting = false; //행성으로 텔포하면 true
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
                //isTeleporting = 우주선으로 이동
            }
            else if (!isTeleporting)
            {
                playerPos.localPosition = OutsideMapManager.Instance.playerTeleportPosition;
                transform.localPosition = OutsideMapManager.Instance.playerTeleportPosition;

                Debug.Log(OutsideMapManager.Instance.playerTeleportPosition);

                isTeleporting = true;
                //!isTeleporting = 행성으로 이동
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            // 상호작용 UI를 만들어야 함
            //_interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            // 상호작용 UI 끄기
            //_interactionUI.SetActive(false);
        }
    }

}
