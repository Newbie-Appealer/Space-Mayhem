using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum PlanetType 
{
    Earth,          // 지구
    Lavenderia,     // 보라색 식물
    Crimsonium,     // 핑크색 종말
    Junkar,         // 쓰레기
    Silvantis,      // 아마존
    Fractonia,      // 단층
    Lithosia,       // 돌
    Psammoria,      // 사막
    Hydroros,       // 바다 
    Cryolithe       // 얼음
}

public class OutsideMapManager : Singleton<OutsideMapManager>
{
    [Header("======Container======")]
    private float[,] _concludeMapArr;
    private MeshRenderer[,] _meshRenderers;     // mesh의 material 접근 위한 
    private MeshFilter[,] _meshFilters;         // mesh 적용 위한    

    [Header("======GameObject======")]
    public Transform _mapParent;                    // 최종 생성 map의 부모 
    public Vector3 _Offset;                         // OusideMap이 생성될 위치

    [Header("=====WIDTH, HEIGHT=====")]
    public const int mapMaxHeight = 100;
    public const int mapMaxWidth = 100;
    public int heightXwidth => mapMaxHeight * mapMaxWidth;

    [Header("======Sacle======")]
    [SerializeField] private int _mapWidth;       // 맵 너비
    [SerializeField] private int _mapHeight;      // 맵 길이
    [SerializeField] private float _noiseScale;   // 노이즈 크기 
    [SerializeField] private float _devigation;     // height 높이를 얼마나 올릴것인가?

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

    [Header("====Pooling====")]
    public OutsideMapPooling outsideMapPooling;

    [Header("======Material======")]
    public List<Material> _mateialList;
    public Material _defultMaterial;

    protected override void InitManager()
    {
        F_InitOutsideMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            F_CreateOutsideMap();

        if (Input.GetKeyDown(KeyCode.L))
            F_ExitOutsideMap();
    }

    // 초기 선언
    public void F_InitOutsideMap()
    {
        // 0. 초기선언 
        _concludeMapArr = new float[mapMaxWidth, mapMaxHeight];
        _meshRenderers = new MeshRenderer[mapMaxWidth, mapMaxHeight];
        _meshFilters = new MeshFilter[mapMaxWidth, mapMaxHeight];
    }

    // ## TODO 
    // 다른스크립트에서 사용

    public void F_CreateOutsideMap()
    {
        // 0. 초기화, (배열, 처음인덱스, 지울 데이터의 갯수) 
        System.Array.Clear(_concludeMapArr, 0, _concludeMapArr.Length);       // 0 
        System.Array.Clear(_meshRenderers, 0, _meshRenderers.Length);        // null
        System.Array.Clear(_meshFilters, 0, _meshFilters.Length);            // null

        // 1. landScape
        F_InitLandScape();      // 현재 landScape 지정 

        // 2. 맵 높이 지정
        // _conclude = Generatemap()하면 conclude가 참조하고있는 메모리가 손실 
        mapHeightGenerate.GenerateMap( ref _concludeMapArr, _mapWidth, _mapHeight, seed, _noiseScale, octaves, persistance, lacunerity, _devigation);

        // 3. 매쉬 생성
        meshGenerator.F_CreateMeshMap( _mapWidth , _mapHeight, ref _concludeMapArr);

        // 4. 콜라이더 생성
        colliderGenerator.F_CreateCollider(_mapWidth, _mapHeight, meshGenerator.PointList);

        // 5. 매쉬 합치기
        meshCombine.F_MeshCombine(
            _mapWidth , _mapHeight , _meshRenderers , _meshFilters);
        
        // 생성 다 하고 만들어놓은 배열, list등 다 메모리 해제 시키기 
    }

    // ## TODO
    // 행성 탈출 시
    public void F_ExitOutsideMap()
    {
        // 0. 콜라이더 pool로 되돌리기 
        outsideMapPooling.F_ReturnColliderObject();

        // 1. mapParent 하위의 mesh도 pool에 넣기 
        while (_mapParent.childCount != 0)
        {
            MeshFilter me = _mapParent.GetChild(0).GetComponent<MeshFilter>();
            me.mesh = null;

            outsideMapPooling.F_ReturMeshObject(me.gameObject, true);
        }
    }


    // landScape 생성 
    private void F_InitLandScape()
    {
        // 1. landScape 새로 생성
        List<Tuple<float, Material>> _list = new List<Tuple<float, Material>>()
        {
            new Tuple<float, Material>( 2f , _mateialList[0]),
            new Tuple<float, Material>( 4f , _mateialList[1]),
            new Tuple<float, Material>( 6f , _mateialList[2]),
            new Tuple<float, Material>( 8f , _mateialList[3]),
            new Tuple<float, Material>( 10f , _mateialList[4]),
            new Tuple<float, Material>( 12f , _mateialList[5]),
            new Tuple<float, Material>( 14f , _mateialList[6]),
            new Tuple<float, Material>( 16f , _mateialList[7]),
        };

        _nowLandScape = new LandScape(_list);
    }

    public void F_GetMeshRenMeshFil(int v_y, int v_x, MeshRenderer v_ren, MeshFilter v_fil)
    {
        _meshRenderers[v_y, v_x] = v_ren;
        _meshFilters[v_y, v_x] = v_fil;
    }


}
