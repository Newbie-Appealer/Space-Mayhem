using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBuildingBlock : MonoBehaviour
{
    [SerializeField]
    public Transform _connParent;
    [SerializeField]
    public List<MyConnector> buildindConnector;

    private void Start()
    {
        _connParent = gameObject.transform.root.transform.GetChild(1);

        foreach ( MyConnector mc in _connParent.GetComponentsInChildren<MyConnector>()) 
        {
            buildindConnector.Add(mc);
        }
    }

    public Transform F_FintTrsSameToType( MyConnectorTpye v_type) 
    {
        for(int i = 0; i <buildindConnector.Count; i++) 
        {
            if (buildindConnector[i]._myConnectorType == v_type )
                return buildindConnector[i].transform;
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌중");
        // BuildingShpere ( 타입 : trigger , Layer : BuildingSphere ) 에 충돌하면
        if (other.gameObject.CompareTag("BuildingSphere")) 
        {
            MyConnector _other = other.GetComponent<MyConnector>();
            MyBuildManager.instance.F_TempBlockTriggerOther(_other);
        }
    }
}
