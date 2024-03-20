using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject _planet;
    [SerializeField] GameObject[] _characters;
    private Vector3 _planet_Pos;
    private float _maxPosY = 0.05f;
    private float _planet_MoveSpeed = 1f;

    private void Start()
    {
        _planet_Pos = _planet.transform.localPosition;
    }
    void Update()
    {
        F_PlanetMove();
        F_CharacterRotate();
    }

    private void F_PlanetMove()
    {
        Vector3 _vec = _planet_Pos;
        _planet_Pos.y = _vec.y + _maxPosY * Mathf.Sin(Time.time * _planet_MoveSpeed);
        _planet.transform.localPosition = _planet_Pos;
    }

    private void F_CharacterRotate()
    {
        _characters[0].transform.Rotate(new Vector3(-5f, 5f, 5f) * Time.deltaTime * 2f);
        _characters[1].transform.Rotate(new Vector3(5f, 5f, 5f) * Time.deltaTime * 2f);
        _characters[2].transform.Rotate(new Vector3(5f, 5f, 5f) * Time.deltaTime * 2f);
    }
}
