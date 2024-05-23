using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDungeon : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        //�÷��̾�� �浹
        if (collision.gameObject.CompareTag("Player"))
        {
            //UI ǥ��
            UIManager.Instance.F_IntercationPopup(true, "Press E EnterDungeon");

            if (Input.GetKeyDown(KeyCode.E))
            {
                //�÷��̾� ��ġ ���� �������� �̵�
                PlayerManager.Instance.playerTransform.position = InsideMapManager.Instance._startRoom.transform.position;
                //��ü �� ����Ʈ ����
                InsideMapManager.Instance.mapLight.SetActive(false);
            }
        }
    }
}
