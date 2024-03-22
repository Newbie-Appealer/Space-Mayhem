using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyConnector : MonoBehaviour
{
    [Header("접근가능?")]
    public bool _canConnect = true;

    private void OnDrawGizmos()
    {
        if(_canConnect == false)
            Gizmos.color = Color.red;

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
        Collider[] _coll = Physics.OverlapSphere( transform.position , 1f);

        foreach ( Collider co in _coll) 
        {
            if (co.gameObject.layer == 23)
            {
                Debug.Log( gameObject.transform.root.name + " 의 하위의 " + gameObject.name + " / 충돌 : " + co.name );
                _canConnect = false;
            }
        }

    }
}
