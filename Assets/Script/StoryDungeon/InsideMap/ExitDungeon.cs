using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDungeon : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UIManager.Instance.F_IntercationPopup(true, "Press E ExitDungeon");

            if (Input.GetKeyDown(KeyCode.E))
                PlayerManager.Instance.playerTransform.position = OutsideMapManager.Instance.playerTeleportPosition;
        }
    }
}
