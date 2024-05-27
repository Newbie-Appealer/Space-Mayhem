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
        // 1번은 레시피 -> 레시피를 제외한 나머지 오브젝트
        int rnd = Random.Range(1, _Prefabs.Length);

        GameObject obj = Instantiate(_Prefabs[rnd], _objectParent.transform);
        return obj;
    }

    public void F_RemoveObjects()
    {
        Destroy(_objectParent); // 오브젝트 제거
        F_CreateItemGroup();    // 오브젝트 생성
    }

    private void F_CreateItemGroup()
    {
        _objectParent = new GameObject();
        _objectParent.name = "DropItemGroup";
        _objectParent.transform.position = Vector3.zero;
    }
}
