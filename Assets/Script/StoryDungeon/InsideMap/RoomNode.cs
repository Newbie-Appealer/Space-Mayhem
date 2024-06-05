using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

//public enum NodeState
//{
//    AVAILABLE,
//    CURRENT,
//    COMPLETED
//}

public class RoomNode : MonoBehaviour
{
    [SerializeField] GameObject[] _walls; //각 방의 벽 리스트
    [SerializeField] GameObject[] _doors; //각 방의 문 리스트
    [SerializeField] GameObject _stair;   //각 방의 계단 오브젝트

    public bool noStair => _stair == null;      // 해당 방에 계단 오브젝트의 없는지 여부    ( true : 계단없음 / false : 계단 있음 )
    public bool onStair => _stair.activeSelf;   // 해당 방의 계단이 켜져있는지 여부         ( true : 계단 켜짐 / false : 계단 꺼짐 )  

    public void F_OffWall(int v_wallToRemove)
    {
        _walls[v_wallToRemove].gameObject.SetActive(false);
        //현재 노드의 벽 Active 꺼서 길 만듬
    }

    public void F_OnDoor(int v_doorToCreate)
    {
        _doors[v_doorToCreate].gameObject.SetActive(true);
        //꺼진 벽 위치에 문 설치
    }

    public void F_InitDungeonState()
    {
        for (int i = 0; i < _walls.Length; i++)
        {
            _walls[i].gameObject.SetActive(true);
            //모든 벽 Active true
        }
        for (int i = 0; i < _doors.Length; i++)
        {
            _doors[i].gameObject.SetActive(false);
            //모든 문 Active false
        }
        if (_stair != null)
            _stair.gameObject.SetActive(false);
        else
            return;
    }

    public void F_OnStair()
    {
        _stair.gameObject.SetActive(true);
        //현재 노드 계단 설치
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
