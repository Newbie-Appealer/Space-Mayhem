using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // BuilingSphere ���̾��� ������Ʈ�� Ʈ�����浹�� ����?
        if(collision.gameObject.layer == BuildingManager.instance._layerMask) 
        {
            Debug.Log(collision.gameObject.name);
        }
    }
}
