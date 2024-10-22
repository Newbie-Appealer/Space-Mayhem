using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EarthManager : MonoBehaviour
{
    [Header("Planet")]
    [SerializeField] private GameObject _planet;
    private float _maxMoveDistance = 0.002f;
    private float _planetMoveSpeed = 0.8f;

    [Header("Character")]
    [SerializeField] private GameObject[] _chr_Arr;
    private float _chrMoveDistance = 0.001f;
    private float _chrMoveSpeed = 0.8f;

    private void Update()
    {
        F_PlanetMove();
        //F_PeopleMove();

        // 1. Managers �̸��� ���� ������Ʈ�� �ִ��� Ȯ��.
        // 2. ������ �׳� ���ֹ�����.
        F_DestroyManagers();
    }

    /// <summary>
    /// Ȥ�� ���� �����ִ� Manager ���� �Լ�
    /// </summary>
    private void F_DestroyManagers()
    {
        GameObject _manager = GameObject.Find("Managers");
        if (_manager != null)
            Destroy(_manager);
        else
            return;
    }

    private void  F_PlanetMove()
    {
        Vector3 _planetV = _planet.transform.position;
        _planetV.y += _maxMoveDistance * Mathf.Sin(Time.time * _planetMoveSpeed);
        _planet.transform.position = _planetV;
    }

    private void F_PeopleMove()
    {
        foreach (GameObject v_go in _chr_Arr)
        {
        Vector3 _chrVector = v_go.transform.position;
        _chrVector.y += _chrMoveDistance * Mathf.Sin(Time.time * _chrMoveSpeed);
        v_go.transform.position = _chrVector;
        }
    }
}
