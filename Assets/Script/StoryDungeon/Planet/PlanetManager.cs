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
    int _planetIndex;
    [SerializeField] int _waitCreatePlanet; //15��
    [SerializeField] int _waitDeletePlanet; //5��
    bool _a;

    private void Start()
    {
        _teleport.SetActive(false);
        _planetIndex = 0;
        StartCoroutine(F_CheckCurrentTime());
    }

    IEnumerator F_CheckCurrentTime()
    {
        while (_planetIndex < _planetPrefList.Length)
        {
            yield return new WaitForSeconds(_waitCreatePlanet);

            _teleport.SetActive(true);

            yield return new WaitForSeconds(_waitDeletePlanet);

            if (!_teleportController.JoinPlanet)
                _teleport.SetActive(false);
            else
                yield return new WaitWhile(() => _teleportController.JoinPlanet);
        }
    }

    public void F_CreatePlanet()
    {
        //�ڷ���Ʈ ǥ��

        _planetObj = Instantiate(_planetPrefList[_planetIndex], new Vector3(-1800, 0, 1100), Quaternion.identity); //�༺ ������Ʈ ����
        _planetObj.GetComponent<Rigidbody>().velocity = Vector3.right * 15;
        _planetIndex++;

        OutsideMapManager.Instance.F_CreateOutsideMap();//�ܺθ� ����
        InsideMapManager.Instance.F_GenerateMaze();//���θ� ����
    }

    public void F_DeletePlanet()
    {
        _teleport.SetActive(false);

        Destroy(_planetObj); //�༺ ������Ʈ ����
        OutsideMapManager.Instance.F_ExitOutsideMap(); //�ܺθ� ����
        InsideMapManager.Instance.F_DestroyInsideMap(); //���θ� ����
    }
}
