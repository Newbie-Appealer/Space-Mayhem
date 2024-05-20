using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetManager : MonoBehaviour
{
    [SerializeField] TeleportController _teleportController;
    [SerializeField] GameObject[] _planetPrefList;
    //[SerializeField] GameObject[] _insideMapList;
    [SerializeField] GameObject _teleport;
    GameObject _planetObj;
    int _planetCount;

    [SerializeField] float _currentTime;
    public float _planetTime;
    [SerializeField] float _createTime; //15 minutes
    public float _deleteTime; //5minutes
    bool _isOnPlanet;

    private void Start()
    {
        _isOnPlanet = false;
        _teleport.SetActive(false);
    }
    private void Update()
    {
        F_CreatePlanet();
        //��Ż�� �����Ǹ鼭 �ܺθʰ� ���θ��� �Բ� ����
        F_DestroyPlanet();
    }

    private void F_CreatePlanet()
    {
        if (!_isOnPlanet && !_teleportController.isTeleporting)
            _currentTime += Time.deltaTime;

        //15�п� �ѹ��� ��Ż ����
        if (_currentTime >= _createTime && !_isOnPlanet)
        {
            _isOnPlanet = true; //�༺�� �����Ǿ��ִ� ���� �ð��� �帣�� ����
            _currentTime = 0; //�ð� �ʱ�ȭ
            _teleport.SetActive(true); //�ڷ���Ʈ ���̰�

            OutsideMapManager.Instance.F_CreateOutsideMap(); //�ܺ� �� ����
            InsideMapManager.Instance.F_GenerateMaze(InsideMapManager.Instance.mazeSize);
            //������������ ���� �� ũ�⸦ �ٸ��� ���� �� ����

            F_MovePlanet();

            if (_planetCount < _planetPrefList.Length - 1)
                _planetCount++;
        }
    }

    public void F_MovePlanet()
    {
        _planetObj = Instantiate(_planetPrefList[_planetCount], new Vector3(-1000f, 0, 500), Quaternion.identity);
        _planetObj.GetComponent<Rigidbody>().velocity = Vector3.right * 15;
    }

    public void F_DestroyPlanet()
    {
        if (_isOnPlanet && !_teleportController.isTeleporting)
            _planetTime += Time.deltaTime;

        if (_planetTime >= _deleteTime && _isOnPlanet)
        {
            Destroy(_planetObj);
            OutsideMapManager.Instance.F_ExitOutsideMap();
            InsideMapManager.Instance.F_DestroyMaze();
            _teleport.SetActive(false);
            _isOnPlanet = false;
            _planetTime = 0;
        }
    }
}
