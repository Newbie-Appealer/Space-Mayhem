using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManger : Singleton<UIManger>
{
    [SerializeField] private InputSystem _inputSystem;
    protected override void InitManager()
    {
        
    }
}
