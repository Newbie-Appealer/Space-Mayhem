using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideMapPooling : MonoBehaviour
{
    /// <summary> 
    /// 1. meshRenderer / meshFilter 가지고 있는 게임 오브젝트 생성
    /// 2. collider 가지고 있는 게임 오브젝트 생성
    /// </summary>

    public GameObject _emptyMesh;           // meshRenderer , meshfilter 가지고 있는 오브젝트
    public GameObject _emptyCollider;       // collider 가지고 있는 오브젝트

    public GameObject _emptyMeshPool;       // mesh 의 pool
    public GameObject _emptyColliderPool;   // collider의 pool 

    public Queue<GameObject> _emptyMeshQueue;       // mesh queue
    public Queue<GameObject> _emptyColliderQueue;   // collider queue 

    private void Start()
    {
        _emptyMeshQueue = new Queue<GameObject>();
        _emptyColliderQueue = new Queue<GameObject>();

        F_InitMapPooing();
    }

    // 초기화 
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

    // Mesh Return
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

    // Collider return
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
}
