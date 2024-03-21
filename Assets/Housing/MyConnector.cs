using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyConnector : MonoBehaviour
{
    [Header("접근가능?")]
    public bool _canConnect = true;

    private void Start()
    {
        _canConnect = true;
    }

    private void OnDrawGizmos()
    {
        if (gameObject.layer == 15)
        {
            Gizmos.color = Color.yellow;
        }
        
        else 
        { 
            Gizmos.color = Color.green;
        }
        Gizmos.DrawWireCube(transform.position, new Vector3(gameObject.transform.lossyScale.x, gameObject.transform.lossyScale.y, gameObject.transform.lossyScale.z));
    }

    public void F_UpdateConnector()
    {
        // 다 지은 블럭의 Layer( BuildFinishedBlock)에서 충돌 검사
        Collider[] _coll = Physics.OverlapSphere( transform.position , 1f , MyBuildManager.instance._finishedLayer);

        // 내 connector에서 충돌 생기면 false로
        foreach ( Collider cl in _coll) 
        {
            Debug.Log(gameObject.transform.root.gameObject.name + " 의 하위 / " + gameObject.name );
            _canConnect = false;
            Debug.Log(_canConnect);
        }

        Debug.Log( "끝나고" + _canConnect);
    }
}
