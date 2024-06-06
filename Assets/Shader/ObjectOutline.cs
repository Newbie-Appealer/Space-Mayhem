using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public enum OutlineMode
{
    OutlineAll,             // �Ⱥ��̴� �κб��� �ܰ���
    OutlineVisible,         // ���̴� �κи� �ܰ���
    OutlineHidden,          // �Ⱥ��̴ºκи� �ܰ���
    OutlineAndSilhouette,   // ���̴ºκ� �ܰ��� / �Ⱥ��̴ºκ� ��ĥ
    SilhouetteOnly          // �Ⱥ��̴ºκ� ��ĥ
}

[Serializable]
public class ListVector3
{
    public List<Vector3> data;
}

public class ObjectOutline : MonoBehaviour
{
    private HashSet<Mesh> _registeredMeshes = new HashSet<Mesh>();
    private List<Mesh> bakeKeys = new List<Mesh>();
    private List<ListVector3> bakeValues = new List<ListVector3>();

    [SerializeField] private OutlineMode _outlineMode;                  // �ܰ��� ���
    [SerializeField] private Color _outlineColor = Color.white;         // �ܰ��� ����
    [SerializeField, Range(0f, 10f)] private float _outlineWidth = 2f;  // �ܰ��� �β�

    // �ܰ��� Material 2��
    private Material _outlineFillMaterial;
    private Material _outlineMaskMaterial;

    // ������
    private Renderer[] _renderers;
    private void Awake()
    {
        // Renderer
        _renderers = GetComponentsInChildren<Renderer>();

        // ���׸��� ���� �� �ʱ�ȭ
        _outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));
        _outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));

        _outlineFillMaterial.name = "OutlineFill (Instance)";
        _outlineMaskMaterial.name = "OutlineMask (Instance)";

        // �ܰ��� �̻ڰ� ������ �ϱ�
        LoadSmoothNormals();

        // Material �ʱ�ȭ
        F_InitializeMaterial();
    }

    #region Ȱ��/��Ȱ��/����
    // ��ũ��Ʈ Ȱ��ȭ ( �ܰ��� ON )
    private void OnEnable()
    {
        F_InitializeMaterial();
        foreach ( var renderer in _renderers )
        {
            // 1. renderer�� Material �� List<material> �� ��ȯ
            var materials = renderer.sharedMaterials.ToList();

            // 2. outlineMaterial�� �߰�
            materials.Add(_outlineFillMaterial);
            materials.Add(_outlineMaskMaterial);

            // 3. Array�� ��ȯ�� Renderer�� material�� ����
            renderer.materials = materials.ToArray();
        }
    }

    // ��ũ��Ʈ ��Ȱ��ȭ ( �ܰ��� OFF )
    private void OnDisable()
    {
        foreach (var renderer in _renderers)
        {
            // 1. renderer�� Material �� List<material> �� ��ȯ
            var materials = renderer.sharedMaterials.ToList();

            // 2. outlineMaterial�� ����
            materials.Remove(_outlineFillMaterial);
            materials.Remove(_outlineMaskMaterial);

            // 3. Array�� ��ȯ�� Renderer�� material�� ����
            renderer.materials = materials.ToArray();
        }
    }

    // ������Ʈ �ı� ( Material ���� )
    private void OnDestroy()
    {
        // Material �ν��Ͻ��� ����
        Destroy(_outlineMaskMaterial);
        Destroy(_outlineFillMaterial);
    }
    #endregion

    #region �븻 ���� ����
    void LoadSmoothNormals()
    {
        // ������ ��� MeshFilter�� ���� �ݺ�
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            // �̹� ��ϵ� �Ž����� Ȯ��
            if (!_registeredMeshes.Add(meshFilter.sharedMesh))
                continue;


            // �븻 ���͸� �˻� / ����
            // �˻��ؼ� ���� �ִٸ� �ش� ���� �ְ�
            // ������ �����ؼ� ���� ����
            var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
            var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

            // ��� ���͸� UV3�� ����
            meshFilter.sharedMesh.SetUVs(3, smoothNormals);

            // ���� �Ž� ����
            var renderer = meshFilter.GetComponent<Renderer>();
            if (renderer != null)
                CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
        }

        // Skinned Mesh Renderer�� ������ Skinned Mesh Renderer�� UV3�� ����
        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            // �̹� UV3�� ����������
            if (!_registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                continue;

            // UV3 ����
            skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

            // ���� �Ž� ����
            CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
        }
    }

    List<Vector3> SmoothNormals(Mesh mesh)
    {

        // Group vertices by location
        var groups = mesh.vertices.Select((vertex, index) 
            => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

        // Copy normals to a new list
        var smoothNormals = new List<Vector3>(mesh.normals);

        // Average normals for grouped vertices
        foreach (var group in groups)
        {

            // Skip single vertices
            if (group.Count() == 1)
            {
                continue;
            }

            // Calculate the average normal
            var smoothNormal = Vector3.zero;

            foreach (var pair in group)
            {
                smoothNormal += smoothNormals[pair.Value];
            }

            smoothNormal.Normalize();

            // Assign smooth normal to each vertex
            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }

        return smoothNormals;
    }

    void CombineSubmeshes(Mesh mesh, Material[] materials)
    {
        if (mesh.subMeshCount == 1)
            return;

        if (mesh.subMeshCount > materials.Length)
            return;

        mesh.subMeshCount++;
        mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }
    #endregion

    #region ���׸��� ������Ʈ
    private void F_InitializeMaterial()
    {
        // 1. �÷� �� �β� ����
        _outlineFillMaterial.SetColor("_OutlineColor", _outlineColor);
        _outlineFillMaterial.SetFloat("_OutlineWidth", _outlineWidth);

        // material�� ����������
        switch (_outlineMode)
        {
            case OutlineMode.OutlineAll:
                _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                break;

            case OutlineMode.OutlineVisible:
                _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                break;

            case OutlineMode.OutlineHidden:
                _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                break;

            case OutlineMode.OutlineAndSilhouette:
                _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                break;

            case OutlineMode.SilhouetteOnly:
                _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                _outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
                break;
        }
    }
    #endregion
}