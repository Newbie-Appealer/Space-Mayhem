using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class ObjectPlace : MonoBehaviour
{
    private int _item_MIN;      // 아이템 생성 최소 개수
    private int _item_MAX;      // 아이템 생성 최대 개수

    private int _recipe_MIN;    // 레시피 추가 생성 최소 개수 ( 도착방에 무조건 1개 )
    private int _recipe_MAX;    // 레시피 추가 생성 최대 개수

    private DropItemSystem _dropItemSystem;

    private void Start()
    {
        // 초기화
        _dropItemSystem = ItemManager.Instance.dropItemSystem;

        _item_MIN = 3;       // 아이템 생성 최소 개수 초기화
        _item_MAX = 10;      // 아이템 생성 최대 개수 초기화

        _recipe_MIN = 0;     // 레시피 추가 생성 최소 개수 초기화 ( 도착방에 무조건 1개 )
        _recipe_MAX = 2;     // 레시피 추가 생성 최대 개수 초기화
}

    /// <summary>
    /// 아이템 배치 함수
    /// </summary>
    public void F_PlaceItem(ref List<RoomNode> v_nodes)
    {
        // 아이템 / 레시피 생성 개수
        int itemCount   = Random.Range(_item_MIN, _item_MAX);
        int recipeCount = Random.Range(_recipe_MIN, _recipe_MAX);

        // v_nodex의 계단없는 방 index List
        List<int> noStairRooms = new List<int>();
        for(int index = 0; index < v_nodes.Count; index++)
        {
            // 방이 없을때
            if (v_nodes[index] == null)
                continue;

            // 계단이 없을때
            if (v_nodes[index].noStair)
                continue;

            // v_nodex[index] 방의 계단이 꺼져있을떄
            if(!v_nodes[index].onStair)
                noStairRooms.Add(index);
        }

        // 아이템을 배치할수있는 방이 하나도 없을때 예외처리
        if (noStairRooms.Count == 0)
            return;

        // 아이템 배치
        for(int i = 0; i < itemCount; i++)
        {
            int roomIndex = Random.Range(0, noStairRooms.Count);            // 랜덤 방
            GameObject obj = _dropItemSystem.F_GetRandomDropItem();         // 오브젝트 생성
            obj.transform.position = v_nodes[roomIndex].transform.position; // 위치 설정
            obj.transform.position += F_RandomDropPosition();               // 위치 변경 ( -2 ~ 2 )
        }

        // 아이템 배치
        for (int i = 0; i < recipeCount; i++)
        {
            int roomIndex = Random.Range(0, noStairRooms.Count);                 // 랜덤 방
            GameObject obj = _dropItemSystem.F_GetDropItem(DropitemName.RECIPE); // 오브젝트 생성
            obj.transform.position = v_nodes[roomIndex].transform.position;      // 위치 설정
            obj.transform.position += F_RandomDropPosition();                    // 위치 변경 ( -2 ~ 2 )
        }
    }

    /// <summary>
    /// 몬스터 배치 함수
    /// </summary>
    public void F_PlaceEnemy(ref List<RoomNode> v_nodes)
    {

    }

    public Vector3 F_RandomDropPosition()
    {
        float x = Random.Range(-2, 2);
        float z = Random.Range(-2, 2);

        return new Vector3(x, 2f, z);
    }
}
