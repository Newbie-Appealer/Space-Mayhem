using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    AVAILABLE,
    CURRENT,
    COMPLETED
}

public class MazeNode : MonoBehaviour
{
    [SerializeField] GameObject[] _walls;
    [SerializeField] GameObject _stair;

    public void F_RemoveWall(int v_wallToRemove)
    {
        _walls[v_wallToRemove].gameObject.SetActive(false);
    }

    public void F_InstallStair(MazeNode current, MazeNode next)
    {
        Debug.Log("current" + current.transform.position + "next" + next.transform.position);
        Instantiate(_stair, gameObject.transform.position, Quaternion.identity, transform);
    }

    public void SetState(NodeState state)
    {
        switch (state)
        {
            case NodeState.AVAILABLE:
                break;
            case NodeState.CURRENT:
                break;
            case NodeState.COMPLETED:
                break;
        }
    }
}
