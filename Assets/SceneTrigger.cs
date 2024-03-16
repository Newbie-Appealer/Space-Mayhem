using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string[] _targetScene;
    Transform _playerPos;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] public GameObject _teleportUI;
    [SerializeField] Camera _playerCam;
    PlanetController _planetController;
    int idx;
    private void Awake()
    {
        _planetController = GameObject.Find("PlanetController").GetComponent<PlanetController>();
    }

    private IEnumerator StreamingScene()
    {
        var scene = SceneManager.GetSceneByName(_targetScene[idx]); //불러올 씬 이름으로 가져오기
        if (!scene.isLoaded) //이미 로딩된 상태가 아닌지 검사
        {
            var op = SceneManager.LoadSceneAsync(_targetScene[idx], LoadSceneMode.Additive);
            //비동기 방식으로 씬 불러오기
            //Additive = 다른 씬을 불러오면 원래 씬의 오브젝트 사라져서 문제가 발생할 수 있음
            while (!op.isDone)
            {
                yield return null;
            }
        }
    }

    private IEnumerator UnloadStreamingScene()
    {
        var scene = SceneManager.GetSceneByName(_targetScene[idx]); //불러올 씬 이름으로 가져오기
        if (scene.isLoaded) //이미 로딩된 상태가 아닌지 검사
        {
            var op = SceneManager.LoadSceneAsync(_targetScene[idx], LoadSceneMode.Additive);
            //비동기 방식으로 씬 불러오기
            //Additive = 다른 씬을 불러오면 원래 씬의 오브젝트 사라져서 문제가 발생할 수 있음
            while (!op.isDone)
            {
                yield return null;
            }
        }
    }

    private void Update()
    {
        F_Teleport();
        idx = _planetController._planetIndex;
    }
    public void F_Teleport()
    {
        _playerPos = PlayerManager.Instance.playerTransform;
        if (Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, 5f, _layerMask))
        {
            _teleportUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(StreamingScene());
                _playerPos.position = new Vector3(0, 1000, 0);
            }
        }
        else
        {
            _teleportUI.SetActive(false);
        }
    }
}
