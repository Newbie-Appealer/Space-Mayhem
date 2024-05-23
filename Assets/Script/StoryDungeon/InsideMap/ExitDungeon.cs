using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDungeon : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        //�÷��̾�� �浹
        if (collision.gameObject.CompareTag("Player"))
        {
            //UI ǥ��
            UIManager.Instance.F_IntercationPopup(true, "Press E ExitDungeon");

            if (Input.GetKeyDown(KeyCode.E))
            {
                //�÷��̾� ��ġ ���� �������� �̵�
                PlayerManager.Instance.playerTransform.position = OutsideMapManager.Instance.playerTeleportPosition;
                //��ü �� ����Ʈ ����
                InsideMapManager.Instance.mapLight.SetActive(true);
            }
        }
    }
}
