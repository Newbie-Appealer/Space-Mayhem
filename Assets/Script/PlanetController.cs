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
        _currentTime += Time.deltaTime; //��� ����Ǵ� �ð�

        if (_currentTime >= 30f && _planetIndex < _planetPrefabs.Length)
        {
            GameObject _planet = Instantiate(_planetPrefabs[_planetIndex], _createPosition.position, Quaternion.identity); //�༺ ����
            _planet.transform.SetParent(_createPosition); //���� ��ġ�� �ڽ����� ����

            Rigidbody _planetRb = _planet.GetComponent<Rigidbody>();
            _planetRb.velocity = Vector3.right * 500;

            _teleport.SetActive(true); //�ڷ���Ʈ ������ �༺�� �þ߿� ���� �� ������ �ɷ� �ٲٱ�
            StartCoroutine(F_DestroyPlanet(_planetRb)); //�༺ ���� üũ

            _currentTime = Time.deltaTime; //�༺ ���� �� �ð� �ʱ�ȭ
        }
    }

    IEnumerator F_DestroyPlanet(Rigidbody _planetRb)
    {
        while (_planetRb.gameObject.activeSelf)
        {
            if (_planetRb.position.x >= 1500) //�༺�� x���� 1500�� ������
            {
                Destroy(_planetRb.gameObject); //�༺ ����
                _teleport.SetActive(false); //�ڷ���Ʈ ����
                _teleport.GetComponent<SceneTrigger>()._teleportUI.SetActive(false);
                break;
            }
            yield return new WaitForSeconds(1f);
        }
        _planetIndex++;
    }
}
