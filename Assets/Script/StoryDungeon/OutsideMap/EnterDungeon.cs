using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDungeon : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        //플레이어와 충돌
        if (collision.gameObject.CompareTag("Player"))
        {
            //UI 표시
            UIManager.Instance.F_IntercationPopup(true, "Press E EnterDungeon");

            if (Input.GetKeyDown(KeyCode.E))
            {
                //플레이어 위치 내부 던전으로 이동
                PlayerManager.Instance.playerTransform.position = InsideMapManager.Instance._startRoom.transform.position;
                //전체 맵 라이트 끄기
                InsideMapManager.Instance.mapLight.SetActive(false);
            }
        }
    }
}
