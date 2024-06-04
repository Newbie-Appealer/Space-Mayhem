using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Install_Item : MonoBehaviour
{
    public LayerMask _installLayer;             // ��ġ ������ ��� ���̾�

    [SerializeField] Material _redColor;
    [SerializeField] Material _greenColor;
    private List<MeshRenderer> _installMesh;

    private bool _checkInstall;
    public bool checkInstall => _checkInstall;

    private void Start()
    {
        _checkInstall = true;
        _installMesh = new List<MeshRenderer>();
        
        // MeshRenderer�� ���� Ȯ�� �� �߰�
        MeshRenderer parentMesh = transform.GetComponent<MeshRenderer>();
        if(parentMesh != null)
            _installMesh.Add(parentMesh);

        // MeshRenderer�� ���� Ȯ�� �� �߰�
        for (int i = 0; i < transform.childCount; i++)
        {
            MeshRenderer mesh = transform.GetChild(i).GetComponent<MeshRenderer>();

            if (mesh != null)
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
