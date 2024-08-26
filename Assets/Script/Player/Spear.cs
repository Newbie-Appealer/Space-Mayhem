using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Spear : MonoBehaviour
{
    [SerializeField] private Transform _spear_Tail;
    [SerializeField] private Transform _pistol_Muzzle;
    [SerializeField] private LineRenderer _line;

    public void F_EnableLine()
    {
        _line.enabled = true;
        _line.positionCount = 2;
        _line.SetPosition(0, _pistol_Muzzle.position);
        _line.SetPosition(1, _spear_Tail.position);
    }

    public void F_DisableLine()
    {
        _line.enabled = false;
    }

    public void F_RestoreLine()
    {
        _line.SetPosition(0, _pistol_Muzzle.position);
        _line.SetPosition(1, _spear_Tail.position);
    }

    public Vector3 F_GetFirePos()
    {
        return _pistol_Muzzle.position;
    }
}
