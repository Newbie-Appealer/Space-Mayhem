using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("=== 작살 ===")]
    [SerializeField] private Spear _spear;
    
    public void F_SpearPowerCharge()
    {
        _spear._spearFireSpeed+= Time.deltaTime * 0.5f;
        if ( _spear._spearFireSpeed > 2f )
        {
            _spear._spearFireSpeed = 2f;
        }
    }
    
    public void F_SpearFire()
    {
        Debug.Log("작살 발사");
        _spear.F_SpearFire();
    }
}
