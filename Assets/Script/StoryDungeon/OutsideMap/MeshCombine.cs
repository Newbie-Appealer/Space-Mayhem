using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombine : MonoBehaviour
{
    // conbine �� mesh 
    private List<Mesh> _combineMesh;
    private List<Material> _materialType;
    private Mesh finalMesh;

    public void F_MeshCombine(List<MeshRenderer> v_meshRen, List<MeshFilter> v_meshFil)
    {
        _combineMesh = new List<Mesh>();
        // meshRender : material �������� ��
        // meshFilder : mesh �������� ��

        // 1. Material ���� ��Ƴ���
        _materialType = new List<Material>();
        for (int i = 0; i < v_meshRen.Count; i++)
        {
            // 1-1. �迭�� �ϸ� ����ó��, list�� �ϸ� ��������� 
            if (v_meshRen[i] == null)
                continue;

            // 1-2. material �迭�� ������? -> ��� 
            if (!_materialType.Contains(v_meshRen[i].sharedMaterial))
                _materialType.Add(v_meshRen[i].sharedMaterial);
        }

        // (+) ã�� material�� meshrenderer�� material�� �ֱ� / (+) material �ٸ� mesh ��ġ���� �ؾ��� , ����� x 
        /*
        MeshRenderer ren = OutsideMapManager.Instance._mapParent.GetComponent<MeshRenderer>();
        Material[] _myMaterial = ren.sharedMaterials;
        for (int i = 0; i < _myMaterial.Length; i++)
        {
            _myMaterial[i] = _materialType[i];
        }
        ren.sharedMaterials = _myMaterial;
        */

        // 2. �ش� material�� ���� ������Ʈ ���� combine
        for (int i = 0; i < _materialType.Count; i++)
        {
            // 2-1. i��° material�� ���� CombineInstance ��Ƴ��� list
            List<CombineInstance> _conbineList = new List<CombineInstance>();
            for (int j = 0; j < v_meshRen.Count; j++)
            {
                if (_materialType[i] == v_meshRen[j].sharedMaterial)
                {
                    CombineInstance cbi = new CombineInstance();

                    cbi.mesh = v_meshFil[j].sharedMesh;
                    cbi.transform = v_meshFil[j].transform.localToWorldMatrix;
                    cbi.subMeshIndex = 0;   // ù��° ���� mesh�� ���յǾ���� 

                    _conbineList.Add(cbi);
                }
            }

            // 3. ��ġ��
            Mesh _combindCompleteMesh = new Mesh();
            _combindCompleteMesh.CombineMeshes(_conbineList.ToArray(), true);        // LINQ�� ToArray 
            _combineMesh.Add(_combindCompleteMesh);

            // �ð������� ��������
            GameObject _meshObj = Instantiate(OutsideMapManager.Instance._applyMeshEnptyObject, OutsideMapManager.Instance._mapParent);
            _meshObj.transform.position = OutsideMapManager.Instance._Offset;
            _meshObj.GetComponent<MeshFilter>().sharedMesh = _combindCompleteMesh;
            _meshObj.GetComponent<MeshRenderer>().sharedMaterial = _materialType[i];


        }

        // 4. ������ ��ġ�� 
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
