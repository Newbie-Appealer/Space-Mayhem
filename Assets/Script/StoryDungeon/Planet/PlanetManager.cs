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
    [SerializeField] float _creationCycle; //15��
    public float _destroyCycle; //5��
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
                //�ڷ���Ʈ ǥ��
                _teleport.SetActive(true);

                _planetObj = Instantiate(_planetPrefList[_planetIndex], new Vector3(-1800, 0, 1100), Quaternion.identity); //�༺ ������Ʈ ����
                _planetObj.GetComponent<Rigidbody>().velocity = Vector3.right * 15;

                yield return new WaitForSeconds(0.02f);

                OutsideMapManager.Instance.F_CreateOutsideMap();//�ܺθ� ����

                yield return new WaitForSeconds(0.02f);

                InsideMapManager.Instance.F_GenerateMaze();//���θ� ����

                _planetIndex++;
            }
        }
    }

    public void F_DeletePlanet()
    {
        if (_currentTime >= _destroyCycle && _isOnPlanet && _planetObj != null)
        {
            _teleport.SetActive(false);

            Destroy(_planetObj); //�༺ ������Ʈ ����
            OutsideMapManager.Instance.F_ExitOutsideMap(); //�ܺθ� ����
            InsideMapManager.Instance.F_DestroyInsideMap(); //���θ� ����

            _isOnPlanet = false;
            _currentTime = 0;
        }
    }
}
