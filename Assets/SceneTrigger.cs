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
        var scene = SceneManager.GetSceneByName(_targetScene[idx]); //�ҷ��� �� �̸����� ��������
        if (!scene.isLoaded) //�̹� �ε��� ���°� �ƴ��� �˻�
        {
            var op = SceneManager.LoadSceneAsync(_targetScene[idx], LoadSceneMode.Additive);
            //�񵿱� ������� �� �ҷ�����
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
        if (scene.isLoaded) //�̹� �ε��� ���°� �ƴ��� �˻�
        {
            var op = SceneManager.LoadSceneAsync(_targetScene[idx], LoadSceneMode.Additive);
            //�񵿱� ������� �� �ҷ�����
            //Additive = �ٸ� ���� �ҷ����� ���� ���� ������Ʈ ������� ������ �߻��� �� ����
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
