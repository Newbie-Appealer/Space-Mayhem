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
    [SerializeField] GameObject[] _walls; //현재 방의 벽 리스트
    [SerializeField] GameObject _stair;   //계단 오브젝트
    [SerializeField] GameObject _light;   //각 방의 라이트

    public void F_OffWall(int v_wallToRemove)
    {
        _walls[v_wallToRemove].gameObject.SetActive(false);
        //현재 노드의 벽 Active 꺼서 길 만듬
    }

    public void F_OnWall()
    {
        for (int i = 0; i < _walls.Length; i++)
            _walls[i].gameObject.SetActive(true);
            //모든 벽 Active 켜기
    }

    public void F_InstallStair()
    {
        Instantiate(_stair, gameObject.transform.position, Quaternion.identity, transform);
        //해당하는 노드 계단 설치
    }

    public void F_InstallLight(int v_index, Vector3 v_nodePos, RoomNode v_nodes)
    {
        if (v_index % 3 == 0)
            Instantiate(_light, new Vector3(v_nodePos.x, v_nodePos.y + 4.2f, v_nodePos.z), Quaternion.identity, v_nodes.transform);
        //3칸에 한번씩 라이트 설치
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
