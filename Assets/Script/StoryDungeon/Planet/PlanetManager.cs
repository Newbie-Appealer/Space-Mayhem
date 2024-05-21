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

    public float _currentTime;
    int _planetIndex;
    [SerializeField] int _waitCreatePlanet; //15분
    [SerializeField] int _waitDeletePlanet; //5분
    bool _infinity;

    private void Start()
    {
        _teleport.SetActive(false);
        _planetIndex = 0;
        _infinity = true;
        StartCoroutine(F_CheckCurrentTime());
    }

    IEnumerator F_CheckCurrentTime()
    {
        while (_infinity)
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
        _planetIndex = Random.Range(0, _planetPrefList.Length);

        _planetObj = Instantiate(_planetPrefList[_planetIndex], new Vector3(-1800, 0, 1100), Quaternion.identity); //행성 오브젝트 생성
        _planetObj.GetComponent<Rigidbody>().velocity = Vector3.right * 15;

        OutsideMapManager.Instance.F_CreateOutsideMap();//외부맵 생성
        InsideMapManager.Instance.F_GenerateMaze();//내부맵 생성
    }

    public void F_DeletePlanet()
    {
        _teleport.SetActive(false);

        Destroy(_planetObj); //행성 오브젝트 삭제
        OutsideMapManager.Instance.F_ExitOutsideMap(); //외부맵 삭제
        InsideMapManager.Instance.F_DestroyInsideMap(); //내부맵 삭제
    }
}
