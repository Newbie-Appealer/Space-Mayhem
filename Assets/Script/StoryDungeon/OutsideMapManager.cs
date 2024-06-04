using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

using Random = UnityEngine.Random;

public enum PlanetType 
{
    Earth,          // 지구
    Lavenderia,     // 보라색 식물
    Crimsonium,     // 핑크색 종말
    Junkar,         // 쓰레기
    Silvantis,      // 아마존
    Fractonia,      // 단층
    Lithosia,       // 돌
    Floralia,       // 꽃
    Hydroros,       // 바다 
    Cryolithe,      // 얼음
    Potaterra       // 감자
}

public class OutsideMapManager : Singleton<OutsideMapManager>
{
    [Header("=====Curr LandScpae====")]
    [HideInInspector] LandScape[] _landSacpeArr;     // landScape 담아놓는 배열
    [SerializeField] LandScape _nowLandScape;       // 현재 landScape

    [Header("====setting====")]
    public Vector3 _Offset;                                     // OusideMap이 생성될 위치
    [HideInInspector] private Vector3 _playerTeleportPosition;  // 플레이어가 외부맵으로 이동할 위치 
    [SerializeField] private int _PlanetSeed;                   // 현재 seed 
    [SerializeField] private int _dropItemCount;                // drop 아이템 갯수 
    [SerializeField] private int _enteranceGeneratePosiX;       // enterance 생성할 위치 x
    [SerializeField] private int _enteranceGeneratePosiZ;       // enterance 생성할 위치 z

    [Header("======Container======")]
    private float[,] _concludeMapArr;
    private MeshRenderer[,] _meshRenderers;         // mesh의 material 접근 위한 
    private MeshFilter[,] _meshFilters;             // mesh 적용 위한    
    private bool[,] _visitObjectArr;                // 오브젝트 설치 bool
    private List<GameObject> _outsidemapAnimal;     // 외부 맵 동물 

    [Header("======GameObject======")]
    [SerializeField] private Transform _mapParent;                    // 최종 생성 map의 부모 

    [Header("=====WIDTH, HEIGHT=====")]
    private const int mapMaxHeight = 100;
    private const int mapMaxWidth = 100;
    [HideInInspector] private int _nowWidth;           // 현재 width
    [HideInInspector] private int _nowHeight;          // 현재 height 

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

    // 프로퍼티
    public LandScape nowLandScape => _nowLandScape;
    public OutsideMapPooling outsideMapPooling => _outsideMapPooling;

    protected override void InitManager()
    {
        F_InitOutsideMap();
    }

    // 초기 선언
    public void F_InitOutsideMap()
    {
        // 0. 스크립트 생성
        mapDataManager = new OutsideMapDataManager();
        _dropItemSystem = ItemManager.Instance.dropItemSystem;

        // 1. 타입만큼 arr 생성
        _landSacpeArr = new LandScape[System.Enum.GetValues(typeof(PlanetType)).Length];

        // 2. 데이터테이블 get
        mapDataManager.F_InitOutsidemapData();

        // 0. 초기선언 
        _concludeMapArr     = new float[mapMaxWidth, mapMaxHeight];
        _meshRenderers      = new MeshRenderer[mapMaxWidth, mapMaxHeight];
        _meshFilters        = new MeshFilter[mapMaxWidth, mapMaxHeight];
        _visitObjectArr     = new bool[mapMaxWidth, mapMaxHeight];
        _outsidemapAnimal   = new List<GameObject>();

        // 3. seed 선언
        _PlanetSeed = 0;
        _dropItemCount = 3;

    }

    // type에 따른 landScape를 arr에 저장
    public void F_InsertLandSacpeArr( string v_type , LandScape v_land ) 
    {
        // 타입에 맞는 index에 land 넣기  
        PlanetType type = (PlanetType)Enum.Parse(typeof(PlanetType), v_type);
        _landSacpeArr[(int)type] = v_land;
    }

    // ## TODO 
    // 다른스크립트에서 사용

    public void F_CreateOutsideMap()
    {
        // 0. 초기화, (배열, 처음인덱스, 지울 데이터의 갯수) 
        System.Array.Clear(_concludeMapArr, 0, _concludeMapArr.Length);       // 0 
        System.Array.Clear(_meshRenderers, 0, _meshRenderers.Length);         // null
        System.Array.Clear(_meshFilters, 0, _meshFilters.Length);             // null
        System.Array.Clear(_visitObjectArr, 0 , _visitObjectArr.Length );     // false    

        // 1. 현재 landScape
        _nowLandScape = _landSacpeArr[_planetManager.planetIdx];

        // 2. 현재 landScape에 대한 랜덤 width, height 구하기
        _nowWidth = Random.Range( _nowLandScape.minWidth , _nowLandScape.maxWidth );
        _nowHeight = Random.Range( _nowLandScape.minHeight , _nowLandScape.maxHeight );

        // 3. 맵 높이 지정
        // _conclude = Generatemap()하면 conclude가 참조하고있는 메모리가 손실 
        mapHeightGenerate.GenerateMap( ref _concludeMapArr, _nowWidth, _nowHeight, _PlanetSeed, 
            _nowLandScape.noiseScale, _nowLandScape.octave, _nowLandScape.persistance, _nowLandScape.lacunerity, _nowLandScape.devigation);

        // 3-1. 변수세팅
        _playerTeleportPosition = new Vector3( _nowWidth / 2, _Offset.y + _concludeMapArr[_nowWidth / 2 , 10], 10);     // 플레이어 위치 설정 
        _enteranceGeneratePosiX = _nowWidth / 2;            // 입구 x 설정
        _enteranceGeneratePosiZ = _nowHeight - 10;          // 입구 z 설정 

        // 3-1. seed 증가 
        _PlanetSeed++;

        // 3-2. 맵 외각 방문처리
        F_mapVisitedProcess();

        // 4. 매쉬 생성
        meshGenerator.F_CreateMeshMap(_nowWidth, _nowHeight, ref _concludeMapArr);

        // 5. 콜라이더 생성
        colliderGenerator.F_CreateCollider(_nowWidth, _nowHeight, meshGenerator.PointList);

        // 6. 매쉬 합치기
        meshCombine.F_MeshCombine(
            _nowWidth, _nowHeight , _meshRenderers , _meshFilters);

        // 7. 플레이어 못나가게 외각에 오브젝트 설치 
        F_PlaceCantEscapeObject();

        // 8. 행성 구성요소 설치
        F_arrangePlanetObject();

        // 9. 동물 생성, navMesh 굽기 , EnemyManager함수 사용 
        F_PlaceOutsideAnimal();

        // 10. 아이템 드롭 
        F_PlaceOutsideItemDrop();
    }

    // 플레이어가 행성밖으로 나가지 못하게
    private void F_PlaceCantEscapeObject() 
    {
        GameObject[] _cantEscapeObj = outsideMapPooling.cantEscapeObjectList;

        // 1. 켜기
        for(int i = 0; i < _cantEscapeObj.Length; i++)
            _cantEscapeObj[i].SetActive(true);

        // 2. 위치설정
        _cantEscapeObj[0].transform.position = _Offset + new Vector3( _nowWidth/2, 10f, 0);             // 아래 
        _cantEscapeObj[1].transform.position = _Offset + new Vector3( 0, 10f, _nowHeight/2 );           // 왼
        _cantEscapeObj[2].transform.position = _Offset + new Vector3( _nowWidth/2, 10f, _nowHeight );   // 위
        _cantEscapeObj[3].transform.position = _Offset + new Vector3( _nowWidth, 10f, _nowHeight/2);    // 오른

        // 3. 왼,오른 오브젝트 -> 회전 90 해야함
        _cantEscapeObj[1].transform.rotation = Quaternion.Euler(0, 90f, 0);
        _cantEscapeObj[3].transform.rotation = Quaternion.Euler(0, 90f, 0);

        // 4. 마지막 : 혹시나 밑으로 떨어졌을 때 되돌리기 위해서
        _cantEscapeObj[4].transform.position = _Offset + new Vector3( _nowWidth/2 , -10f, _nowHeight/2 );
        _cantEscapeObj[4].transform.rotation = Quaternion.Euler(90f, 0, 0 );
        _cantEscapeObj[4].transform.localScale = new Vector3( 100f, 100f, 1f );

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

        // 2. map object 다시 집어넣기 
        outsideMapPooling.F_ReturnPlanetObject( (int)_nowLandScape.planetType );

        // 3. cantEScapeObject 다시 집어넣기
        outsideMapPooling.F_ReturnCantEscapeObject();
    }

    public void F_GetMeshRenMeshFil(int v_y, int v_x, MeshRenderer v_ren, MeshFilter v_fil)
    {
        _meshRenderers[v_y, v_x] = v_ren;
        _meshFilters[v_y, v_x] = v_fil;
    }

    // 랜덤 오브젝트 설치 
    public void F_arrangePlanetObject() 
    {
        // 0. 현재 land의 planet 타입에 따라 OutsideMapPooling의 List에 접근해서 오브젝트 설치 
        int _nowPlanetIdx = (int)(_nowLandScape.planetType);

        // 1. 오브젝트 list 의 순서
        // [0] : skybox
        // [1] : 입구
        // [2] : 물 or empty

        // 1-1. [0] skybox
        GameObject _skybox = outsideMapPooling.planetObjectList[_nowPlanetIdx][0];
        _skybox.SetActive(true);
        _skybox.transform.position = new Vector3(_nowWidth/2 , 7f , _nowHeight/2)+ _Offset;

        // 1-2. [1] 입구
        GameObject _enternace = outsideMapPooling.planetObjectList[_nowPlanetIdx][1];
        _enternace.SetActive(true);

        // 1-2-1. 입구 위치 설정 
        _enternace.transform.position = new Vector3(_enteranceGeneratePosiX, _concludeMapArr[_enteranceGeneratePosiX, _enteranceGeneratePosiZ] , _enteranceGeneratePosiZ) + _Offset;

        // 1-2. [2] 물 or empty
        GameObject _water = outsideMapPooling.planetObjectList[_nowPlanetIdx][2];
        _water.SetActive(true);

        // 물만 있는 행성일 때는 
        if (_nowLandScape.planetType == PlanetType.Hydroros)        
            _water.transform.position = new Vector3(0, 8f, 0) + _Offset;            

        // 그 외의 행성 
        else 
            _water.transform.position = new Vector3( _nowWidth/2, _concludeMapArr[_nowWidth/2 , _nowHeight/2], _nowHeight/2) + _Offset;           


        // 2. 그 외 오브젝트 설치 
        for (int i = 3; i < outsideMapPooling.planetObjectList[_nowPlanetIdx].Count; i++)
        {
            // 0. 현재 행성에 해당하는 오브젝트 
            GameObject _obj = outsideMapPooling.planetObjectList[_nowPlanetIdx][i];
            _obj.SetActive(true);

            // 1. 위치 설정 
            F_setObjectInRandeomPosi(_obj);
        }

    }

    // 랜덤 animal 생성 
    private void F_PlaceOutsideAnimal() 
    {
        _outsidemapAnimal.Clear();

        string[] _animalName = { "SWAN" , "SWAN", "SWAN", "SWAN", "SWAN", "SWAN", "TURTLE" , "TURTLE", "TURTLE", "TURTLE" };

        // 1. Enemy 생성
        _outsidemapAnimal = EnemyManager.Instance.F_GetEnemys(_animalName);

        // 2. 위치 지정
        foreach( GameObject enemy in _outsidemapAnimal) 
        {
            F_setObjectInRandeomPosi(enemy);
        }

        // 3. navMesh bake
        EnemyManager.Instance.F_NavMeshBake( NavMeshType.OUTSIDE );
    }

    // 랜덤 dropItem 설치 
    private void F_PlaceOutsideItemDrop() 
    {
        for (int i = 0; i < _dropItemCount; i++) 
        {
            // 1. 랜덤 아이템 drop 
            F_setObjectInRandeomPosi(_dropItemSystem.F_GetRandomDropItem());   
        }
        
    } 

    // 랜덤 위치 잡기 
    private void F_setObjectInRandeomPosi( GameObject v_obj ) 
    {
        while (true)
        {
            // 0. 맵이 다 visit 되면  return 
            if (F_MapVisit() <= 0)
                return;

            // 1. 랜덤 위치 설정 
            int _randx = Random.Range(5, _nowWidth - 5);
            int _randz = Random.Range(5, _nowHeight - 5);

            // 2. 방문 안했으면
            if (_visitObjectArr[_randx, _randz] != true)
            {
                v_obj.transform.position = new Vector3(_randx, _concludeMapArr[_randx, _randz], _randz) + _Offset;      // 오브젝트 위치 설정 후 
                _visitObjectArr[_randx, _randz] = true;                                                                 // 방문처리 
                break;
            }
        }
    }

    // 남은 visit 갯수 구하기 
    private int F_MapVisit()
    {
        int cnt = 0; 

        for (int y = 0; y < _nowHeight; y++) 
        {
            for (int x = 0; x < _nowWidth; x++) 
            {
                // 0. 예외처리
                if (cnt == _nowWidth * _nowHeight)
                    return 0;

                // 1. 방문횟수 추가 
                if (_visitObjectArr[x, y] == false)
                    cnt++;
            }
        }

        return cnt;
    }

    // 맵 방문처리 
    private void F_mapVisitedProcess() 
    {
        // 1. 플레이어 생성 위치 방문처리 
        for (int y = (int)(_playerTeleportPosition.z - 3); y < _playerTeleportPosition.z + 3; y++)
        {
            for (int x = (int)_playerTeleportPosition.x - 3; x < _playerTeleportPosition.x + 3; x++) 
            {
                _visitObjectArr[x, y] = true;
            }
        }


        // 2. 외부 -> 내부 enterance 방문처리
        for (int y = _enteranceGeneratePosiZ - 5; y < _enteranceGeneratePosiZ + 5; y++)
        {
            for (int x = _enteranceGeneratePosiX - 5; x < _enteranceGeneratePosiX + 5; x++)
            {
                _visitObjectArr[x, y] = true;
            }
        }


        // 3. 맵 외각 방문처리 
        // (1) 하
        for (int y = 0; y < 5; y++) 
        {
            for (int x = 0; x < _nowWidth; x++) 
            {
                _visitObjectArr[x, y] = true;
            }
        }

        // (2) 좌
        for (int y = 5; y < _nowHeight; y++) 
        {
            for (int x = 0; x < 5; x++) 
            {
                _visitObjectArr[x, y] = true;
            }
        }

        // (3) 상
        for (int y = _nowHeight - 5; y < _nowHeight; y++) 
        {
            for (int x = 5; x < _nowWidth; x++) 
            {
                _visitObjectArr[x, y] = true;
            }
        }

        // (4) 우
        for (int y = 5; y < _nowHeight - 5; y++) 
        {
            for (int x = _nowWidth - 5; x < _nowWidth; x++) 
            {
                _visitObjectArr[x, y] = true;
            }
        }

    }


}
