using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlace : MonoBehaviour
{
    private int _item_MIN;      // 아이템 생성 최소 개수
    private int _item_MAX;      // 아이템 생성 최대 개수

    private int _recipe_MIN;    // 레시피 추가 생성 최소 개수 ( 도착방에 무조건 1개 )
    private int _recipe_MAX;    // 레시피 추가 생성 최대 개수

    private int _enemy_MIN;     // 몬스터 생성 최소 개수
    private int _enemy_MAX;     // 몬스터 생성 최대 개수

    private DropItemSystem _dropItemSystem;

    private void Start()
    {
        // 초기화
        _dropItemSystem = ItemManager.Instance.dropItemSystem;

        _item_MIN = 20;     // 아이템 생성 최소 개수 초기화
        _item_MAX = 50;     // 아이템 생성 최대 개수 초기화

        _recipe_MIN = 1;    // 레시피 추가 생성 최소 개수 초기화 ( 도착방에 무조건 1개 )
        _recipe_MAX = 2;    // 레시피 추가 생성 최대 개수 초기화

        _enemy_MIN = 2;     // 몬스터 생성 최소 개수
        _enemy_MAX = 10;    // 몬스터 생성 최대 개수
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

        F_PlaceEnemy(ref v_nodes, ref noStairRooms);
    }

    /// <summary>
    /// 몬스터 배치 함수
    /// </summary>
    public void F_PlaceEnemy(ref List<RoomNode> v_nodes, ref List<int> v_noStairIndexs)
    {
        int enemyCount = 5;                 // 몬스터 생성 수


        string[] names = { "SPIDER_BLACK", "SPIDER_SAND" };     // enemy name 배열
        string[] enemyNames = new string[enemyCount];           // 생성할 몬스터의 name 배열
        for(int i = 0; i < enemyCount; i++)
        {
            enemyNames[i] = names[Random.Range(0, names.Length)];
        }

        // 1. Enemy 생성
        List<GameObject> enemys = EnemyManager.Instance.F_GetEnemys(enemyNames);   

        // 2. Enemy 배치
        foreach(GameObject enemy in enemys)
        {
            int roomIndex = Random.Range(0, v_noStairIndexs.Count);
            enemy.transform.position = v_nodes[roomIndex].transform.position;
            enemy.transform.position += new Vector3(2, 2, 2);
        }

        // -. 내부맵 NavMesh Bake
        EnemyManager.Instance.F_NavMeshBake(NavMeshType.INSIDE);
    }

    public Vector3 F_RandomDropPosition()
    {
        float x = Random.Range(-2, 2);
        float z = Random.Range(-2, 2);

        return new Vector3(x, 2f, z);
    }
}
