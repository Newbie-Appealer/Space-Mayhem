using System;
using System.Collections.Generic;
using UnityEngine;

public class OutsideMapPooling : MonoBehaviour
{
    /// <summary> 
    /// 1. meshRenderer / meshFilter ������ �ִ� ���� ������Ʈ ����
    /// 2. collider ������ �ִ� ���� ������Ʈ ����
    /// 
    /// </summary>

    [Header("====meshRender, meshfilter====")]
    [SerializeField] private GameObject _emptyMesh;                   // meshRenderer , meshfilter ������ �ִ� ������Ʈ
    [SerializeField] private GameObject _emptyCollider;               // collider ������ �ִ� ������Ʈ

    [SerializeField] private GameObject _emptyMeshPool;               // mesh �� pool
    [SerializeField] private GameObject _emptyColliderPool;           // collider�� pool 

    [SerializeField] private Queue<GameObject> _emptyMeshQueue;       // mesh queue
    [SerializeField] private Queue<GameObject> _emptyColliderQueue;   // collider queue 

    [Header("====PlanetObect====")]
    [SerializeField] private GameObject _planetObjectTransform;           // planet ������Ʈ ��Ƴ��� �θ� 
    [SerializeField] private List<List<GameObject>> _planetsObjectList;

    [Header("====Cant Escape Map====")]
    [SerializeField] private GameObject _cantEcapeMapObject;          // �÷��̾ �ܺθ����� �����°� ���� ������Ʈ 
    [SerializeField] private GameObject[] _cantEscapeObjecList;       // 4�� ���� 

    // ������Ƽ
    public List<List<GameObject>> planetObjectList { get => _planetsObjectList; }
    public GameObject[] cantEscapeObjectList { get => _cantEscapeObjecList; }

    private void Start()
    {
        // meshRender , collider queue �ʱ�ȭ
        _emptyMeshQueue = new Queue<GameObject>();
        _emptyColliderQueue = new Queue<GameObject>();

        // mesh, collider pool �ʱ�ȭ
        F_InitMapPooing();

        // �༺ ������Ʈ pool �ʱ�ȭ
        F_InitPlanetObject();

        // cant Escape ������Ʈ �ʱ�ȭ
        F_InitCantEscapeObject();
    }

    // cant Escape ������Ʈ �ʱ�ȭ
    private void F_InitCantEscapeObject() 
    {
        _cantEscapeObjecList = new GameObject[4];       // ��,��,��,��
        for(int i= 0; i < _cantEscapeObjecList.Length; i++) 
        {
            GameObject _temp = F_InstaneObject(_cantEcapeMapObject);
            _cantEscapeObjecList[i] = _temp;    
        }
    }

    // ������Ʈ ������ return 
    private GameObject F_InstaneObject(GameObject v_obj)
    {
        GameObject _t = Instantiate(v_obj);
        _t.transform.parent = _planetObjectTransform.transform;
        _t.transform.localPosition = Vector3.zero;
        _t.SetActive(false);

        return _t;
    }


    // planet Object �ʱ�ȭ
    public void F_InitPlanetObject()
    {
        // 1. planet Object �ʱ�ȭ
        _planetsObjectList = new List<List<GameObject>>();

        // 2. planet Object �̸� ���� 
        PlanetType[] values = (PlanetType[])Enum.GetValues(typeof(PlanetType));  // enum�� string���� �޾ƿ�������

        for (int i = 0; i < System.Enum.GetNames(typeof(PlanetType)).Length; i++)   // �༺ Ÿ�Ը�ŭ 
        {
            string _path = "OutsideMapObject/" + values[i].ToString();      // type�� ���� ��� ����
            GameObject[] _obj = Resources.LoadAll<GameObject>(_path);       // ��ο� �ִ� ������Ʈ �� ������

            List<GameObject> _tempPlanet = new List<GameObject>();                 // list�� ����

            // [0] ��ī�̹ڽ�
            GameObject _sky = F_InstaneObject(_obj[0]);
            _tempPlanet.Add(_sky);

            // [1] �Ա�
            GameObject _en = F_InstaneObject(_obj[1]);
            _tempPlanet.Add(_en);

            // [2] ��
            GameObject _wa = F_InstaneObject(_obj[2]);
            _tempPlanet.Add(_wa);

            // [������] �ι� �ݺ� 
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

    // planet object �ٽ� ������
    public void F_ReturnPlanetObject( int v_typeIdx ) 
    {
        for (int i = 0; i < _planetsObjectList[v_typeIdx].Count; i++) 
        {
            GameObject _re = _planetsObjectList[v_typeIdx][i];
            _re.SetActive(false);
            _re.transform.localPosition = Vector3.zero; 
        }

    }

    // mesh, collider �ʱ�ȭ 
    public void F_InitMapPooing()
    {
        // �ʱ� ���� �ʺ� * ���� ��ŭ  
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

        // 1. ť�� ������
        if (_emptyMeshQueue.Count != 0)
        {
            _returnMeshObj = _emptyMeshQueue.Dequeue();
            _returnMeshObj.SetActive(true);
        }
        // 2.Ȥ�ø𸦿��� , ť�� ������
        else
        {
            // 2-1. ���� �����ϱ�  
            _returnMeshObj = Instantiate(_emptyMesh);
        }

        return _returnMeshObj;
    }

    // Collider get
    public GameObject F_GetColliderObject()
    {
        GameObject _returnCollObj = null;

        // 1. ť�� ������
        if (_emptyColliderQueue.Count != 0)
        {
            _returnCollObj = _emptyColliderQueue.Dequeue();
            _returnCollObj.SetActive(true);
        }
        // 2. Ȥ�ø𸦿��� , ť�� ������ 
        else
        {
            // 2-1. ���� �����ϱ� 
            _returnCollObj = Instantiate(_emptyCollider);
        }

        return _returnCollObj;
    }

    // Mesh return
    public void F_ReturMeshObject(GameObject v_returnMesh , bool v_flag )
    {
        // 1. �θ�� 0,0,0�� , ���� 
        v_returnMesh.transform.localPosition = Vector3.zero;
        v_returnMesh.SetActive(false);

        if (v_flag)
            v_returnMesh.transform.parent = _emptyMeshPool.transform;

        // 2. queue�� �ֱ� 
        _emptyMeshQueue.Enqueue(v_returnMesh);
    }

    // Collider return
    public void F_ReturnColliderObject()
    {
        for (int i = 0; i < _emptyColliderPool.transform.childCount; i++)
        {
            GameObject _child = _emptyColliderPool.transform.GetChild(i).gameObject;
            // 1. ����������(pool���� ����������) pool�� �ֱ� 
            if (_child.activeSelf == true)
            {
                // 1. �θ�� 0,0,0�� , ���� 
                _child.transform.localPosition = Vector3.zero;
                _child.SetActive(false);

                // 2. queue�� �ֱ� 
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

            // ȸ�� ��������
            _cantEscapeObjecList[i].transform.rotation = Quaternion.Euler(0,0,0);
        }
    }
}
