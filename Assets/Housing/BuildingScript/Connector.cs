using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class Connector : MonoBehaviour
{
    // �� Ŀ���� ����

    [Header("type")]
    public SelectBuildType _selectBuildType;
    public ConnectorType _connectorType;

    [Header("State")]
    public bool _canConnectToWall  = true;     // ���̶� ���� ����?
    public bool _canConnectToFloor = true;     // �ٴ��̶� ���� ����?
    public bool _canConnect        = true;     // �׷��� ���� ����?

    public void Start()
    {
        F_ConnectUpdate(false);                 // �̹� ��ġ �Ǿ� �ִ� ���� ������ �� connector ������Ʈ�� �������
    }

    // Ŀ���� ���� ������Ʈ
    public void F_ConnectUpdate( bool v_flag ) 
    {
        // �ݶ��̴� �˻� ( ���⼭ ���̾� �˻��ϸ� �ȵ�, ������ )
        // connect ��ġ����, 0,5f ������ ��ŭ , Ŀ������ ���̾�(BuildShpere)���� 
        Collider[] colls = Physics.OverlapSphere( transform.position , 0.5f);

        foreach (Collider _co in colls) 
        {
            // 0. ���� Connector�� ��ü �ݶ��̴��� �� overlapShere�� �浹�� �Ͼ (���� ��ġ�� �����ϱ�)
            // -> ���� �ݶ��̴� ���� �˻��������
            if ( _co == gameObject.GetComponent<Collider>())
                continue;

            // 1. ���̾� �˻�
            if (_co.gameObject.layer == gameObject.layer)
            {
                // 0. ������ ����Ǿ� �ִ� ���� Ŀ����
                Connector _other = _co.GetComponent<Connector>();

                // 2. ��� build type�� wall �̸�?
                if (_other._selectBuildType == SelectBuildType.wall)
                    _canConnectToWall = false;

                // 3. ���� build type�� floor �̸�?
                if (_other._selectBuildType == SelectBuildType.floor)
                    _canConnectToFloor = false;

                // 4. �浹 �Ͼ other Ŀ���͵� ������Ʈ ���������
                if (v_flag == true)
                    _other.F_ConnectUpdate(false);

                // false�� ������ other Ŀ���� -> �� Ŀ���ͷ� �Ѿ���� ���� ���� (�� connector�� �̹� ������Ʈ �Ǿ���)
                // ( + ���� ������ �����ֱ� )
            }
        }

    }

    private void OnDrawGizmos()
    {
        Color temp = Color. yellow;           // �Ѵ� ���� �����ϸ�
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
