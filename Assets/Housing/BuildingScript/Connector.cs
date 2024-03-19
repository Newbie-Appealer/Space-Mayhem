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
    // �� Ŀ���� ����

    [Header("=== type ===")]
    public SelectBuildType _selectBuildType;
    public ConnectorType _connectorType;

    [Header("=== Check, if Connect ===")]
    public bool _isConnectToWall  = false;     // ���̶� ���� �Ǿ��ֳ�?
    public bool _isConnectToFloor = false;     // �ٴ��̶� ���� �Ǿ��ֳ�?
    public bool _canConnect        = true;     // �׷��� ���� ����?

    private bool _test1 = true;
    private bool _test2 = true;

    // Ŀ���� ���� ������Ʈ
    public void F_ConnectUpdate( bool v_flag = false ) 
    {

        // connect ��ġ����, ������ ��ŭ , Ŀ������ ���̾�(BuildShpere)���� 
        Collider[] colls = Physics.OverlapSphere( transform.position , 1f , 1<<20);

        _isConnectToFloor = false;
        _isConnectToWall = false;

        foreach (Collider _co in colls) 
        {
            // 0. ���� Connector�� ��ü �ݶ��̴��� �� overlapShere�� �浹�� �Ͼ (���� ��ġ�� �����ϱ�)
            // -> ���� �ݶ��̴� ���� �˻��������
            if ( _co == gameObject.GetComponent<Collider>() )
            {
                continue;
            }

            // 1. ���̾� �˻� ( building Sphere )
            if ( _co.gameObject.layer == 20 )
            {
                // 0. Building Manger ���� Ȥ�ó� temp ���� ��ĥ��� ����ó��
                if (_co.gameObject.transform.root.name == BuildingManager.instance._nowSelectBlockName + "(Clone)")
                    continue;

                // 0. ������ ����Ǿ� �ִ� ���� Ŀ����
                Connector _other = _co.GetComponent<Connector>();

                // 2. ��� build type�� wall �̸�?
                if (_other._selectBuildType == SelectBuildType.wall)
                    _isConnectToWall = true;

                // 3. ���� build type�� floor �̸�?
                if (_other._selectBuildType == SelectBuildType.floor)
                    _isConnectToFloor = true;

                // 4. �浹 �Ͼ other Ŀ���͵� ������Ʈ ���������
               if (v_flag == true)
                    _other.F_ConnectUpdate();

                // false�� ������ other Ŀ���� -> �� Ŀ���ͷ� �Ѿ���� ���� ���� (�� connector�� �̹� ������Ʈ �Ǿ���)
                // ( + ���� ������ �����ֱ� )
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
