using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject _playerObject;
    public GameObject playerObject
    { get { return _playerObject; } }

    protected override void InitManager()
    {
        
    }
}
