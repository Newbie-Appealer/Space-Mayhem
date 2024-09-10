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
        UIManager.Instance.F_PlayerMessagePopupTEXT("행성으로 이동중");

        // 로딩 ON
        UIManager.Instance.F_OnLoading(true);
        yield return new WaitForSeconds(1f);

        // [ 행성 -> 우주선 ]
        if (planetManager.joinPlanet)
        {
            // 행성 입장상태를 false로 전환
            planetManager.joinPlanet = false;

            // 1. 플레이어 이동 및 부활
            _playerPos.position = _defalutPostion_player;
            PlayerManager.Instance.PlayerController._isPlayerDead = false;
            yield return new WaitForSeconds(0.5f);

            // 2. 포탈 이동
            planetManager.teleport.position = _defalutPostion_teleport;
            yield return new WaitForSeconds(0.5f);

            // 3. 맵/오브젝트/몬스터 삭제
            OutsideMapManager.Instance.F_ExitOutsideMap();          //외부맵 삭제
            yield return new WaitForSeconds(0.5f);
            InsideMapManager.Instance.F_DestroyInsideMap();         //내부맵 삭제
            yield return new WaitForSeconds(0.5f);
            EnemyManager.Instance.F_RemoveEnemy();                  //몬스터 삭제
            yield return new WaitForSeconds(0.25f);
            ItemManager.Instance.dropItemSystem.F_RemoveObjects();  //아이템오브젝트 삭제
            yield return new WaitForSeconds(0.25f);

            // 4. 행성 오브젝트 파괴
            planetManager.F_DeletePlanet();
            yield return new WaitForSeconds(0.5f);

            // 5. 사운드 변경 ( 행성 -> 우주선 ) 우주선 BGM 로테이션 재생
            SoundManager.Instance.F_StartSpaceShipBGM();
        }

        // [ 우주선 -> 행성 ]
        else if (!planetManager.joinPlanet)
        {
            // 행성 입장상태를 true 전환
            planetManager.joinPlanet = true;

            // 1. 행성 생성
            OutsideMapManager.Instance.F_CreateOutsideMap();    //외부맵 생성
            yield return new WaitForSeconds(0.5f);
            InsideMapManager.Instance.F_GenerateInsideMap();    //내부맵 생성
            yield return new WaitForSeconds(0.5f);


            // 2. 플레이어 이동
            _playerPos.position = OutsideMapManager.Instance.playerTeleportPosition;
            yield return new WaitForSeconds(0.5f);

            // 3. 포탈 이동
            planetManager.teleport.position = OutsideMapManager.Instance.playerTeleportPosition;
            yield return new WaitForSeconds(0.5f);

            // 4. 사운드 변경 ( 우주선 -> 행성 ) 외부맵 BGM 로테이션 재생
            SoundManager.Instance.F_StartOUTSideBGM();
        }

        // 로딩 OFF 및 플레이어 Rigidbody Kinematic 속성을 false ( 물리충돌 ON )
        yield return new WaitForSeconds(0.5f);
        PlayerManager.Instance.PlayerController.F_OnKinematic(false);
        UIManager.Instance.F_OnLoading(false);
    }
}
