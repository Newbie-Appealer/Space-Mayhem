using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombine : MonoBehaviour
{
    // conbine �� mesh 
    private List<Mesh> _combineMesh;
    private List<Material> _materialType;
    private Mesh finalMesh;

    public void F_MeshCombine(int v_width , int v_height , MeshRenderer[,] v_meshRen, MeshFilter[,] v_meshfilter )
    {

        _combineMesh = new List<Mesh>();
        // meshRender : material �������� ��
        // meshFilder : mesh �������� ��

        // 1. Material ���� ��Ƴ���
        _materialType = new List<Material>();
        for (int y = 0; y < v_height-1; y++) 
        {
            for(int x = 0; x < v_width-1; x++) 
            {
                // ���� ó�� 
                if (v_meshRen[ x, y ] == null)
                    continue;

                // 1-2. material �迭�� ������? -> ��� 
                if (!_materialType.Contains(v_meshRen[x, y].sharedMaterial))
                    _materialType.Add(v_meshRen[x, y].sharedMaterial);
            }
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

            // 2-2. ���� x �ʺ�ŭ 
            for (int y = 0; y < v_height - 1; y++)
            {
                for (int x = 0; x < v_width - 1; x++)
                {
                    // ����ó�� 
                    if (v_meshRen[x, y] == null)
                        continue;

                    // 2-3. ���� material �˻��ؼ� ���� 
                    if (_materialType[i] == v_meshRen[x, y].sharedMaterial)
                    {
                        CombineInstance cbi = new CombineInstance();

                        cbi.mesh = v_meshfilter[x, y].sharedMesh;
                        cbi.transform = v_meshfilter[x, y].transform.localToWorldMatrix;
                        cbi.subMeshIndex = 0;   // ù��° ���� mesh�� ���յǾ���� 

                        _conbineList.Add(cbi);
                    }
                }
            }

            // 3. ��ġ��
            Mesh _combindCompleteMesh = new Mesh();
            _combindCompleteMesh.CombineMeshes(_conbineList.ToArray(), true);        // LINQ�� ToArray 
            _combineMesh.Add(_combindCompleteMesh);

            // �ð������� ��������
            GameObject _meshObj = OutsideMapManager.Instance.outsideMapPooling.F_GetMeshObject();
            _meshObj.transform.parent = OutsideMapManager.Instance.mapParent;
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
