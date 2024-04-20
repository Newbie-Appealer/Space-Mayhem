using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Install_Item : MonoBehaviour
{
    [SerializeField] Material _redColor;
    [SerializeField] Material _greenColor;
    private List<MeshRenderer> _installMesh;
    public bool _checkInstall;

    private void Start()
    {
        _checkInstall = true;
        _installMesh = new List<MeshRenderer>();
        _installMesh.Add(transform.GetComponent<MeshRenderer>());
        for (int i = 0; i < transform.childCount; i++)
        {
            // meshRenderer 없는 오브젝트에 대한 예외처리 추가했음 ( - 재민 )
            MeshRenderer mesh = transform.GetChild(i).GetComponent<MeshRenderer>();

            if(mesh != null)            // GetCompnent했을때 값이 null이 아닐때 ( component가 없으면 null이 들어옴 )
                _installMesh.Add(mesh);

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
