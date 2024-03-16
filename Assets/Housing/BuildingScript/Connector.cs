using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class Connector : MonoBehaviour
{
    // 각 커넥터 관리

    [Header("type")]
    public SelectBuildType _selectBuildType;
    public ConnectorType _connectorType;

    [Header("State")]
    public bool _canConnectToWall  = true;     // 벽이랑 연결 가능?
    public bool _canConnectToFloor = true;     // 바닥이랑 연결 가능?
    public bool _canConnect        = true;     // 그래서 연결 가능?

    public void Start()
    {
        F_ConnectUpdate(false);                 // 이미 설치 되어 있는 블럭도 시작할 때 connector 업데이트를 해줘야함
    }

    // 커넥터 연결 업데이트
    public void F_ConnectUpdate( bool v_flag ) 
    {
        // 콜라이더 검사 ( 여기서 레이어 검사하면 안됨, 오류남 )
        // connect 위치에서, 0,5f 반지름 만큼 , 커넥터의 레이어(BuildShpere)에서 
        Collider[] colls = Physics.OverlapSphere( transform.position , 0.5f);

        foreach (Collider _co in colls) 
        {
            // 0. 현재 Connector에 자체 콜라이더랑 내 overlapShere랑 충돌이 일어남 (같은 위치에 있으니까)
            // -> 같은 콜라이더 인지 검사해줘야함
            if ( _co == gameObject.GetComponent<Collider>())
                continue;

            // 1. 레이어 검사
            if (_co.gameObject.layer == gameObject.layer)
            {
                // 0. 나한테 연결되어 있는 상태 커넥터
                Connector _other = _co.GetComponent<Connector>();

                // 2. 상대 build type이 wall 이면?
                if (_other._selectBuildType == SelectBuildType.wall)
                    _canConnectToWall = false;

                // 3. 상태 build type이 floor 이면?
                if (_other._selectBuildType == SelectBuildType.floor)
                    _canConnectToFloor = false;

                // 4. 충돌 일어난 other 커넥터도 업데이트 시켜줘야함
                if (v_flag == true)
                    _other.F_ConnectUpdate(false);

                // false를 넣으면 other 커넥터 -> 내 커넥터로 넘어오는 일이 없음 (내 connector는 이미 업데이트 되었음)
                // ( + 무한 루프를 막아주기 )
            }
        }

    }

    private void OnDrawGizmos()
    {
        Color temp = Color. yellow;           // 둘다 연결 가능하면
        if (_canConnectToWall && !_canConnectToFloor)
            temp = Color.blue;
        else if (!_canConnectToWall && _canConnectToFloor)
            temp = Color.green;
        else if (!_canConnectToFloor && !_canConnectToFloor)
            temp = Color.red;

        Gizmos.color = temp;
        Gizmos.DrawWireSphere( transform.position , 0.5f );
    }

}
