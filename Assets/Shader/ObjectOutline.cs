using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public enum OutlineMode
{
    OutlineAll,             // 안보이는 부분까지 외곽선
    OutlineVisible,         // 보이는 부분만 외곽선
    OutlineHidden,          // 안보이는부분만 외곽선
    OutlineAndSilhouette,   // 보이는부분 외곽선 / 안보이는부분 색칠
    SilhouetteOnly          // 안보이는부분 색칠
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

    [SerializeField] private OutlineMode _outlineMode;                  // 외곽선 모드
    [SerializeField] private Color _outlineColor = Color.white;         // 외곽선 색상
    [SerializeField, Range(0f, 10f)] private float _outlineWidth = 2f;  // 외곽선 두께

    // 외곽선 Material 2종
    private Material _outlineFillMaterial;
    private Material _outlineMaskMaterial;

    // 렌더러
    private Renderer[] _renderers;
    private void Awake()
    {
        // Renderer
        _renderers = GetComponentsInChildren<Renderer>();

        // 마테리얼 생성 및 초기화
        _outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));
        _outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));

        _outlineFillMaterial.name = "OutlineFill (Instance)";
        _outlineMaskMaterial.name = "OutlineMask (Instance)";

        // 외곽선 이쁘게 나오게 하기
        LoadSmoothNormals();

        // Material 초기화
        F_InitializeMaterial();
    }

    #region 활성/비활성/삭제
    // 스크립트 활성화 ( 외곽선 ON )
    private void OnEnable()
    {
        F_InitializeMaterial();
        foreach ( var renderer in _renderers )
        {
            // 1. renderer의 Material 을 List<material> 로 변환
            var materials = renderer.sharedMaterials.ToList();

            // 2. outlineMaterial을 추가
            materials.Add(_outlineFillMaterial);
            materials.Add(_outlineMaskMaterial);

            // 3. Array로 변환후 Renderer의 material로 적용
            renderer.materials = materials.ToArray();
        }
    }

    // 스크립트 비활성화 ( 외곽선 OFF )
    private void OnDisable()
    {
        foreach (var renderer in _renderers)
        {
            // 1. renderer의 Material 을 List<material> 로 변환
            var materials = renderer.sharedMaterials.ToList();

            // 2. outlineMaterial을 삭제
            materials.Remove(_outlineFillMaterial);
            materials.Remove(_outlineMaskMaterial);

            // 3. Array로 변환후 Renderer의 material로 적용
            renderer.materials = materials.ToArray();
        }
    }

    // 오브젝트 파괴 ( Material 삭제 )
    private void OnDestroy()
    {
        // Material 인스턴스를 삭제
        Destroy(_outlineMaskMaterial);
        Destroy(_outlineFillMaterial);
    }
    #endregion

    #region 노말 벡터 관련
    void LoadSmoothNormals()
    {
        // 하위의 모든 MeshFilter에 대해 반복
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            // 이미 등록된 매쉬인지 확인
            if (!_registeredMeshes.Add(meshFilter.sharedMesh))
                continue;


            // 노말 벡터를 검색 / 생성
            // 검색해서 값이 있다면 해당 값을 넣고
            // 없으면 생성해서 값을 넣음
            var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
            var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

            // 노발 벡터를 UV3에 저장
            meshFilter.sharedMesh.SetUVs(3, smoothNormals);

            // 서브 매쉬 결합
            var renderer = meshFilter.GetComponent<Renderer>();
            if (renderer != null)
                CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
        }

        // Skinned Mesh Renderer이 있으면 Skinned Mesh Renderer의 UV3를 지움
        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            // 이미 UV3가 지워졌을때
            if (!_registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                continue;

            // UV3 지움
            skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

            // 서브 매쉬 결합
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

    #region 마테리얼값 업데이트
    private void F_InitializeMaterial()
    {
        // 1. 컬러 및 두께 설정
        _outlineFillMaterial.SetColor("_OutlineColor", _outlineColor);
        _outlineFillMaterial.SetFloat("_OutlineWidth", _outlineWidth);

        // material의 렌더링설정
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