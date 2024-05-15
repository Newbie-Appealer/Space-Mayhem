using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    //[SerializeField] GameObject _interactionUI;
    //[SerializeField] TextMeshProUGUI _interactionText;

    private bool isPlayerInTrigger = false; // 플레이어 텔포 가능 유무
    private Transform playerPos;

    private void Update()
    {
        playerPos = PlayerManager.Instance.playerTransform;

        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (playerPos.position.y > 100)
            {
                playerPos.position = new Vector3(0, 1, 0);
                // E를 누르고 플레이어 높이가 100 이상이면 Vector3.zero로 이동
            }
            else if (playerPos.position.y < 100)
            {
                playerPos.position = new Vector3(20, 1010, 20);
                // E를 누르고 플레이어 높이가 100 이하이면 외부 행성 위치로 이동
            }
        }
    }
    private void OnTriggerEnter(Collider other)
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
