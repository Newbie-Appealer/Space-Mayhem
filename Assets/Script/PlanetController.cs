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
    public GameObject _teleport;
    public Rigidbody _planetRb;
    public int _planetIndex;
    public bool _createFlag;

    private void Awake()
    {
        _createFlag = true;
    }
    private void Update()
    {
        F_CreatePlanet();
        
    }
    
    public void F_CreatePlanet()
    {
        if (_createFlag)
        {
            _currentTime += Time.deltaTime; //��� ����Ǵ� �ð�
        }

        if (_currentTime >= 10f && _planetIndex < _planetPrefabs.Length)
        {
            GameObject _planet = Instantiate(_planetPrefabs[_planetIndex], _createPosition.position, Quaternion.identity); //�༺ ����
            _planet.transform.SetParent(_createPosition); //���� ��ġ�� �ڽ����� ����

            _planetRb = _planet.GetComponent<Rigidbody>();

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
                _teleport.GetComponent<SceneTrigger>()._teleportUI.SetActive(false);
                _teleport.SetActive(false);
                _planetIndex++;
                break;
            }
            yield return new WaitForSeconds(1f);
        }
        //_planetIndex++;
    }
}
