using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthManager : MonoBehaviour
{
    [Header("Planet")]
    [SerializeField] private GameObject _planet;
    private float _maxMoveDistance = 0.003f;
    private float _planetMoveSpeed = 0.8f;

    [Header("Character")]
    [SerializeField] private GameObject[] _chr_Arr;

    private void Update()
    {
        F_PlanetMove();
        F_PeopleMove();
    }

    private void  F_PlanetMove()
    {
        Vector3 _planetV = _planet.transform.position;
        _planetV.y += _maxMoveDistance * Mathf.Sin(Time.time * _planetMoveSpeed);
        _planet.transform.position = _planetV;
    }

    private void F_PeopleMove()
    {

    }
}
