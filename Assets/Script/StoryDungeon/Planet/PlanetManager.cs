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
    GameObject planetObj;
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
        F_CreatePlanet();
        //��Ż�� �����Ǹ鼭 �ܺθʰ� ���θ��� �Բ� �����ȴ�
    }

    private void F_CreatePlanet()
    {
        //15�п� �ѹ��� ��Ż�� �����Ѵ�
        if (_currentTime >= _createTime && !_isOnPlanet)
        {
            _isOnPlanet = true; //�༺�� �����Ǿ��ִ� ���� �ð��� �帣�� ����
            _currentTime = 0; //�ð� �ʱ�ȭ
            _teleport.SetActive(true); //�ڷ���Ʈ ���̰�

            OutsideMapManager.Instance.F_CreateOutsideMap(); //�ܺ� �� ����
            InsideMapManager.Instance.F_GenerateMaze(InsideMapManager.Instance.mazeSize);
            //������������ ���� �� ũ�⸦ �ٸ��� ���� �� ����

            F_MovePlanet();

            if (_planetCount < _planetPrefList.Length)
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
        Destroy(planetObj);
        OutsideMapManager.Instance.F_ExitOutsideMap();
        InsideMapManager.Instance.F_DestroyMaze();
        _teleport.SetActive(false);
        _isOnPlanet = false;
    }
}
