using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectorType 
{
    top, bottom, left, right
}

public class Connector : MonoBehaviour
{
    public SelectBuildType _selectBuildType;
    public ConnectorType _connetortype;

    public bool _canConnect = true;         // 여기에 연결 할 수 있는지

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, transform.lossyScale.x / 2);      // 내 절대 지름 / 2 ( draw 에는 반지름 넣어야함)
    }

    public void F_UpdateConnector(bool v_flag = false)    // 기본은 false 로 
    {
        // 내 connector에 연결(충돌) 되어있는 콜라이더 검사
        Collider[] _otherConnectorCollider = Physics.OverlapSphere(transform.position, transform.lossyScale.x / 2);

        foreach ( Collider _coll in _otherConnectorCollider) 
        {
            if (_coll.gameObject.layer == gameObject.layer) 
            {
                Connector _getConnet = _coll.GetComponent<Connector>();         // 충돌체의 커넥터 스크립트 가져오기

                if (v_flag)
                    _getConnet.F_UpdateConnector();     // 상태 커넥터도 업데이트 해주기

            }

        }

        _canConnect = false;

    }

}
