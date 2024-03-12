using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // BuilingSphere 레이어의 오브젝트와 트리거충돌이 나면?
        if(collision.gameObject.layer == BuildingManager.instance._layerMask) 
        {
            Debug.Log(collision.gameObject.name);
        }
    }
}
