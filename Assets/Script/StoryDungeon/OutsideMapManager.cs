using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutsideMapManager : Singleton<OutsideMapManager>
{
    [Header("======Container======")]
    private float[,] _concludeMapArr;
    private MeshRenderer[,] _meshRenderers;     // mesh의 렌더링 위한 ( width - 1, height - 1 의 값을 가짐 )
    private MeshFilter[,] _meshFilters;         // mesh 적용 위한    

    [Header("======GameObject======")]
    public Transform _mapParent;
    public Transform _colliderParent;
    public GameObject _applyMeshEnptyObject;         // mesh 적용시킬 빈 오브젝트 
    public Vector3 _Offset;                          // OusideMap이 생성될 위치

    [Header("======Sacle======")]
    [SerializeField] private int _mapWidth;       // 맵 너비
    [SerializeField] private int _mapHeight;      // 맵 길이
    [SerializeField] private float _noiseScale;   // 노이즈 크기 
    [SerializeField] private float _devigation;     // 외각높이가 얼마나 높을것인가? 

    [Header("======octaves======")]
    [SerializeField] private int seed;            // 시드
    [SerializeField] private int octaves;         // 옥타브
    [SerializeField] private float persistance;   // 진폭(amplitude) 크기 (얼마나 낮~높 을지) 결정
    [SerializeField] private float lacunerity;    // 빈도(Frequency) 의 폭 결정

    [Header("======Script======")]
    public MapHeightGenerator mapHeightGenerate;
    public MeshGenerator meshGenerator;
    public MeshCombine meshCombine;
    public ColliderGenerator colliderGenerator;
    public LandScape _nowLandScape;

    [Header("======Material======")]
    public List<Material> _mateialList;
    public Material _defultMaterial;

    protected override void InitManager()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            F_CreateOutsideMap();
    }

    // ## TODO 
    // 다른스크립트에서 사용
    public void F_CreateOutsideMap()
    {
        // 0. 초기화
        _concludeMapArr = new float[_mapWidth, _mapHeight];
        _meshRenderers = new MeshRenderer[_mapWidth - 1, _mapHeight - 1];
        _meshFilters = new MeshFilter[_mapWidth - 1, _mapHeight - 1];

        // 1. landScape
        F_InitLandScape();      // 현재 landScape 지정 

        // 2. 맵 높이 지정
        _concludeMapArr = mapHeightGenerate.GenerateMap(_mapWidth, _mapHeight, seed, _noiseScale, octaves, persistance, lacunerity, _devigation);

        // 3. 매쉬 생성
        meshGenerator.F_CreateMeshMap(ref _concludeMapArr);

        // 4. 콜라이더 생성
        colliderGenerator.F_CreateCollider(_mapWidth, _mapHeight, meshGenerator.PointList);

        // 5. 매쉬 합치기
        meshCombine.F_MeshCombine(
            _meshRenderers.Cast<MeshRenderer>().ToList(), _meshFilters.Cast<MeshFilter>().ToList());

        // ##TODO
        // 생성 다 하고 만들어놓은 배열, list등 다 메모리 해제 시키기 
    }


    // landScape 생성 
    private void F_InitLandScape()
    {
        // 1. landScape 새로 생성
        List<Tuple<float, Material>> _list = new List<Tuple<float, Material>>
        {
            new Tuple<float, Material>( 2f , _mateialList[0]),
            new Tuple<float, Material>( 4f , _mateialList[1]),
            new Tuple<float, Material>( 6f , _mateialList[2]),
            new Tuple<float, Material>( 8f , _mateialList[3])
        };

        _nowLandScape = new LandScape(_list);
    }

    public void F_GetMeshRenMeshFil(int v_y, int v_x, MeshRenderer v_ren, MeshFilter v_fil)
    {
        _meshRenderers[v_y, v_x] = v_ren;
        _meshFilters[v_y, v_x] = v_fil;
    }


}
