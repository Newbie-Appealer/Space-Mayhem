using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class OutsideMapPooling : MonoBehaviour
{
    /// <summary> 
    /// 1. meshRenderer / meshFilter ������ �ִ� ���� ������Ʈ ����
    /// 2. collider ������ �ִ� ���� ������Ʈ ����
    /// 
    /// </summary>

    [Header("meshRender, meshfilter")]
    public GameObject _emptyMesh;           // meshRenderer , meshfilter ������ �ִ� ������Ʈ
    public GameObject _emptyCollider;       // collider ������ �ִ� ������Ʈ

    public GameObject _emptyMeshPool;       // mesh �� pool
    public GameObject _emptyColliderPool;   // collider�� pool 

    public Queue<GameObject> _emptyMeshQueue;       // mesh queue
    public Queue<GameObject> _emptyColliderQueue;   // collider queue 

    [Header("PlanetObect")]
    public GameObject _planetObjectTransform;           // planet ������Ʈ ��Ƴ��� �θ� 
    public List<List<GameObject>> _planetsObjectList;

    private void Start()
    {
        // meshRender , collider queue �ʱ�ȭ
        _emptyMeshQueue     = new Queue<GameObject>();
        _emptyColliderQueue = new Queue<GameObject>();

        // mesh, collider pool �ʱ�ȭ
        F_InitMapPooing();

        // �༺ ������Ʈ pool �ʱ�ȭ
        F_InitPlanetObject();

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
            GameObject[] _obj = Resources.LoadAll<GameObject>(_path);       // ��ο� �ִ� ������Ʈ �� �������

            List<GameObject> _tempPlanet = new List<GameObject>();                 // list�� ����
            for (int j = 0; j < _obj.Length; j++) 
            { 
                GameObject _t = Instantiate( _obj[j] );
                _t.transform.parent         = _planetObjectTransform.transform;
                _t.transform.localPosition  = Vector3.zero;  
                _t.SetActive(false);

                _tempPlanet.Add( _t );
            }

            _planetsObjectList.Add( _tempPlanet );
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
}