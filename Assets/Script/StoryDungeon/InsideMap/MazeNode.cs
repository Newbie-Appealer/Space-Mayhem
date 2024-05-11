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

    public void F_InstallStair(int v_scale)
    {
        Instantiate(_stair, gameObject.transform.position, Quaternion.identity).transform.localScale = new Vector3(v_scale, v_scale, v_scale);
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
