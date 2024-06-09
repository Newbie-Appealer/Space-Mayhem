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

    [HideInInspector] int _planetIndex;
    [SerializeField] int _waitCreatePlanet; //15분
    [SerializeField] int _waitDeletePlanet; //1분30초
    
    [SerializeField] bool _joinPlanet; //행성으로 텔포하면 true
    public bool joinPlanet { get => _joinPlanet; set => _joinPlanet = value; }
    public Transform teleport => _teleport.transform;
    public int planetIdx => _planetIndex;

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

            // 텔포 활성화 / 행성 오브젝트 생성
            _teleport.SetActive(true);
            _teleport.transform.position = _teleportController._defalutPostion_teleport;
            F_CreatePlanet();

            UIManager.Instance.F_PlayerMessagePopupTEXT("Use the portal to enter the planet");

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
                    break;
                }
            }
            // 행성 파괴 대기시간까지 행성에 입장하지않았으면.
            F_DeletePlanet();   // 행성 오브젝트 파괴 / 텔레포트 비활성화
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
        UIManager.Instance.F_PlayerMessagePopupTEXT("Close portal");
        _teleport.SetActive(false);
        Destroy(_planetObj); //행성 오브젝트 삭제
    }
}
