using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string _targetScene;
    private IEnumerator StreamingScene()
    {
        var scene = SceneManager.GetSceneByName(_targetScene); //�ҷ��� �� �̸����� ��������
        if (scene.isLoaded) //�̹� �ε��� ���°� �ƴ��� �˻�
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            StartCoroutine(StreamingScene());
        }
    }
}
