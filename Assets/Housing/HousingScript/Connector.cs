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

    public bool _canConnect = true;         // ���⿡ ���� �� �� �ִ���

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, transform.lossyScale.x / 2);      // �� ���� ���� / 2 ( draw ���� ������ �־����)
    }

    public void F_UpdateConnector(bool v_flag = false)    // �⺻�� false �� 
    {
        // �� connector�� ����(�浹) �Ǿ��ִ� �ݶ��̴� �˻�
        Collider[] _otherConnectorCollider = Physics.OverlapSphere(transform.position, transform.lossyScale.x / 2);

        foreach ( Collider _coll in _otherConnectorCollider) 
        {
            if (_coll.gameObject.layer == gameObject.layer) 
            {
                Connector _getConnet = _coll.GetComponent<Connector>();         // �浹ü�� Ŀ���� ��ũ��Ʈ ��������

                if (v_flag)
                    _getConnet.F_UpdateConnector();     // ���� Ŀ���͵� ������Ʈ ���ֱ�

            }

        }

        _canConnect = false;

    }

}
