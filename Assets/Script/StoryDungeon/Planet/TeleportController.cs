using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    private bool _isPlayerInTrigger = false; // 플레이어 텔포 가능 유무
    private bool _isTeleporting = false; //행성으로 텔포하면 true
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
                //isTeleporting = 우주선으로 이동
                _planetManager._planetTime = _planetManager._deleteTime;
                _planetManager.F_DestroyPlanet();
            }
            else if (!_isTeleporting)
            {
                _playerPos.localPosition = OutsideMapManager.Instance.playerTeleportPosition; //플레이어 위치 이동
                transform.localPosition = OutsideMapManager.Instance.playerTeleportPosition; //포탈 위치 이동

                Debug.Log(OutsideMapManager.Instance.playerTeleportPosition);

                _isTeleporting = true;
                //!isTeleporting = 행성으로 이동
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerInTrigger = true;
            // 상호작용 UI 켜기
            UIManager.Instance.F_IntercationPopup(true, "Press E Teleport");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerInTrigger = false;
            // 상호작용 UI 끄기
            UIManager.Instance.F_IntercationPopup(false, "");
        }
    }
}
