using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
public class PlanetManager : MonoBehaviour
{
    [SerializeField] TeleportController _teleportController;
    [SerializeField] GameObject[] _planetPrefList;
    [SerializeField] GameObject _teleport;
    GameObject _planetObj;

    int _planetIndex;
    [SerializeField] int _waitCreatePlanet; //15분
    [SerializeField] int _waitDeletePlanet; //5분
    
    [SerializeField] bool _joinPlanet; //행성으로 텔포하면 true
    public bool joinPlanet { get => _joinPlanet; set => _joinPlanet = value; }

    private void Start()
    {
        _teleport.SetActive(false);
        _joinPlanet = false;

        StartCoroutine(F_CheckCurrentTime());
    }

    IEnumerator F_CheckCurrentTime()
    {
        while (true)
        {
            // _waitCreate Planet 시간만큼 대기 ( 행성 생성 대기시간 )
            yield return new WaitForSeconds(_waitCreatePlanet);

            // 텔포 생성 / 행성 오브젝트 생성
            F_OnTeleport(true, _teleportController._defalutPostion_teleport);
            UIManager.Instance.F_PlayerMessagePopupTEXT("Use the portal to enter the planet");
            F_CreatePlanet();

            // _waitDeletePlanet 시간만큼 대기 ( 행성 파괴 대기시간 )
            for (int i = 0; i < _waitDeletePlanet; i++)
            {
                // 1초씩 _waitDeletePlanet번 대기
                yield return new WaitForSeconds(1f);

                // 파괴 전 입장하면
                if (joinPlanet)
                {
                    // 행성에서 나올때 까지 대기 후 While 처음으로 되돌아감
                    yield return new WaitWhile(() => joinPlanet);
                    continue;
                }
            }
            // 행성 파괴 대기시간까지 행성에 입장하지않았으면.
            F_OnTeleport(false, _teleportController._defalutPostion_teleport);  // 텔포 비활성화
            F_DeletePlanet();                                                   // 행성 오브젝트 파괴
            UIManager.Instance.F_PlayerMessagePopupTEXT("Close portal");
        }
    }

    public void F_CreatePlanet()
    {
        // 행성 번호 랜덤
        _planetIndex = Random.Range(0, Enum.GetValues(typeof(PlanetType)).Length);

        // 행성 행성
        _planetObj = Instantiate(_planetPrefList[_planetIndex], new Vector3(-1800, 0, 1100), Quaternion.identity); //행성 오브젝트 생성
        _planetObj.GetComponent<Rigidbody>().velocity = Vector3.right * 15;
    }

    public void F_DeletePlanet()
    {
        _teleport.SetActive(false);
        Destroy(_planetObj); //행성 오브젝트 삭제
    }

    public void F_OnTeleport(bool v_state, Vector3 v_pos)
    {
        // 포탈 On / OFF
        _teleport.SetActive(v_state);

        // 포탈 위치 옮겨질떄까지 While문으로 보내버리기
        while(Vector3.Distance(v_pos, _teleport.transform.position) >= 1f)
            _teleport.transform.position = v_pos;
        // 플레이어가 이동하지않는 버그를 해결하기위한 방법과 동일함.
        // 더 좋은 해결방법있으면 수정하면될듯
    }
}
