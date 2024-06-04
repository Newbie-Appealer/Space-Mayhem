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
    [HideInInspector] LandScape[] _landSacpeArr;     // landScape ��Ƴ��� �迭
    [SerializeField] LandScape _nowLandScape;       // ���� landScape

    [Header("====setting====")]
    public Vector3 _Offset;                                     // OusideMap�� ������ ��ġ
    [HideInInspector] private Vector3 _playerTeleportPosition;  // �÷��̾ �ܺθ����� �̵��� ��ġ 
    [SerializeField] private int _PlanetSeed;                   // ���� seed 
    [SerializeField] private int _dropItemCount;                // drop ������ ���� 
    [SerializeField] private int _enteranceGeneratePosiX;       // enterance ������ ��ġ x
    [SerializeField] private int _enteranceGeneratePosiZ;       // enterance ������ ��ġ z

    [Header("======Container======")]
    private float[,] _concludeMapArr;
    private MeshRenderer[,] _meshRenderers;         // mesh�� material ���� ���� 
    private MeshFilter[,] _meshFilters;             // mesh ���� ����    
    private bool[,] _visitObjectArr;                // ������Ʈ ��ġ bool
    private List<GameObject> _outsidemapAnimal;     // �ܺ� �� ���� 

    [Header("======GameObject======")]
    [SerializeField] private Transform _mapParent;                    // ���� ���� map�� �θ� 

    [Header("=====WIDTH, HEIGHT=====")]
    private const int mapMaxHeight = 100;
    private const int mapMaxWidth = 100;
    [HideInInspector] private int _nowWidth;           // ���� width
    [HideInInspector] private int _nowHeight;          // ���� height 

    public int heightXwidth => mapMaxHeight * mapMaxWidth;
    public Vector3 playerTeleportPosition => _playerTeleportPosition;
    public Transform mapParent => _mapParent;
    
    [Header("====== Script ======")]
    [SerializeField] private MapHeightGenerator mapHeightGenerate;
    [SerializeField] private MeshGenerator meshGenerator;
    [SerializeField] private MeshCombine meshCombine;
    [SerializeField] private ColliderGenerator colliderGenerator;
    [SerializeField] private OutsideMapDataManager mapDataManager;
    [SerializeField] private OutsideMapPooling _outsideMapPooling;
    [SerializeField] private PlanetManager _planetManager;

    [Header("===== Hide Script ====")]
    [HideInInspector] private DropItemSystem _dropItemSystem;

    // ������Ƽ
    public LandScape nowLandScape => _nowLandScape;
    public OutsideMapPooling outsideMapPooling => _outsideMapPooling;

    protected override void InitManager()
    {
        F_InitOutsideMap();
    }

    // �ʱ� ����
    public void F_InitOutsideMap()
    {
        // 0. ��ũ��Ʈ ����
        mapDataManager = new OutsideMapDataManager();
        _dropItemSystem = ItemManager.Instance.dropItemSystem;

        // 1. Ÿ�Ը�ŭ arr ����
        _landSacpeArr = new LandScape[System.Enum.GetValues(typeof(PlanetType)).Length];

        // 2. ���������̺� get
        mapDataManager.F_InitOutsidemapData();

        // 0. �ʱ⼱�� 
        _concludeMapArr     = new float[mapMaxWidth, mapMaxHeight];
        _meshRenderers      = new MeshRenderer[mapMaxWidth, mapMaxHeight];
        _meshFilters        = new MeshFilter[mapMaxWidth, mapMaxHeight];
        _visitObjectArr     = new bool[mapMaxWidth, mapMaxHeight];
        _outsidemapAnimal   = new List<GameObject>();

        // 3. seed ����
        _PlanetSeed = 0;
        _dropItemCount = 3;

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
        System.Array.Clear(_visitObjectArr, 0 , _visitObjectArr.Length );     // false    

        // 1. ���� landScape
        _nowLandScape = _landSacpeArr[_planetManager.planetIdx];

        // 2. ���� landScape�� ���� ���� width, height ���ϱ�
        _nowWidth = Random.Range( _nowLandScape.minWidth , _nowLandScape.maxWidth );
        _nowHeight = Random.Range( _nowLandScape.minHeight , _nowLandScape.maxHeight );

        // 3. �� ���� ����
        // _conclude = Generatemap()�ϸ� conclude�� �����ϰ��ִ� �޸𸮰� �ս� 
        mapHeightGenerate.GenerateMap( ref _concludeMapArr, _nowWidth, _nowHeight, _PlanetSeed, 
            _nowLandScape.noiseScale, _nowLandScape.octave, _nowLandScape.persistance, _nowLandScape.lacunerity, _nowLandScape.devigation);

        // 3-1. ��������
        _playerTeleportPosition = new Vector3( _nowWidth / 2, _Offset.y + _concludeMapArr[_nowWidth / 2 , 10], 10);     // �÷��̾� ��ġ ���� 
        _enteranceGeneratePosiX = _nowWidth / 2;            // �Ա� x ����
        _enteranceGeneratePosiZ = _nowHeight - 10;          // �Ա� z ���� 

        // 3-1. seed ���� 
        _PlanetSeed++;

        // 3-2. �� �ܰ� �湮ó��
        F_mapVisitedProcess();

        // 4. �Ž� ����
        meshGenerator.F_CreateMeshMap(_nowWidth, _nowHeight, ref _concludeMapArr);

        // 5. �ݶ��̴� ����
        colliderGenerator.F_CreateCollider(_nowWidth, _nowHeight, meshGenerator.PointList);

        // 6. �Ž� ��ġ��
        meshCombine.F_MeshCombine(
            _nowWidth, _nowHeight , _meshRenderers , _meshFilters);

        // 7. �÷��̾� �������� �ܰ��� ������Ʈ ��ġ 
        F_PlaceCantEscapeObject();

        // 8. �༺ ������� ��ġ
        F_arrangePlanetObject();

        // 9. ���� ����, navMesh ���� , EnemyManager�Լ� ��� 
        F_PlaceOutsideAnimal();

        // 10. ������ ��� 
        F_PlaceOutsideItemDrop();
    }

    // �÷��̾ �༺������ ������ ���ϰ�
    private void F_PlaceCantEscapeObject() 
    {
        GameObject[] _cantEscapeObj = outsideMapPooling.cantEscapeObjectList;

        // 1. �ѱ�
        for(int i = 0; i < _cantEscapeObj.Length; i++)
            _cantEscapeObj[i].SetActive(true);

        // 2. ��ġ����
        _cantEscapeObj[0].transform.position = _Offset + new Vector3( _nowWidth/2, 10f, 0);             // �Ʒ� 
        _cantEscapeObj[1].transform.position = _Offset + new Vector3( 0, 10f, _nowHeight/2 );           // ��
        _cantEscapeObj[2].transform.position = _Offset + new Vector3( _nowWidth/2, 10f, _nowHeight );   // ��
        _cantEscapeObj[3].transform.position = _Offset + new Vector3( _nowWidth, 10f, _nowHeight/2);    // ����

        // 3. ��,���� ������Ʈ -> ȸ�� 90 �ؾ���
        _cantEscapeObj[1].transform.rotation = Quaternion.Euler(0, 90f, 0);
        _cantEscapeObj[3].transform.rotation = Quaternion.Euler(0, 90f, 0);

        // 4. ������ : Ȥ�ó� ������ �������� �� �ǵ����� ���ؼ�
        _cantEscapeObj[4].transform.position = _Offset + new Vector3( _nowWidth/2 , -10f, _nowHeight/2 );
        _cantEscapeObj[4].transform.rotation = Quaternion.Euler(90f, 0, 0 );
        _cantEscapeObj[4].transform.localScale = new Vector3( 100f, 100f, 1f );

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

        // 3. cantEScapeObject �ٽ� ����ֱ�
        outsideMapPooling.F_ReturnCantEscapeObject();
    }

    public void F_GetMeshRenMeshFil(int v_y, int v_x, MeshRenderer v_ren, MeshFilter v_fil)
    {
        _meshRenderers[v_y, v_x] = v_ren;
        _meshFilters[v_y, v_x] = v_fil;
    }

    // ���� ������Ʈ ��ġ 
    public void F_arrangePlanetObject() 
    {
        // 0. ���� land�� planet Ÿ�Կ� ���� OutsideMapPooling�� List�� �����ؼ� ������Ʈ ��ġ 
        int _nowPlanetIdx = (int)(_nowLandScape.planetType);

        // 1. ������Ʈ list �� ����
        // [0] : skybox
        // [1] : �Ա�
        // [2] : �� or empty

        // 1-1. [0] skybox
        GameObject _skybox = outsideMapPooling.planetObjectList[_nowPlanetIdx][0];
        _skybox.SetActive(true);
        _skybox.transform.position = new Vector3(_nowWidth/2 , 7f , _nowHeight/2)+ _Offset;

        // 1-2. [1] �Ա�
        GameObject _enternace = outsideMapPooling.planetObjectList[_nowPlanetIdx][1];
        _enternace.SetActive(true);

        // 1-2-1. �Ա� ��ġ ���� 
        _enternace.transform.position = new Vector3(_enteranceGeneratePosiX, _concludeMapArr[_enteranceGeneratePosiX, _enteranceGeneratePosiZ] , _enteranceGeneratePosiZ) + _Offset;

        // 1-2. [2] �� or empty
        GameObject _water = outsideMapPooling.planetObjectList[_nowPlanetIdx][2];
        _water.SetActive(true);

        // ���� �ִ� �༺�� ���� 
        if (_nowLandScape.planetType == PlanetType.Hydroros)        
            _water.transform.position = new Vector3(0, 8f, 0) + _Offset;            

        // �� ���� �༺ 
        else 
            _water.transform.position = new Vector3( _nowWidth/2, _concludeMapArr[_nowWidth/2 , _nowHeight/2], _nowHeight/2) + _Offset;           


        // 2. �� �� ������Ʈ ��ġ 
        for (int i = 3; i < outsideMapPooling.planetObjectList[_nowPlanetIdx].Count; i++)
        {
            // 0. ���� �༺�� �ش��ϴ� ������Ʈ 
            GameObject _obj = outsideMapPooling.planetObjectList[_nowPlanetIdx][i];
            _obj.SetActive(true);

            // 1. ��ġ ���� 
            F_setObjectInRandeomPosi(_obj);
        }

    }

    // ���� animal ���� 
    private void F_PlaceOutsideAnimal() 
    {
        _outsidemapAnimal.Clear();

        string[] _animalName = { "SWAN" , "SWAN", "SWAN", "SWAN", "SWAN", "SWAN", "TURTLE" , "TURTLE", "TURTLE", "TURTLE" };

        // 1. Enemy ����
        _outsidemapAnimal = EnemyManager.Instance.F_GetEnemys(_animalName);

        // 2. ��ġ ����
        foreach( GameObject enemy in _outsidemapAnimal) 
        {
            F_setObjectInRandeomPosi(enemy);
        }

        // 3. navMesh bake
        EnemyManager.Instance.F_NavMeshBake( NavMeshType.OUTSIDE );
    }

    // ���� dropItem ��ġ 
    private void F_PlaceOutsideItemDrop() 
    {
        for (int i = 0; i < _dropItemCount; i++) 
        {
            // 1. ���� ������ drop 
            F_setObjectInRandeomPosi(_dropItemSystem.F_GetRandomDropItem());   
        }
        
    } 

    // ���� ��ġ ��� 
    private void F_setObjectInRandeomPosi( GameObject v_obj ) 
    {
        while (true)
        {
            // 0. ���� �� visit �Ǹ�  return 
            if (F_MapVisit() <= 0)
                return;

            // 1. ���� ��ġ ���� 
            int _randx = Random.Range(5, _nowWidth - 5);
            int _randz = Random.Range(5, _nowHeight - 5);

            // 2. �湮 ��������
            if (_visitObjectArr[_randx, _randz] != true)
            {
                v_obj.transform.position = new Vector3(_randx, _concludeMapArr[_randx, _randz], _randz) + _Offset;      // ������Ʈ ��ġ ���� �� 
                _visitObjectArr[_randx, _randz] = true;                                                                 // �湮ó�� 
                break;
            }
        }
    }

    // ���� visit ���� ���ϱ� 
    private int F_MapVisit()
    {
        int cnt = 0; 

        for (int y = 0; y < _nowHeight; y++) 
        {
            for (int x = 0; x < _nowWidth; x++) 
            {
                // 0. ����ó��
                if (cnt == _nowWidth * _nowHeight)
                    return 0;

                // 1. �湮Ƚ�� �߰� 
                if (_visitObjectArr[x, y] == false)
                    cnt++;
            }
        }

        return cnt;
    }

    // �� �湮ó�� 
    private void F_mapVisitedProcess() 
    {
        // 1. �÷��̾� ���� ��ġ �湮ó�� 
        for (int y = (int)(_playerTeleportPosition.z - 3); y < _playerTeleportPosition.z + 3; y++)
        {
            for (int x = (int)_playerTeleportPosition.x - 3; x < _playerTeleportPosition.x + 3; x++) 
            {
                _visitObjectArr[x, y] = true;
            }
        }


        // 2. �ܺ� -> ���� enterance �湮ó��
        for (int y = _enteranceGeneratePosiZ - 5; y < _enteranceGeneratePosiZ + 5; y++)
        {
            for (int x = _enteranceGeneratePosiX - 5; x < _enteranceGeneratePosiX + 5; x++)
            {
                _visitObjectArr[x, y] = true;
            }
        }


        // 3. �� �ܰ� �湮ó�� 
        // (1) ��
        for (int y = 0; y < 5; y++) 
        {
            for (int x = 0; x < _nowWidth; x++) 
            {
                _visitObjectArr[x, y] = true;
            }
        }

        // (2) ��
        for (int y = 5; y < _nowHeight; y++) 
        {
            for (int x = 0; x < 5; x++) 
            {
                _visitObjectArr[x, y] = true;
            }
        }

        // (3) ��
        for (int y = _nowHeight - 5; y < _nowHeight; y++) 
        {
            for (int x = 5; x < _nowWidth; x++) 
            {
                _visitObjectArr[x, y] = true;
            }
        }

        // (4) ��
        for (int y = 5; y < _nowHeight - 5; y++) 
        {
            for (int x = _nowWidth - 5; x < _nowWidth; x++) 
            {
                _visitObjectArr[x, y] = true;
            }
        }

    }


}
