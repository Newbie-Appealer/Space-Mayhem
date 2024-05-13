using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

using Random = UnityEngine.Random;

public enum PlanetType 
{
    Earth,          // ����
    Lavenderia,     // ����� �Ĺ�
    Crimsonium,     // ��ũ�� ����
    Junkar,         // ������
    Silvantis,      // �Ƹ���
    Fractonia,      // ����
    Lithosia,       // ��
    Floralia,       // ��
    Hydroros,       // �ٴ� 
    Cryolithe,      // ����
    Potaterra       // ����
}

public class OutsideMapManager : Singleton<OutsideMapManager>
{
    [Header("=====Curr LandScpae====")]
    [SerializeField] LandScape[] _landSacpeArr;     // landScape ��Ƴ��� �迭
    [SerializeField] LandScape _nowLandScape;       // ���� landScape
    [SerializeField] private int _PlanetSeed;       // ���� seed 

    [Header("======Container======")]
    private float[,] _concludeMapArr;
    private MeshRenderer[,] _meshRenderers;     // mesh�� material ���� ���� 
    private MeshFilter[,] _meshFilters;         // mesh ���� ����    
    private bool[,] _arrangeObjectArr;          // ������Ʈ ��ġ bool

    [Header("======GameObject======")]
    public Transform _mapParent;                    // ���� ���� map�� �θ� 
    public Vector3 _Offset;                         // OusideMap�� ������ ��ġ

    [Header("=====WIDTH, HEIGHT=====")]
    private const int mapMaxHeight = 100;
    private const int mapMaxWidth = 100;
    [SerializeField]  private int _nowWidth;           // ���� width
    [SerializeField]  private int _nowHeight;          // ���� height 
    public int heightXwidth => mapMaxHeight * mapMaxWidth;

    [Header("======Script======")]
    public MapHeightGenerator mapHeightGenerate;
    public MeshGenerator meshGenerator;
    public MeshCombine meshCombine;
    public ColliderGenerator colliderGenerator;
    public OutsideMapDataManager mapDataManager;

    [Header("====Pooling====")]
    public OutsideMapPooling outsideMapPooling;

    // ������Ƽ
    public LandScape nowLandScape => _nowLandScape;

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
        // 0. ���������̺� ��ũ��Ʈ ����
        mapDataManager = new OutsideMapDataManager();

        // 1. Ÿ�Ը�ŭ arr ����
        _landSacpeArr = new LandScape[System.Enum.GetValues(typeof(PlanetType)).Length];

        // 2. ���������̺� get
        mapDataManager.F_InitOutsidemapData();

        // 0. �ʱ⼱�� 
        _concludeMapArr = new float[mapMaxWidth, mapMaxHeight];
        _meshRenderers  = new MeshRenderer[mapMaxWidth, mapMaxHeight];
        _meshFilters    = new MeshFilter[mapMaxWidth, mapMaxHeight];
        _arrangeObjectArr     = new bool[mapMaxWidth, mapMaxHeight];

        // 3. seed ����
        _PlanetSeed = 0;

    }

    // type�� ���� landScape�� arr�� ����
    public void F_InsertLandSacpeArr( string v_type , LandScape v_land ) 
    {
        // Ÿ�Կ� �´� index�� land �ֱ�  
        PlanetType type = (PlanetType)Enum.Parse(typeof(PlanetType), v_type);
        _landSacpeArr[(int)type] = v_land;
    }

    // ## TODO 
    // �ٸ���ũ��Ʈ���� ���

    public void F_CreateOutsideMap()
    {
        // 0. �ʱ�ȭ, (�迭, ó���ε���, ���� �������� ����) 
        System.Array.Clear(_concludeMapArr, 0, _concludeMapArr.Length);       // 0 
        System.Array.Clear(_meshRenderers, 0, _meshRenderers.Length);         // null
        System.Array.Clear(_meshFilters, 0, _meshFilters.Length);             // null
        System.Array.Clear(_arrangeObjectArr, 0 , _arrangeObjectArr.Length ); // false    

        // 1. ���� landScape
        _nowLandScape = F_InitLandScape();

        // 2. ���� landScape�� ���� ���� width, height ���ϱ�
        _nowWidth = Random.Range( _nowLandScape.minWidth , _nowLandScape.maxWidth );
        _nowHeight = Random.Range( _nowLandScape.minHeight , _nowLandScape.maxHeight );

        // 2. �� ���� ����
        // _conclude = Generatemap()�ϸ� conclude�� �����ϰ��ִ� �޸𸮰� �ս� 
        mapHeightGenerate.GenerateMap( ref _concludeMapArr, _nowWidth, _nowHeight, _PlanetSeed, 
            _nowLandScape.noiseScale, _nowLandScape.octave, _nowLandScape.persistance, _nowLandScape.lacunerity, _nowLandScape.devigation);

        // 2-1. seed ���� 
        _PlanetSeed++;

        // 3. �Ž� ����
        meshGenerator.F_CreateMeshMap(_nowWidth, _nowHeight, ref _concludeMapArr);

        // 4. �ݶ��̴� ����
        colliderGenerator.F_CreateCollider(_nowWidth, _nowHeight, meshGenerator.PointList);

        // 5. �Ž� ��ġ��
        meshCombine.F_MeshCombine(
            _nowWidth, _nowHeight , _meshRenderers , _meshFilters);

        // 6. �༺ ������� ��ġ
        F_arrangePlanetObject();
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

        // 2. map object �ٽ� ����ֱ� 
        outsideMapPooling.F_ReturnPlanetObject( (int)_nowLandScape.planetType );
    }


    // �ӽ� : landScape ���� 
    private LandScape F_InitLandScape()
    {
        // 1. Planet enum�� ���� �ε����� ���ϱ�
        int _rand = Random.Range( 0, System.Enum.GetValues(typeof(PlanetType)).Length );

        return _landSacpeArr[_rand];
        
    }

    public void F_GetMeshRenMeshFil(int v_y, int v_x, MeshRenderer v_ren, MeshFilter v_fil)
    {
        _meshRenderers[v_y, v_x] = v_ren;
        _meshFilters[v_y, v_x] = v_fil;
    }

    public void F_arrangePlanetObject() 
    {
        // 0. ���� land�� planet Ÿ�Կ� ���� OutsideMapPooling�� List�� �����ؼ� ������Ʈ ��ġ 
        int _nowPlanetIdx = (int)(_nowLandScape.planetType);
        for (int i = 0; i < outsideMapPooling._planetsObjectList[_nowPlanetIdx].Count; i++) 
        {
            GameObject _obj = outsideMapPooling._planetsObjectList[_nowPlanetIdx][i];
            _obj.SetActive(true);
             
            // 1. ��ġ ���� 
            while (true)
            {
                // 1. ���� ��ġ ���� 
                int _randx = Random.Range( 5, _nowWidth - 5 );
                int _randz = Random.Range( 5, _nowHeight - 5 );

                // 2. �湮 ��������
                if (_arrangeObjectArr[_randx, _randz] != true)
                {
                    _obj.transform.position = new Vector3(_randx, _concludeMapArr[_randx, _randz], _randz) + _Offset;
                    _arrangeObjectArr[_randx, _randz] = true;   // �湮ó�� 
                    break;
                }
            }
             
        }
    }
}
