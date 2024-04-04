using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyConnector : MonoBehaviour
{
    [Header("���ٰ���?")]
    public bool _canConnect = true;

    private void Awake()
    {
        // !! ����ٰ� Ŀ���� ������Ʈ �ϸ� �ȵ� -> ���������� ���� ��ġ �Ǵµ� �׷� update �� �Ǵ� Ŀ���Ͱ� ������������  
        //F_UpdateConnector();    
    }

    private void OnDrawGizmos()
    {
        if (_canConnect == false)
            Gizmos.color = Color.red;

        else if (gameObject.layer == 15)         // temp floor ���̾��
            Gizmos.color = Color.yellow;
        else if (gameObject.layer == 16)         // temp ceilling ���̾��
            Gizmos.color = Color.blue;
        else if (gameObject.layer == 17)         // temp wall ���̾��
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.white;         // dontRaycast ���̾� �̸�
        //Gizmos.DrawWireCube(transform.position, new Vector3(gameObject.transform.lossyScale.x, gameObject.transform.lossyScale.y, gameObject.transform.lossyScale.z));
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 1f, 1f));
    }
    

    public void F_UpdateConnector()
    {
        // �� ���� ���� Layer( BuildFinishedBlock)���� �浹 �˻�
        Collider[] _coll = Physics.OverlapSphere( transform.position , 1f);

        foreach ( Collider co in _coll) 
        {
            if (co.gameObject.layer == BuildMaster.Instance.myBuildManger.BuildFinishedLayer)      // Finished building Layer �̸�?
            {
                _canConnect = false;
            }
        }
    }

}
