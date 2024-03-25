using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private Rigidbody _rb;
    public float _spearFireSpeed = 0f;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Scrap"))
        {
            Debug.Log("�Ĺ� ����");
        }
    }

    public void F_SpearFire()
    {
        _rb.AddForce(transform.forward * _spearFireSpeed, ForceMode.Impulse);
    }
}
