using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetManager : MonoBehaviour
{
    [SerializeField] GameObject[] _planetPrefList;
    //[SerializeField] GameObject[] _insideMapList;
    [SerializeField] GameObject _teleport;
    int _planetCount = 0;

    float _currentTime = 0;
    float _createTime = 20f; //15 minutes
    bool _isOnPlanet;

    private void Start()
    {
        _isOnPlanet = false;
        _teleport.SetActive(false);
    }
    private void Update()
    {
        if (!_isOnPlanet)
            _currentTime += Time.deltaTime;
        Debug.Log(_currentTime);
        F_CreatePlanet();
        //포탈이 생성되면서 외부맵과 내부맵이 함께 생성된다
    }

    private void F_CreatePlanet()
    {
        //15분에 한번씩 포탈을 생성한다
        if (_currentTime >= _createTime && !_isOnPlanet)
        {
            _isOnPlanet = true; //행성이 생성되어있는 동안 시간이 흐르지 않음
            _currentTime = 0; //시간 초기화
            _teleport.SetActive(true); //텔레포트 보이게

            OutsideMapManager.Instance.F_CreateOutsideMap(); //외부 맵 생성
            InsideMapManager.Instance.F_GenerateMaze(InsideMapManager.Instance.mazeSize);
            //스테이지별로 내부 맵 크기를 다르게 넣을 수 있음

            StartCoroutine(F_MovePlanet());

            if (_planetCount < _planetPrefList.Length)
                _planetCount++;
        }
    }

    IEnumerator F_MovePlanet()
    {
        GameObject planetObj = Instantiate(_planetPrefList[_planetCount], new Vector3(-1000f, 0, 500), Quaternion.identity);
        Rigidbody planetRb = planetObj.GetComponent<Rigidbody>();
        planetRb.velocity = Vector3.right * 30;

        yield return new WaitForSeconds(300); //5 minutes

        Destroy(planetObj);
        _teleport.SetActive(false);
    }
}
