using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeshGenerator : MonoBehaviour
{
    [Header("Mesh 생성")]
    private List<Vector3> pointList;     // 전체 점 담아놓을 list
    private Vector3[] _verties;          // grid의 한 점에 대한 삼각형 vertex
    private Vector2[] _uv;               // uv
    private int[] _triangle;             // 삼각형 idx
    private int _width, _height;

    public List<Vector3> PointList { get => pointList; }

    private void Start()
    {
        pointList = new List<Vector3>( OutsideMapManager.Instance.heightXwidth);
    }

    public void F_CreateMeshMap( int v_width , int v_height , ref float[,] heightArr)
    {
        // 전체 점 위치 담아놓을 list
        //pointList = new List<Vector3>(v_width * v_height);  // 배열 가로x세로 만큼 생성
        pointList.Clear();

        // width, height 지정 
        _width = v_width;
        _height = v_height;

        // 점 list에 넣기
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                // 0. noiseMap의 height로
                pointList.Add(new Vector3(x, heightArr[x, y], y));
            }
        }

        // mesh 그리기 
        for (int y = 0; y < _height -1 ; y++)       // 마지막 한줄 안 
        {
            for (int x = 0; x < _width -1 ; x++)    // 마지막 한줄 안 함
            {
                if (y * _width + x >= _height * _width - 1 - _width)
                    return;

                // 매쉬 만들기 
                F_CreateMesh( x, y, heightArr[x, y]);
            }
        }

    }

    private void F_CreateMesh(int v_x , int v_y, float v_height) // 행, 열, 높이 
    {
        int _idx = v_y * _width + v_x;  // 인덱스 : y * 맵 너비 + x

        // 1. pool에서 mesh 꺼내기                                                                                                                
        GameObject _empty = OutsideMapManager.Instance.outsideMapPooling.F_GetMeshObject();
        // 2. 매쉬 생성
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

        // 3. 삼각형
        _triangle = new int[]   // verties 기준 몇번째 
        {
            0,2,1,
            1,2,3
        };

        mesh.vertices = _verties;           // mesh의 점 위치 
        mesh.uv = _uv;                      // textrue을 입히려면 사용해야함
        mesh.triangles = _triangle;         // 삼각형 그리기 , vertices의 index를 담아둠
        mesh.RecalculateNormals();          // 법선 다시 계산 (렌더링에 중요하다)

        // 1. Material 적용 ( MeshRenderer , Meshfilter은 gameobject에 부착하여 생성해야됨 )
        Material _mate = F_FindMaterial(v_height);
        _empty.GetComponent<MeshRenderer>().sharedMaterial = _mate;     // sharedMaterial 사용@!!

        // 2. 매쉬 적용 
        _empty.GetComponent<MeshFilter>().mesh = mesh;

        // 3. Outside의 meshren, meshfilter 배열에 저장
        OutsideMapManager.Instance.F_GetMeshRenMeshFil( v_x , v_y, _empty.GetComponent<MeshRenderer>(), _empty.GetComponent<MeshFilter>());

        // 4. pool에 mesh 넣기 
        OutsideMapManager.Instance.outsideMapPooling.F_ReturMeshObject(_empty , false);
    }

    private Material F_FindMaterial(float v_height)
    {
        Material _returnMaterial = OutsideMapManager.Instance._nowLandScape.F_GetMaterial(v_height);
        return _returnMaterial;
    }
}
