using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [Header("width * height ��ŭ �迭")]
    private Mesh[,] _meshArr;

    [Header("Mesh ����")]
    private List<Vector3> pointList;     // ��ü �� ��Ƴ��� list
    private Vector3[] _verties;          // grid�� �� ���� ���� �ﰢ�� vertex
    private Vector2[] _uv;               // uv
    private int[] _triangle;             // �ﰢ�� idx
    private int _width, _height;

    public List<Vector3> PointList { get => pointList; }

    public void F_CreateMeshMap(ref float[,] heightArr)
    {
        // ��ü �� ��ġ ��Ƴ��� list
        pointList = new List<Vector3>(heightArr.GetLength(0) * heightArr.GetLength(1));  // �迭 ����x���� ��ŭ ����

        // width, height ���� 
        _width = heightArr.GetLength(0);
        _height = heightArr.GetLength(1);

        // �� list�� �ֱ�
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                // 0. noiseMap�� height��
                pointList.Add(new Vector3(x, heightArr[x, y], y));
            }
        }

        // mesh �׸��� 
        for (int y = 0; y < _height - 1; y++)       // ������ ���� �� ��
        {
            for (int x = 0; x < _width - 1; x++)    // ������ ���� �� ��
            {
                // �Ž� ����� 
                F_CreateMesh(y, x, heightArr[x, y]);
            }
        }

    }

    private void F_CreateMesh(int v_y, int v_x, float v_height) // ���� �ε��� , ����
    {
        int _idx = v_y * _height + v_x;  // �ε��� : y * �ʳ��� + x

        // �� ������Ʈ ����                                                                                                                  
        GameObject _empty = Instantiate(OutsideMapManager.Instance._applyMeshEnptyObject, OutsideMapManager.Instance._mapParent);
        // �Ž� ����
        Mesh mesh = new Mesh();

        // 1. vertex
        _verties = new Vector3[]
        {
            pointList[_idx],
            pointList[_idx + 1],
            pointList[_idx + _width ],
            pointList[_idx + _width + 1]
        };

        // 2. uv
        _uv = new Vector2[]
        {
            new Vector2 (0, 0),
            new Vector2 (0, 1),
            new Vector2 (1, 1),
            new Vector2 (1, 0)
        };

        // 3. �ﰢ��
        _triangle = new int[]   // verties ���� ���° 
        {
            0,2,1,
            1,2,3
        };

        mesh.vertices = _verties;           // mesh�� �� ��ġ 
        mesh.uv = _uv;                      // textrue�� �������� ����ؾ���
        mesh.triangles = _triangle;         // �ﰢ�� �׸��� , vertices�� index�� ��Ƶ�
        mesh.RecalculateNormals();          // ���� �ٽ� ��� (�������� �߿��ϴ�)

        // 1. Material ���� ( MeshRenderer , Meshfilter�� gameobject�� �����Ͽ� �����ؾߵ� )
        Material _mate = F_FindMaterial(v_height);
        _empty.GetComponent<MeshRenderer>().sharedMaterial = _mate;     // sharedMaterial ���@!!

        // 2. �Ž� ���� 
        _empty.GetComponent<MeshFilter>().mesh = mesh;

        // 3. Outside�� meshren, meshfilter �迭�� ����
        OutsideMapManager.Instance.F_GetMeshRenMeshFil(v_y, v_x, _empty.GetComponent<MeshRenderer>(), _empty.GetComponent<MeshFilter>());

        Destroy(_empty);
    }

    private Material F_FindMaterial(float v_height)
    {
        Material _returnMaterial = OutsideMapManager.Instance._nowLandScape.F_GetMaterial(v_height);
        return _returnMaterial;
    }
}