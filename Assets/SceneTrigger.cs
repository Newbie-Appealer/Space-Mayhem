using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string[] _targetScene;
    [SerializeField] GameObject _playerPos;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] public GameObject _teleportUI;
    [SerializeField] Camera _playerCam;
    PlanetController _planetController;
    [SerializeField] Vector3[] _teleportPos;
    int idx;
    private void Awake()
    {
        _planetController = GameObject.Find("PlanetController").GetComponent<PlanetController>();
    }

    private IEnumerator StreamingScene()
    {
        var scene = SceneManager.GetSceneByName(_targetScene[idx]); //�ҷ��� �� �̸����� ��������
        if (!scene.isLoaded) //�̹� �ε��� ���°� �ƴ��� �˻�
        {
            _planetController._createFlag = false;
            _planetController._planetRb.velocity = Vector3.zero;
            _playerPos.transform.position = new Vector3(0, 1000, 0);
            _planetController._teleport.transform.position = _teleportPos[idx];

            var op = SceneManager.LoadSceneAsync(_targetScene[idx], LoadSceneMode.Additive);
            //�񵿱� ������� �� �ε�
            //Additive = �ٸ� ���� �ҷ����� ���� ���� ������Ʈ ������� ������ �߻��� �� ����
            while (!op.isDone)
            {
                yield return null;
            }
        }
    }

    private IEnumerator UnloadStreamingScene()
    {
        var scene = SceneManager.GetSceneByName(_targetScene[idx]); //�ҷ��� �� �̸����� ��������
        if (scene.isLoaded) //�̹� �ε��� �������� �˻�
        {
            _planetController._createFlag = true;
            _planetController._planetRb.velocity = Vector3.right * 500;
            _playerPos.transform.position = Vector3.zero;
            _planetController._teleport.transform.position = new Vector3(0, 0.3f, 0);

            var op = SceneManager.UnloadSceneAsync(_targetScene[idx]);
            //�񵿱� ������� �� ��ε�
            while (op.isDone)
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
        if (Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, 5f, _layerMask))
        {
            _teleportUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(StreamingScene());
                StartCoroutine(UnloadStreamingScene());
            }
        }
        else
        {
            _teleportUI.SetActive(false);
        }
    }
}
