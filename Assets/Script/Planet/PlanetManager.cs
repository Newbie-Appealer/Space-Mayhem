using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _planetPrefabs;
    [SerializeField] private Transform _planet;
    private float _currentTime;
    private bool _createPlanet;

    private void Awake()
    {
        _createPlanet = true;
    }
    private void Update()
    {
        _currentTime = Time.realtimeSinceStartup;
        
    }

    public void F_CreatePlanet()
    {
        for (int i = 0; i < _planetPrefabs.Count(); i++)
        {
            if (_currentTime / 15f == 0 && _createPlanet)
            {
                GameObject temp = Instantiate(_planetPrefabs[i], _planet.position, Quaternion.identity);
                temp.transform.SetParent(_planet);
                _createPlanet = false;
            }
        }
    }

}
