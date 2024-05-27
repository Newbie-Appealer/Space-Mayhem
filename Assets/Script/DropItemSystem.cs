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
        // 1���� ������ -> �����Ǹ� ������ ������ ������Ʈ
        int rnd = Random.Range(1, _Prefabs.Length);

        GameObject obj = Instantiate(_Prefabs[rnd], _objectParentTransform);
        return obj;
    }

    public void F_RemoveObjects()
    {
        while(_objectParentTransform.childCount != 0)
        {
            int index = _objectParentTransform.childCount - 1;
            Destroy(_objectParentTransform.GetChild(index));
        }
    }
}
