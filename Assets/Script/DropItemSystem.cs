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

    [SerializeField] private GameObject _objectParent;

    private void Start()
    {
        F_CreateItemGroup();
    }

    public GameObject F_GetDropItem(DropitemName v_name)
    {
        GameObject obj = Instantiate(_Prefabs[(int)v_name], _objectParent.transform);
        return obj;
    }
     
    public GameObject F_GetRandomDropItem()
    {
        // 1���� ������ -> �����Ǹ� ������ ������ ������Ʈ
        int rnd = Random.Range(1, _Prefabs.Length);

        GameObject obj = Instantiate(_Prefabs[rnd], _objectParent.transform);
        return obj;
    }

    public void F_RemoveObjects()
    {
        Destroy(_objectParent); // ������Ʈ ����
        F_CreateItemGroup();    // ������Ʈ ����
    }

    private void F_CreateItemGroup()
    {
        _objectParent = new GameObject();
        _objectParent.name = "DropItemGroup";
        _objectParent.transform.position = Vector3.zero;
    }
}
