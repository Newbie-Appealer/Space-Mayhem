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
    [SerializeField] GameObject _light;

    public void F_RemoveWall(int v_wallToRemove)
    {
        _walls[v_wallToRemove].gameObject.SetActive(false);
    }

    public void F_InstallStair()
    {
        Instantiate(_stair, gameObject.transform.position, Quaternion.identity, transform);
    }

    public void F_InstallLight(int v_index, Vector3 v_nodePos, MazeNode v_nodes)
    {
        if (v_index % 3 == 0)
            Instantiate(_light, new Vector3(v_nodePos.x, v_nodePos.y + 4.2f, v_nodePos.z), Quaternion.identity, v_nodes.transform);
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
