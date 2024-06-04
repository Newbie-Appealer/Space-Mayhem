using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantEscapeOutsideMap : MonoBehaviour
{
    /// <summary>
    /// 플레이어가 충돌하면 ousideMap의 playerTeleport 위치로 이동 
    /// layer : CantEscapeOusideMapLayer > 플레이어랑만 충돌함 
    /// </summary>

    private void OnCollisionEnter(Collision collision)
    {
        F_SendingBackPlayerToMap(collision.gameObject);
    }

    private void F_SendingBackPlayerToMap(GameObject v_obj) 
    {
        // 플레이어를 외부맵 성성위치로 보내기 
        v_obj.transform.position = OutsideMapManager.Instance.playerTeleportPosition;
    }

}
