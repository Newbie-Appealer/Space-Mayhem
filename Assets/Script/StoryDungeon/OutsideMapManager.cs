using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum PlanetType 
{
    Earth,          // ����
    Lavenderia,     // ����� �Ĺ�
    Crimsonium,     // ��ũ�� ����
    Junkar,         // ������
    Silvantis,      // �Ƹ���
    Fractonia,      // ����
    Lithosia,       // ��
    Psammoria,      // �縷
    Hydroros,       // �ٴ� 
    Cryolithe       // ����
}

public class OutsideMapManager : Singleton<OutsideMapManager>
{
    [Header("======Container======")]
    private float[,] _concludeMapArr;
    private MeshRenderer[,] _meshRenderers;     // mesh�� material ���� ���� 
    private MeshFilter[,] _meshFilters;         // mesh ���� ����    

    [Header("======GameObject======")]
    public Transform _mapParent;                    // ���� ���� map�� �θ� 
    public Vector3 _Offset;                         // OusideMap�� ������ ��ġ

    [Header("=====WIDTH, HEIGHT=====")]
    public const int mapMaxHeight = 100;
    public const int mapMaxWidth = 100;
    public int heightXwidth => mapMaxHeight * mapMaxWidth;

    [Header("======Sacle======")]
    [SerializeField] private int _mapWidth;       // �� �ʺ�
    [SerializeField] private int _mapHeight;      // �� ����
    [SerializeField] private float _noiseScale;   // ������ ũ�� 
    [SerializeField] private float _devigation;     // height ���̸� �󸶳� �ø����ΰ�?

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

    // �ʱ� ����
    public void F_InitOutsideMap()
    {
        // 0. �ʱ⼱�� 
        _concludeMapArr = new float[mapMaxWidth, mapMaxHeight];
        _meshRenderers = new MeshRenderer[mapMaxWidth, mapMaxHeight];
        _meshFilters = new MeshFilter[mapMaxWidth, mapMaxHeight];
    }

    // ## TODO 
    // �ٸ���ũ��Ʈ���� ���

    public void F_CreateOutsideMap()
    {
        // 0. �ʱ�ȭ, (�迭, ó���ε���, ���� �������� ����) 
        System.Array.Clear(_concludeMapArr, 0, _concludeMapArr.Length);       // 0 
        System.Array.Clear(_meshRenderers, 0, _meshRenderers.Length);        // null
        System.Array.Clear(_meshFilters, 0, _meshFilters.Length);            // null

        // 1. landScape
        F_InitLandScape();      // ���� landScape ���� 

        // 2. �� ���� ����
        // _conclude = Generatemap()�ϸ� conclude�� �����ϰ��ִ� �޸𸮰� �ս� 
        mapHeightGenerate.GenerateMap( ref _concludeMapArr, _mapWidth, _mapHeight, seed, _noiseScale, octaves, persistance, lacunerity, _devigation);

        // 3. �Ž� ����
        meshGenerator.F_CreateMeshMap( _mapWidth , _mapHeight, ref _concludeMapArr);

        // 4. �ݶ��̴� ����
        colliderGenerator.F_CreateCollider(_mapWidth, _mapHeight, meshGenerator.PointList);

        // 5. �Ž� ��ġ��
        meshCombine.F_MeshCombine(
            _mapWidth , _mapHeight , _meshRenderers , _meshFilters);
        
        // ���� �� �ϰ� �������� �迭, list�� �� �޸� ���� ��Ű�� 
    }

    // ## TODO
    // �༺ Ż�� ��
    public void F_ExitOutsideMap()
    {
        // 0. �ݶ��̴� pool�� �ǵ����� 
        outsideMapPooling.F_ReturnColliderObject();

        // 1. mapParent ������ mesh�� pool�� �ֱ� 
        while (_mapParent.childCount != 0)
        {
            MeshFilter me = _mapParent.GetChild(0).GetComponent<MeshFilter>();
            me.mesh = null;

            outsideMapPooling.F_ReturMeshObject(me.gameObject, true);
        }
    }


    // landScape ���� 
    private void F_InitLandScape()
    {
        // 1. landScape ���� ����
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
