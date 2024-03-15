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
    private int _planetIndex;

    private void Update()
    {
        F_CreatePlanet();
        
    }

    public void F_CreatePlanet()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= 5f && _planetIndex < _planetPrefabs.Length)
        {
            GameObject _planet = Instantiate(_planetPrefabs[_planetIndex], _createPosition.position, Quaternion.identity);
            _planet.transform.SetParent(_createPosition);
            StartCoroutine(F_MovePlanet(_planet));
            _planetIndex++;
            _currentTime = Time.deltaTime;
        }
    }

    IEnumerator F_MovePlanet(GameObject v_planet)
    {
        Rigidbody rb = v_planet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.right * 200;
        while (rb.gameObject.activeSelf)
        {
            if (rb.position.x >= 1500)
            {
                Destroy(rb.gameObject);
                break;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
