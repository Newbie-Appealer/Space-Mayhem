using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDungeon : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UIManager.Instance.F_IntercationPopup(true, "Press E EnterDungeon");

            if (Input.GetKeyDown(KeyCode.E))
                PlayerManager.Instance.playerTransform.position = InsideMapManager.Instance._startRoom.transform.position;
        }
    }
}
