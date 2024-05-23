using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum NodeState
//{
//    AVAILABLE,
//    CURRENT,
//    COMPLETED
//}

public class RoomNode : MonoBehaviour
{
    [SerializeField] GameObject[] _walls; //���� ���� �� ����Ʈ
    [SerializeField] GameObject _stair;   //��� ������Ʈ
    [SerializeField] GameObject _light;   //�� ���� ����Ʈ

    public void F_OffWall(int v_wallToRemove)
    {
        _walls[v_wallToRemove].gameObject.SetActive(false);
        //���� ����� �� Active ���� �� ����
    }

    public void F_OnWall()
    {
        for (int i = 0; i < _walls.Length; i++)
            _walls[i].gameObject.SetActive(true);
            //��� �� Active �ѱ�
    }

    public void F_InstallStair()
    {
        Instantiate(_stair, gameObject.transform.position, Quaternion.identity, transform);
        //�ش��ϴ� ��� ��� ��ġ
    }

    public void F_InstallLight(int v_index, Vector3 v_nodePos, RoomNode v_nodes)
    {
        if (v_index % 3 == 0)
            Instantiate(_light, new Vector3(v_nodePos.x, v_nodePos.y + 4.2f, v_nodePos.z), Quaternion.identity, v_nodes.transform);
        //3ĭ�� �ѹ��� ����Ʈ ��ġ
    }

    //public void SetState(NodeState state)
    //{
    //    switch (state)
    //    {
    //        case NodeState.AVAILABLE:
    //            break;
    //        case NodeState.CURRENT:
    //            break;
    //        case NodeState.COMPLETED:
    //            break;
    //    }
    //}
}
