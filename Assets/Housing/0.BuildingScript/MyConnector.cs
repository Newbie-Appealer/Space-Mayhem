using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyConnector : MonoBehaviour
{
    [Header("접근가능?")]
    public bool _canConnect = true;

    private void Awake()
    {
        // !! 여기다가 커넥터 업데이트 하면 안됨 -> 순차적으로 블럭이 설치 되는데 그럼 update 못 되는 커넥터가 있을수도잇음  
        //F_UpdateConnector();    
    }

    private void OnDrawGizmos()
    {
        if (_canConnect == false)
            Gizmos.color = Color.red;

        else if (gameObject.layer == 15)         // temp floor 레이어면
            Gizmos.color = Color.yellow;
        else if (gameObject.layer == 16)         // temp ceilling 레이어면
            Gizmos.color = Color.blue;
        else if (gameObject.layer == 17)         // temp wall 레이어면
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.white;         // dontRaycast 레이어 이면
        //Gizmos.DrawWireCube(transform.position, new Vector3(gameObject.transform.lossyScale.x, gameObject.transform.lossyScale.y, gameObject.transform.lossyScale.z));
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 1f, 1f));
    }
    

    public void F_UpdateConnector()
    {
        // 다 지은 블럭의 Layer( BuildFinishedBlock)에서 충돌 검사
        Collider[] _coll = Physics.OverlapSphere( transform.position , 1f);

        foreach ( Collider co in _coll) 
        {
            if (co.gameObject.layer == BuildMaster.Instance.myBuildManger.BuildFinishedLayer)      // Finished building Layer 이면?
            {
                _canConnect = false;
            }
        }
    }

}
