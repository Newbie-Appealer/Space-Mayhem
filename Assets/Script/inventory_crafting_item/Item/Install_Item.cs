using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Install_Item : MonoBehaviour
{
    public bool _checkInstall;
    [SerializeField] Material _redColor;
    [SerializeField] Material _greenColor;
    private List<MeshRenderer> _installMesh;

    private void Start()
    {
        _checkInstall = true;
        _installMesh = new List<MeshRenderer>();
        _installMesh.Add(transform.GetComponent<MeshRenderer>());
        for (int i = 0; i < transform.childCount; i++)
        {
            _installMesh.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_checkInstall)
        {
            _checkInstall = false;

            for (int i = 0; i < _installMesh.Count; i++)
            {
                _installMesh[i].material = _redColor;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        F_ChgMaterial();
    }

    public void F_ChgMaterial()
    {
        if (!_checkInstall)
        {
            _checkInstall = true;

            for (int i = 0; i < _installMesh.Count; i++)
            {
                _installMesh[i].material = _greenColor;
            }
        }
    }
}
