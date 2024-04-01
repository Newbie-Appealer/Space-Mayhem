using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Install_Item : MonoBehaviour
{
    public bool _checkInstall;
    MeshRenderer _installMesh;
    [SerializeField] Material _redColor;
    [SerializeField] Material _greenColor;

    private void Start()
    {
        _checkInstall = true;
        _installMesh = GetComponent<MeshRenderer>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (_checkInstall)
        {
            _checkInstall = false;
            _installMesh.material = _redColor;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().material = _redColor;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _checkInstall = true;
        _installMesh.material = _greenColor;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().material = _greenColor;
        }
    }
}
