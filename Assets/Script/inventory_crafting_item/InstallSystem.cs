using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallSystem : MonoBehaviour
{
    [Header("object prefabs")]
    [SerializeField] GameObject[] _installPrefabs;
    [SerializeField] Material[] _material;
    [SerializeField] MeshRenderer _meshRenderer;
    LayerMask _floorlayer;

    private void Start()
    {
    }

    public void F_OnIsntallMode(int v_objectcode)
    {
        
    }
}
