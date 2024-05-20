using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    [SerializeField] PlanetManager planetManager;
    private bool _isTeleporting; //행성으로 텔포하면 true
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
            UIManager.Instance.F_IntercationPopup(true, "Press E Teleport"); // 상호작용 UI 켜기

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
            UIManager.Instance.F_IntercationPopup(false, ""); // 상호작용 UI 끄기
        }
    }

    public void F_TeleportPlayer()
    {
        if (_isTeleporting)
        {
            _playerPos.localPosition = new Vector3(0, 1, 0);
            transform.localPosition = Vector3.zero;
            _isTeleporting = false; //isTeleporting = 우주선으로 이동

            planetManager._currentTime = planetManager._destroyCycle;
        }
        else if (!_isTeleporting)
        {
            _playerPos.localPosition = OutsideMapManager.Instance.playerTeleportPosition; //플레이어 위치 이동
            transform.localPosition = OutsideMapManager.Instance.playerTeleportPosition; //포탈 위치 이동

            Debug.Log(OutsideMapManager.Instance.playerTeleportPosition);

            _isTeleporting = true; //!isTeleporting = 행성으로 이동
        }
    }
}
