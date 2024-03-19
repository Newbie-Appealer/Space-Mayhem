using System.Collections;
using System.Collections.Generic;
//using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public enum ConnectorType
{
    top, bottom, left, right
}

public class Connector : MonoBehaviour
{
    // 각 커넥터 관리

    [Header("=== type ===")]
    public SelectBuildType _selectBuildType;
    public ConnectorType _connectorType;

    [Header("=== Check, if Connect ===")]
    public bool _isConnectToWall  = false;     // 벽이랑 연결 되어있나?
    public bool _isConnectToFloor = false;     // 바닥이랑 연결 되어있나?
    public bool _canConnect        = true;     // 그래서 연결 가능?

    private bool _test1 = true;
    private bool _test2 = true;

    // 커넥터 연결 업데이트
    public void F_ConnectUpdate( bool v_flag = false ) 
    {

        // connect 위치에서, 반지름 만큼 , 커넥터의 레이어(BuildShpere)에서 
        Collider[] colls = Physics.OverlapSphere( transform.position , 1f , 1<<20);

        _isConnectToFloor = false;
        _isConnectToWall = false;

        foreach (Collider _co in colls) 
        {
            // 0. 현재 Connector에 자체 콜라이더랑 내 overlapShere랑 충돌이 일어남 (같은 위치에 있으니까)
            // -> 같은 콜라이더 인지 검사해줘야함
            if ( _co == gameObject.GetComponent<Collider>() )
            {
                continue;
            }

            // 1. 레이어 검사 ( building Sphere )
            if ( _co.gameObject.layer == 20 )
            {
                // 0. Building Manger 에서 혹시나 temp 블럭과 겹칠까봐 예외처리
                if (_co.gameObject.transform.root.name == BuildingManager.instance._nowSelectBlockName + "(Clone)")
                    continue;

                // 0. 나한테 연결되어 있는 상태 커넥터
                Connector _other = _co.GetComponent<Connector>();

                // 2. 상대 build type이 wall 이면?
                if (_other._selectBuildType == SelectBuildType.wall)
                    _isConnectToWall = true;

                // 3. 상태 build type이 floor 이면?
                if (_other._selectBuildType == SelectBuildType.floor)
                    _isConnectToFloor = true;

                // 4. 충돌 일어난 other 커넥터도 업데이트 시켜줘야함
               if (v_flag == true)
                    _other.F_ConnectUpdate();

                // false를 넣으면 other 커넥터 -> 내 커넥터로 넘어오는 일이 없음 (내 connector는 이미 업데이트 되었음)
                // ( + 무한 루프를 막아주기 )
            }
        }

        if (_isConnectToFloor && _isConnectToWall )
            _canConnect = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isConnectToFloor ? ( !_isConnectToWall ? Color.red : Color.blue) : (!_isConnectToWall ? Color.green : Color.yellow);
        Gizmos.DrawWireSphere( transform.position , 0.5f );
    }

}
