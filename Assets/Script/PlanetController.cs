using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetController : MonoBehaviour
{
    [SerializeField] private GameObject[] _planetPrefabs;
    [SerializeField] private Transform _createPosition;
    [SerializeField] private float _currentTime;
    [SerializeField] private GameObject _teleport;
    public int _planetIndex;

    private void Update()
    {
        F_CreatePlanet();
        
    }

    public void F_CreatePlanet()
    {
        _currentTime += Time.deltaTime; //계속 진행되는 시간

        if (_currentTime >= 30f && _planetIndex < _planetPrefabs.Length)
        {
            GameObject _planet = Instantiate(_planetPrefabs[_planetIndex], _createPosition.position, Quaternion.identity); //행성 생성
            _planet.transform.SetParent(_createPosition); //생성 위치의 자식으로 지정

            Rigidbody _planetRb = _planet.GetComponent<Rigidbody>();
            _planetRb.velocity = Vector3.right * 500;

            _teleport.SetActive(true); //텔레포트 생성은 행성이 시야에 보일 때 나오는 걸로 바꾸기
            StartCoroutine(F_DestroyPlanet(_planetRb)); //행성 삭제 체크

            _currentTime = Time.deltaTime; //행성 생성 후 시간 초기화
        }
    }

    IEnumerator F_DestroyPlanet(Rigidbody _planetRb)
    {
        while (_planetRb.gameObject.activeSelf)
        {
            if (_planetRb.position.x >= 1500) //행성의 x값이 1500이 넘으면
            {
                Destroy(_planetRb.gameObject); //행성 삭제
                _teleport.SetActive(false); //텔레포트 삭제
                _teleport.GetComponent<SceneTrigger>()._teleportUI.SetActive(false);
                break;
            }
            yield return new WaitForSeconds(1f);
        }
        _planetIndex++;
    }
}
