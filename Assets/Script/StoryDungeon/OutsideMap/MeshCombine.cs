using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombine : MonoBehaviour
{
    // conbine 한 mesh 
    private List<Mesh> _combineMesh;
    private List<Material> _materialType;
    private Mesh finalMesh;

    public void F_MeshCombine(List<MeshRenderer> v_meshRen, List<MeshFilter> v_meshFil)
    {
        _combineMesh = new List<Mesh>();
        // meshRender : material 가져오기 용
        // meshFilder : mesh 가져오기 용

        // 1. Material 종류 담아놓기
        _materialType = new List<Material>();
        for (int i = 0; i < v_meshRen.Count; i++)
        {
            // 1-1. 배열로 하면 예외처리, list로 하면 상관없을듯 
            if (v_meshRen[i] == null)
                continue;

            // 1-2. material 배열에 없으면? -> 담기 
            if (!_materialType.Contains(v_meshRen[i].sharedMaterial))
                _materialType.Add(v_meshRen[i].sharedMaterial);
        }

        // (+) 찾은 material을 meshrenderer의 material에 넣기 / (+) material 다른 mesh 합치려면 해야함 , 현재는 x 
        /*
        MeshRenderer ren = OutsideMapManager.Instance._mapParent.GetComponent<MeshRenderer>();
        Material[] _myMaterial = ren.sharedMaterials;
        for (int i = 0; i < _myMaterial.Length; i++)
        {
            _myMaterial[i] = _materialType[i];
        }
        ren.sharedMaterials = _myMaterial;
        */

        // 2. 해당 material을 가진 오브젝트 끼리 combine
        for (int i = 0; i < _materialType.Count; i++)
        {
            // 2-1. i번째 material과 같은 CombineInstance 담아놓을 list
            List<CombineInstance> _conbineList = new List<CombineInstance>();
            for (int j = 0; j < v_meshRen.Count; j++)
            {
                if (_materialType[i] == v_meshRen[j].sharedMaterial)
                {
                    CombineInstance cbi = new CombineInstance();

                    cbi.mesh = v_meshFil[j].sharedMesh;
                    cbi.transform = v_meshFil[j].transform.localToWorldMatrix;
                    cbi.subMeshIndex = 0;   // 첫번째 하위 mesh만 결합되어야함 

                    _conbineList.Add(cbi);
                }
            }

            // 3. 합치기
            Mesh _combindCompleteMesh = new Mesh();
            _combindCompleteMesh.CombineMeshes(_conbineList.ToArray(), true);        // LINQ의 ToArray 
            _combineMesh.Add(_combindCompleteMesh);

            // 시각적으로 보기위한
            GameObject _meshObj = Instantiate(OutsideMapManager.Instance._applyMeshEnptyObject, OutsideMapManager.Instance._mapParent);
            _meshObj.transform.position = OutsideMapManager.Instance._Offset;
            _meshObj.GetComponent<MeshFilter>().sharedMesh = _combindCompleteMesh;
            _meshObj.GetComponent<MeshRenderer>().sharedMaterial = _materialType[i];


        }

        // 4. 마지막 합치기 
        /*
        List<CombineInstance> _finalConbine = new List<CombineInstance>();
        for (int i = 0; i < _combineMesh.Count; i++) 
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = _combineMesh[i];
            ci.transform = Matrix4x4.identity;
            ci.subMeshIndex = 0;
            _finalConbine.Add(ci);
        } 

        finalMesh = new Mesh();
        finalMesh.CombineMeshes(_finalConbine.ToArray() , false );

        GameObject _finalObj = OutsidemapManager.instance._mapParent.gameObject;
        _finalObj.name = "Final";
        _finalObj.GetComponent<MeshFilter>().sharedMesh = finalMesh;
  
        */

    }

}
