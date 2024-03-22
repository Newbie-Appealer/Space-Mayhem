using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyConnector : MonoBehaviour
{
    [Header("���ٰ���?")]
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
        // �� ���� ���� Layer( BuildFinishedBlock)���� �浹 �˻�
        Collider[] _coll = Physics.OverlapSphere( transform.position , 1f);

        foreach ( Collider co in _coll) 
        {
            if (co.gameObject.layer == 23)
            {
                Debug.Log( gameObject.transform.root.name + " �� ������ " + gameObject.name + " / �浹 : " + co.name );
                _canConnect = false;
            }
        }

    }
}
