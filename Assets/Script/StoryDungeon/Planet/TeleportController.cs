using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    [HideInInspector] public Vector3 _defalutPostion_teleport;
    [HideInInspector] public Vector3 _defalutPostion_player;

    [SerializeField] PlanetManager planetManager;
    private Transform _playerPos;

    private void Start()
    {
        // 초기화
        _playerPos = PlayerManager.Instance.playerTransform;
        _defalutPostion_teleport = new Vector3(0, 0.3f, 0);
        _defalutPostion_player = new Vector3(0, 1f, 0);
    }

    public IEnumerator F_TeleportPlayer()
    {
        // 플레이어 Rigidbody Kinematic 속성을 false(물리충돌 OFF)
        PlayerManager.Instance.PlayerController.F_OnKinematic(true);
        UIManager.Instance.F_PlayerMessagePopupTEXT("Teleport to Planet");

        // 로딩 ON
        UIManager.Instance.F_OnLoading(true);
        yield return new WaitForSeconds(1f);

        // [ 행성 -> 우주선 ]
        if (planetManager.joinPlanet)
        {
            // 행성 입장상태를 false로 전환
            planetManager.joinPlanet = false;

            // 1. 플레이어 이동 
            _playerPos.position = _defalutPostion_player;
            yield return new WaitForSeconds(0.5f);

            // 2. 포탈 이동
            planetManager.teleport.position = _defalutPostion_teleport;
            yield return new WaitForSeconds(0.5f);

            // 3. 맵 삭제
            OutsideMapManager.Instance.F_ExitOutsideMap();      //외부맵 삭제
            yield return new WaitForSeconds(0.5f);
            InsideMapManager.Instance.F_DestroyInsideMap();     //내부맵 삭제
            yield return new WaitForSeconds(0.5f);

            // 4. 행성 오브젝트 파괴
            planetManager.F_DeletePlanet();
            yield return new WaitForSeconds(0.5f);
        }

        // [ 우주선 -> 행성 ]
        else if (!planetManager.joinPlanet)
        {
            // 행성 입장상태를 true 전환
            planetManager.joinPlanet = true;

            // 1. 행성 생성
            OutsideMapManager.Instance.F_CreateOutsideMap();    //외부맵 생성
            yield return new WaitForSeconds(0.5f);
            InsideMapManager.Instance.F_GenerateInsideMap();         //내부맵 생성
            yield return new WaitForSeconds(0.5f);


            // 2. 플레이어 이동
            _playerPos.position = OutsideMapManager.Instance.playerTeleportPosition;
            yield return new WaitForSeconds(0.5f);

            // 3. 포탈 이동
            planetManager.teleport.position = OutsideMapManager.Instance.playerTeleportPosition;
            yield return new WaitForSeconds(0.5f);
        }

        // 로딩 OFF 및 플레이어 Rigidbody Kinematic 속성을 false ( 물리충돌 ON )
        yield return new WaitForSeconds(0.5f);
        PlayerManager.Instance.PlayerController.F_OnKinematic(false);
        UIManager.Instance.F_OnLoading(false);
    }
}
