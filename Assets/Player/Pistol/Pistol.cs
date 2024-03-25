using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("=== 작살 ===")]
    [SerializeField] private Spear _spear;
    [SerializeField] private float _spearFireSpeed;
    private Rigidbody _spear_rb;
    private Vector3 _spear_Firepos;
    private float _spear_Distance = 0.3f;

    private void Start()
    {
        _spear_rb = _spear.GetComponent<Rigidbody>();
        _spear_Firepos = _spear.transform.localPosition;
    }
    public void F_SpearPowerCharge()
    {
        _spearFireSpeed += Time.deltaTime * 2f;
        if (_spearFireSpeed > 4f )
        {
            _spearFireSpeed = 4f;
        }
    }
    
    public void F_SpearFire()
    {
        Debug.Log("작살 발사");
        _spear_rb.AddForce(transform.forward * _spearFireSpeed, ForceMode.Impulse);
    }

    public void F_SpearComeBack()
    {
        _spear.transform.localPosition = Vector3.Lerp(_spear.transform.localPosition, _spear_Firepos, _spearFireSpeed * Time.deltaTime);
        if (Vector3.Distance(_spear.transform.localPosition, _spear_Firepos) < _spear_Distance )
        {
            _spear_rb.velocity = Vector3.zero;
            _spear.transform.localPosition = _spear_Firepos;
            for (int l = 0; l< ScrapManager.Instance._scrapHitedSpear.Count; l++)
            {
                ScrapManager.Instance._scrapHitedSpear[l].GetComponent<Scrap>().F_GetScrap();
            }
            ScrapManager.Instance._scrapHitedSpear.Clear();
        }
    }
}
