using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public enum DropitemName
{
    RECIPE,         // 0
    ITEMBOX_1,      // 1
    ITEMBOX_2,      // 2
    FIBER,          // 3
    SCRAP,          // 4
    PLASTIC         // 5
}

public class DropItemSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] _Prefabs;

    [SerializeField] private Transform _objectParentTransform;

    public GameObject F_GetDropItem(DropitemName v_name)
    {
        GameObject obj = Instantiate(_Prefabs[(int)v_name], _objectParentTransform);
        return obj;
    }
     
    public GameObject F_GetRandomDropItem()
    {
        // 1번은 레시피 -> 레시피를 제외한 나머지 오브젝트
        int rnd = Random.Range(1, _Prefabs.Length);
        GameObject obj = Instantiate(_Prefabs[rnd], _objectParentTransform);
        return obj;
    }

    public void F_RemoveObjects()
    {
        for(int i = 0; i < _objectParentTransform.childCount; i++) 
        {
            Destroy(_objectParentTransform.GetChild(0));
        }
    }
}
