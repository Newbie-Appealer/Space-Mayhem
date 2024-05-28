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
    [SerializeField] GameObject[] _walls; //�� ���� �� ����Ʈ
    [SerializeField] GameObject[] _doors; //�� ���� �� ����Ʈ
    [SerializeField] GameObject _stair;   //�� ���� ��� ������Ʈ
    [SerializeField] GameObject _light;   //�� ���� ����Ʈ

    public bool noStair => _stair == null;      // �ش� �濡 ��� ������Ʈ�� ������ ����    ( true : ��ܾ��� / false : ��� ���� )
    public bool onStair => _stair.activeSelf;   // �ش� ���� ����� �����ִ��� ����         ( true : ��� ���� / false : ��� ���� )  

    public void F_OffWall(int v_wallToRemove)
    {
        _walls[v_wallToRemove].gameObject.SetActive(false);
        //���� ����� �� Active ���� �� ����
    }

    public void F_OnDoor(int v_doorToCreate)
    {
        _doors[v_doorToCreate].gameObject.SetActive(true);
        //���� �� ��ġ�� �� ��ġ
    }

    public void F_InitDungeonState()
    {
        for (int i = 0; i < _walls.Length; i++)
            _walls[i].gameObject.SetActive(true);
            //��� �� Active true
        for (int i = 0; i < _doors.Length; i++)
        {
            if (_doors[i].activeSelf)
                _doors[i].gameObject.SetActive(false);
            //��� �� Active false
        }
    }

    public void F_OnStair()
    {
        _stair.gameObject.SetActive(true);
        //���� ��� ��� ��ġ
    }

    public void F_OnLight(int v_index)
    {
        if (v_index % 3 == 0)
            _light.gameObject.SetActive(true);
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
