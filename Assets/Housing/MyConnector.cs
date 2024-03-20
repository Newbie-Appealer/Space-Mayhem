using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum MyConnectorTpye 
{
    top, left, right, bottom
}

public class MyConnector : MonoBehaviour
{
    public MySelectedBuildType _mySelectBuildType;
    public MyConnectorTpye _myConnectorType;

    public bool _canConnect = true;
    public bool _isConnectWall = false;
    public bool _isConnectFloor = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere( transform.position , transform.lossyScale.x/2 );
    }
}
