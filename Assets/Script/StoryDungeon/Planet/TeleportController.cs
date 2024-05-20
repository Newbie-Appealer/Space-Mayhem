using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    //[SerializeField] GameObject _interactionUI;
    //[SerializeField] TextMeshProUGUI _interactionText;

    private bool isPlayerInTrigger = false; // 플레이어 텔포 가능 유무
    public bool isTeleporting = false; //행성으로 텔포하면 true
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
                //isTeleporting = 우주선으로 이동
                planetManager._planetTime = planetManager._deleteTime;
                planetManager.F_DestroyPlanet();
            }
            else if (!isTeleporting)
            {
                playerPos.position = OutsideMapManager.Instance.playerTeleportPosition; //플레이어 위치 이동
                transform.position = OutsideMapManager.Instance.playerTeleportPosition; //포탈 위치 이동

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
            // 상호작용 UI를 켜기
            UIManager.Instance.F_IntercationPopup(true, "Press E Teleport");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            // 상호작용 UI 끄기
            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }

}
