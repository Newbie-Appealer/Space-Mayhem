using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string _targetScene;
    Transform _playerPos;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] GameObject _teleportUI;
    [SerializeField] Camera _playerCam;
    private IEnumerator StreamingScene()
    {
        var scene = SceneManager.GetSceneByName(_targetScene); //�ҷ��� �� �̸����� ��������
        if (!scene.isLoaded) //�̹� �ε��� ���°� �ƴ��� �˻�
        {
            var op = SceneManager.LoadSceneAsync(_targetScene, LoadSceneMode.Additive);
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
    }
    public void F_Teleport()
    {
        if (Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, 5f, _layerMask))
        {
            _teleportUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(StreamingScene());
                _playerPos = PlayerManager.Instance.playerTransform;
                _playerPos.position = new Vector3(0, 1000, 0);
            }
        }
        else
        {
            _teleportUI.SetActive(false);
        }
    }
}
