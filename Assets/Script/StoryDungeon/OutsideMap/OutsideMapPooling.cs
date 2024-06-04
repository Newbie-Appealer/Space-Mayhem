using System;
using System.Collections.Generic;
using UnityEngine;

public class OutsideMapPooling : MonoBehaviour
{
    /// <summary> 
    /// 1. meshRenderer / meshFilter 가지고 있는 게임 오브젝트 생성
    /// 2. collider 가지고 있는 게임 오브젝트 생성
    /// 
    /// </summary>

    [Header("====meshRender, meshfilter====")]
    [SerializeField] private GameObject _emptyMesh;                   // meshRenderer , meshfilter 가지고 있는 오브젝트
    [SerializeField] private GameObject _emptyCollider;               // collider 가지고 있는 오브젝트

    [SerializeField] private GameObject _emptyMeshPool;               // mesh 의 pool
    [SerializeField] private GameObject _emptyColliderPool;           // collider의 pool 

    [SerializeField] private Queue<GameObject> _emptyMeshQueue;       // mesh queue
    [SerializeField] private Queue<GameObject> _emptyColliderQueue;   // collider queue 

    [Header("====PlanetObect====")]
    [SerializeField] private GameObject _planetObjectTransform;           // planet 오브젝트 담아놓을 부모 
    [SerializeField] private List<List<GameObject>> _planetsObjectList;

    [Header("====Cant Escape Map====")]
    [SerializeField] private GameObject _cantEcapeMapObject;          // 플레이어가 외부맵으로 나가는걸 막는 오브젝트 
    [SerializeField] private GameObject[] _cantEscapeObjecList;       // 4개 생성 

    // 프로퍼티
    public List<List<GameObject>> planetObjectList { get => _planetsObjectList; }
    public GameObject[] cantEscapeObjectList { get => _cantEscapeObjecList; }

    private void Start()
    {
        // meshRender , collider queue 초기화
        _emptyMeshQueue = new Queue<GameObject>();
        _emptyColliderQueue = new Queue<GameObject>();

        // mesh, collider pool 초기화
        F_InitMapPooing();

        // 행성 오브젝트 pool 초기화
        F_InitPlanetObject();

        // cant Escape 오브젝트 초기화
        F_InitCantEscapeObject();
    }

    // cant Escape 오브젝트 초기화
    private void F_InitCantEscapeObject() 
    {
        _cantEscapeObjecList = new GameObject[4];       // 상,하,좌,우
        for(int i= 0; i < _cantEscapeObjecList.Length; i++) 
        {
            GameObject _temp = F_InstaneObject(_cantEcapeMapObject);
            _cantEscapeObjecList[i] = _temp;    
        }
    }

    // 오브젝트 생성후 return 
    private GameObject F_InstaneObject(GameObject v_obj)
    {
        GameObject _t = Instantiate(v_obj);
        _t.transform.parent = _planetObjectTransform.transform;
        _t.transform.localPosition = Vector3.zero;
        _t.SetActive(false);

        return _t;
    }


    // planet Object 초기화
    public void F_InitPlanetObject()
    {
        // 1. planet Object 초기화
        _planetsObjectList = new List<List<GameObject>>();

        // 2. planet Object 미리 생성 
        PlanetType[] values = (PlanetType[])Enum.GetValues(typeof(PlanetType));  // enum을 string으로 받아오기위한

        for (int i = 0; i < System.Enum.GetNames(typeof(PlanetType)).Length; i++)   // 행성 타입만큼 
        {
            string _path = "OutsideMapObject/" + values[i].ToString();      // type에 따른 경로 접근
            GameObject[] _obj = Resources.LoadAll<GameObject>(_path);       // 경로에 있는 오브젝트 다 들고오기

            List<GameObject> _tempPlanet = new List<GameObject>();                 // list에 넣을

            // [0] 스카이박스
            GameObject _sky = F_InstaneObject(_obj[0]);
            _tempPlanet.Add(_sky);

            // [1] 입구
            GameObject _en = F_InstaneObject(_obj[1]);
            _tempPlanet.Add(_en);

            // [2] 물
            GameObject _wa = F_InstaneObject(_obj[2]);
            _tempPlanet.Add(_wa);

            // [나머지] 두번 반복 
            for (int cnt = 0; cnt < 2; cnt++)
            {
                for (int j = 3; j < _obj.Length; j++)
                {
                    GameObject _temp = F_InstaneObject(_obj[j]);
                    _tempPlanet.Add(_temp);
                }
            }

            _planetsObjectList.Add(_tempPlanet);
        }
    
    }

    // planet object 다시 꺼놓기
    public void F_ReturnPlanetObject( int v_typeIdx ) 
    {
        for (int i = 0; i < _planetsObjectList[v_typeIdx].Count; i++) 
        {
            GameObject _re = _planetsObjectList[v_typeIdx][i];
            _re.SetActive(false);
            _re.transform.localPosition = Vector3.zero; 
        }

    }

    // mesh, collider 초기화 
    public void F_InitMapPooing()
    {
        // 초기 생성 너비 * 높이 만큼  
        for (int i = 0; i < OutsideMapManager.Instance.heightXwidth; i++)
        {
            GameObject _me = Instantiate(_emptyMesh);
            GameObject _co = Instantiate(_emptyCollider);

            _me.transform.parent = _emptyMeshPool.transform;
            _co.transform.parent = _emptyColliderPool.transform;

            _emptyMeshQueue.Enqueue(_me);
            _emptyColliderQueue.Enqueue(_co);

            _me.SetActive(false);
            _co.SetActive(false);
        }
    }

    // Mesh get
    public GameObject F_GetMeshObject()
    {
        GameObject _returnMeshObj = null;

        // 1. 큐에 있으면
        if (_emptyMeshQueue.Count != 0)
        {
            _returnMeshObj = _emptyMeshQueue.Dequeue();
            _returnMeshObj.SetActive(true);
        }
        // 2.혹시모를예외 , 큐에 없으면
        else
        {
            // 2-1. 새로 생성하기  
            _returnMeshObj = Instantiate(_emptyMesh);
        }

        return _returnMeshObj;
    }

    // Collider get
    public GameObject F_GetColliderObject()
    {
        GameObject _returnCollObj = null;

        // 1. 큐에 있으면
        if (_emptyColliderQueue.Count != 0)
        {
            _returnCollObj = _emptyColliderQueue.Dequeue();
            _returnCollObj.SetActive(true);
        }
        // 2. 혹시모를예외 , 큐에 없으면 
        else
        {
            // 2-1. 새로 생성하기 
            _returnCollObj = Instantiate(_emptyCollider);
        }

        return _returnCollObj;
    }

    // Mesh return
    public void F_ReturMeshObject(GameObject v_returnMesh , bool v_flag )
    {
        // 1. 부모밑 0,0,0로 , 끄기 
        v_returnMesh.transform.localPosition = Vector3.zero;
        v_returnMesh.SetActive(false);

        if (v_flag)
            v_returnMesh.transform.parent = _emptyMeshPool.transform;

        // 2. queue에 넣기 
        _emptyMeshQueue.Enqueue(v_returnMesh);
    }

    // Collider return
    public void F_ReturnColliderObject()
    {
        for (int i = 0; i < _emptyColliderPool.transform.childCount; i++)
        {
            GameObject _child = _emptyColliderPool.transform.GetChild(i).gameObject;
            // 1. 켜져있으면(pool에서 빠져있으면) pool에 넣기 
            if (_child.activeSelf == true)
            {
                // 1. 부모밑 0,0,0로 , 끄기 
                _child.transform.localPosition = Vector3.zero;
                _child.SetActive(false);

                // 2. queue에 넣기 
                _emptyColliderQueue.Enqueue(_child); ;
            }
        }
    }

    // CantScapeObject return 
    public void F_ReturnCantEscapeObject() 
    {
        for(int i = 0; i < _cantEscapeObjecList.Length; i++) 
        {
            _cantEscapeObjecList[i].transform.localPosition = Vector3.zero;
            _cantEscapeObjecList[i].SetActive(false);

            // 회전 돌려놓기
            _cantEscapeObjecList[i].transform.rotation = Quaternion.Euler(0,0,0);
        }
    }
}
