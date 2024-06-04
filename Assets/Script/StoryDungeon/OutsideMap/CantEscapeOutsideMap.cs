using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantEscapeOutsideMap : MonoBehaviour
{
    /// <summary>
    /// �÷��̾ �浹�ϸ� ousideMap�� playerTeleport ��ġ�� �̵� 
    /// layer : CantEscapeOusideMapLayer > �÷��̾���� �浹�� 
    /// </summary>

    private void OnCollisionEnter(Collision collision)
    {
        F_SendingBackPlayerToMap(collision.gameObject);
    }

    private void F_SendingBackPlayerToMap(GameObject v_obj) 
    {
        // �÷��̾ �ܺθ� ������ġ�� ������ 
        v_obj.transform.position = OutsideMapManager.Instance.playerTeleportPosition;
    }

}
