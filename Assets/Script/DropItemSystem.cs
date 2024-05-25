using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public enum DropitemName
{
    RECIPE,
    ITEMBOX
}

public class DropItemSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] _Prefabs;

    [SerializeField] private Transform _objectParentTransform;
    public GameObject F_GetDropItem(DropitemName v_name)
    {
        return Instantiate(_Prefabs[(int)v_name]);
    }
     
    public GameObject F_GetRandomDropItem()
    {
        // 1번은 레시피 -> 레시피를 제외한 나머지 오브젝트
        int rnd = Random.Range(1, _Prefabs.Length);
        return Instantiate(_Prefabs[rnd]);
    }

    public void F_RemoveObjects()
    {
        for (int i = 0; i < _objectParentTransform.childCount; i++)
            Destroy(_objectParentTransform.GetChild(0).gameObject);
    }
}
