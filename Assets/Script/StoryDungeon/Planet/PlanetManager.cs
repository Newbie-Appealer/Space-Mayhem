using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetManager : MonoBehaviour
{
    [SerializeField] TeleportController teleportController;
    [SerializeField] GameObject[] _planetPrefList; //행성 외부 모습
    //[SerializeField] GameObject[] _insideMapList;
    [SerializeField] GameObject _teleport;
    GameObject planetObj; //행성 외부 오브젝트 담는 변수
    int _planetCount; //_planetPrefList Index

    [SerializeField] float _currentTime;
    public float _planetTime; //생성 후 유지시간
    float _createTime = 10f; //생성 주기 15 minutes
    [HideInInspector] public float _deleteTime = 10f;
    bool _isOnPlanet; //행성 생성 조건

    private void Start()
    {
        _isOnPlanet = false;
        _teleport.SetActive(false);
    }
    private void Update()
    {
        F_CreatePlanet();
        //포탈이 생성되면서 외부맵과 내부맵이 함께 생성
        F_DestroyPlanet();
    }

    private void F_CreatePlanet()
    {
        if (!_isOnPlanet && !teleportController.isTeleporting)
            _currentTime += Time.deltaTime;

        //15분에 한번씩 포탈 생성
        if (_currentTime >= _createTime && !_isOnPlanet)
        {
            _isOnPlanet = true; //행성이 생성되어있는 동안 시간이 흐르지 않음
            _currentTime = 0; //시간 초기화
            _teleport.SetActive(true); //텔레포트 보이게

            OutsideMapManager.Instance.F_CreateOutsideMap(); //외부 맵 생성
            InsideMapManager.Instance.F_GenerateMaze(InsideMapManager.Instance.mazeSize);
            //스테이지별로 내부 맵 크기를 다르게 넣을 수 있음

            F_MovePlanet();

            if (_planetCount < _planetPrefList.Length - 1)
                _planetCount++;
        }
    }

    public void F_MovePlanet()
    {
        planetObj = Instantiate(_planetPrefList[_planetCount], new Vector3(-1000f, 0, 500), Quaternion.identity);
        planetObj.GetComponent<Rigidbody>().velocity = Vector3.right * 15;
    }

    public void F_DestroyPlanet()
    {
        if (_isOnPlanet && !teleportController.isTeleporting)
            _planetTime += Time.deltaTime;

        if (_planetTime >= _deleteTime && _isOnPlanet)
        {
            Destroy(planetObj);
            OutsideMapManager.Instance.F_ExitOutsideMap();
            InsideMapManager.Instance.F_DestroyMaze();
            _teleport.SetActive(false);
            _isOnPlanet = false;
            _planetTime = 0;
        }
    }
}
