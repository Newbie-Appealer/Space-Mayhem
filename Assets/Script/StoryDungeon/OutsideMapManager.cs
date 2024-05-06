using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutsideMapManager : Singleton<OutsideMapManager>
{
    [Header("======Container======")]
    private float[,] _concludeMapArr;
    private MeshRenderer[,] _meshRenderers;     // mesh�� ������ ���� ( width - 1, height - 1 �� ���� ���� )
    private MeshFilter[,] _meshFilters;         // mesh ���� ����    

    [Header("======GameObject======")]
    public Transform _mapParent;
    public Transform _colliderParent;
    public GameObject _applyMeshEnptyObject;         // mesh �����ų �� ������Ʈ 
    public Vector3 _Offset;                          // OusideMap�� ������ ��ġ

    [Header("======Sacle======")]
    [SerializeField] private int _mapWidth;       // �� �ʺ�
    [SerializeField] private int _mapHeight;      // �� ����
    [SerializeField] private float _noiseScale;   // ������ ũ�� 
    [SerializeField] private float _devigation;     // �ܰ����̰� �󸶳� �������ΰ�? 

    [Header("======octaves======")]
    [SerializeField] private int seed;            // �õ�
    [SerializeField] private int octaves;         // ��Ÿ��
    [SerializeField] private float persistance;   // ����(amplitude) ũ�� (�󸶳� ��~�� ����) ����
    [SerializeField] private float lacunerity;    // ��(Frequency) �� �� ����

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
    // �ٸ���ũ��Ʈ���� ���
    public void F_CreateOutsideMap()
    {
        // 0. �ʱ�ȭ
        _concludeMapArr = new float[_mapWidth, _mapHeight];
        _meshRenderers = new MeshRenderer[_mapWidth - 1, _mapHeight - 1];
        _meshFilters = new MeshFilter[_mapWidth - 1, _mapHeight - 1];

        // 1. landScape
        F_InitLandScape();      // ���� landScape ���� 

        // 2. �� ���� ����
        _concludeMapArr = mapHeightGenerate.GenerateMap(_mapWidth, _mapHeight, seed, _noiseScale, octaves, persistance, lacunerity, _devigation);

        // 3. �Ž� ����
        meshGenerator.F_CreateMeshMap(ref _concludeMapArr);

        // 4. �ݶ��̴� ����
        colliderGenerator.F_CreateCollider(_mapWidth, _mapHeight, meshGenerator.PointList);

        // 5. �Ž� ��ġ��
        meshCombine.F_MeshCombine(
            _meshRenderers.Cast<MeshRenderer>().ToList(), _meshFilters.Cast<MeshFilter>().ToList());

        // ##TODO
        // ���� �� �ϰ� �������� �迭, list�� �� �޸� ���� ��Ű�� 
    }


    // landScape ���� 
    private void F_InitLandScape()
    {
        // 1. landScape ���� ����
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
