using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetManager : MonoBehaviour
{
    [SerializeField] TeleportController _teleportController;
    [SerializeField] GameObject[] _planetPrefList;
    [SerializeField] GameObject _teleport;
    GameObject _planetObj;

    public float _currentTime;
    [SerializeField] float _creationCycle; //15분
    public float _destroyCycle; //5분
    int _planetIndex;
    bool _isOnPlanet;

    private void Start()
    {
        _teleport.SetActive(false);
        _planetIndex = 0;
        _isOnPlanet = false;
    }
    private void Update()
    {
        if (!_teleportController.IsTeleporting)
            _currentTime += Time.deltaTime;
        else
            _currentTime = 0;

        StartCoroutine(F_CreatePlanet());
        F_DeletePlanet();
    }

    IEnumerator F_CreatePlanet()
    {
        if (_planetIndex < _planetPrefList.Length)
        {
            if (_currentTime >= _creationCycle && !_isOnPlanet)
            {
                _isOnPlanet = true;
                _currentTime = 0;
                //텔레포트 표시
                _teleport.SetActive(true);

                _planetObj = Instantiate(_planetPrefList[_planetIndex], new Vector3(-1800, 0, 1100), Quaternion.identity); //행성 오브젝트 생성
                _planetObj.GetComponent<Rigidbody>().velocity = Vector3.right * 15;

                yield return new WaitForSeconds(0.02f);

                OutsideMapManager.Instance.F_CreateOutsideMap();//외부맵 생성

                yield return new WaitForSeconds(0.02f);

                InsideMapManager.Instance.F_GenerateMaze();//내부맵 생성

                _planetIndex++;
            }
        }
    }

    public void F_DeletePlanet()
    {
        if (_currentTime >= _destroyCycle && _isOnPlanet && _planetObj != null)
        {
            _teleport.SetActive(false);

            Destroy(_planetObj); //행성 오브젝트 삭제
            OutsideMapManager.Instance.F_ExitOutsideMap(); //외부맵 삭제
            InsideMapManager.Instance.F_DestroyInsideMap(); //내부맵 삭제

            _isOnPlanet = false;
            _currentTime = 0;
        }
    }
}
