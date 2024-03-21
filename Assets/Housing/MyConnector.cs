using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyConnector : MonoBehaviour
{
    [Header("���ٰ���?")]
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
        // �� ���� ���� Layer( BuildFinishedBlock)���� �浹 �˻�
        Collider[] _coll = Physics.OverlapSphere( transform.position , 1f , MyBuildManager.instance._finishedLayer);

        // �� connector���� �浹 ����� false��
        foreach ( Collider cl in _coll) 
        {
            Debug.Log(gameObject.transform.root.gameObject.name + " �� ���� / " + gameObject.name );
            _canConnect = false;
            Debug.Log(_canConnect);
        }

        Debug.Log( "������" + _canConnect);
    }
}
