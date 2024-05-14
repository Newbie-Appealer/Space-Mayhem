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
            //상호작용 UI 맹글어야 함
            _interactionUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) && playerPos.position.y > 100)
            {
                playerPos.position = Vector3.zero;
                //E 누르고 플레이어 높이가 100 위라면 Vector3.zero
            }
            else if (Input.GetKeyDown(KeyCode.E) && playerPos.position.y < 100)
            {
                playerPos.position = new Vector3(20, 1010, 20);
                //E 누르고 플레이어 높이가100 아래라면 외부 행성 위치로
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
