using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlace : MonoBehaviour
{
    private int _item_MIN;      // ������ ���� �ּ� ����
    private int _item_MAX;      // ������ ���� �ִ� ����

    private int _recipe_MIN;    // ������ �߰� ���� �ּ� ���� ( �����濡 ������ 1�� )
    private int _recipe_MAX;    // ������ �߰� ���� �ִ� ����

    private int _enemy_MIN;     // ���� ���� �ּ� ����
    private int _enemy_MAX;     // ���� ���� �ִ� ����

    private DropItemSystem _dropItemSystem;

    private void Start()
    {
        // �ʱ�ȭ
        _dropItemSystem = ItemManager.Instance.dropItemSystem;

        _item_MIN = 20;     // ������ ���� �ּ� ���� �ʱ�ȭ
        _item_MAX = 50;     // ������ ���� �ִ� ���� �ʱ�ȭ

        _recipe_MIN = 1;    // ������ �߰� ���� �ּ� ���� �ʱ�ȭ ( �����濡 ������ 1�� )
        _recipe_MAX = 2;    // ������ �߰� ���� �ִ� ���� �ʱ�ȭ

        _enemy_MIN = 2;     // ���� ���� �ּ� ����
        _enemy_MAX = 10;    // ���� ���� �ִ� ����
    }

    /// <summary>
    /// ������ ��ġ �Լ�
    /// </summary>
    public void F_PlaceItem(ref List<RoomNode> v_nodes)
    {
        // ������ / ������ ���� ����
        int itemCount   = Random.Range(_item_MIN, _item_MAX);
        int recipeCount = Random.Range(_recipe_MIN, _recipe_MAX);

        // v_nodex�� ��ܾ��� �� index List
        List<int> noStairRooms = new List<int>();
        for(int index = 0; index < v_nodes.Count; index++)
        {
            // ���� ������
            if (v_nodes[index] == null)
                continue;

            // ����� ������
            if (v_nodes[index].noStair)
                continue;

            // v_nodex[index] ���� ����� ����������
            if(!v_nodes[index].onStair)
                noStairRooms.Add(index);
        }

        // �������� ��ġ�Ҽ��ִ� ���� �ϳ��� ������ ����ó��
        if (noStairRooms.Count == 0)
            return;

        // ������ ��ġ
        for(int i = 0; i < itemCount; i++)
        {
            int roomIndex = Random.Range(0, noStairRooms.Count);            // ���� ��
            GameObject obj = _dropItemSystem.F_GetRandomDropItem();         // ������Ʈ ����
            obj.transform.position = v_nodes[roomIndex].transform.position; // ��ġ ����
            obj.transform.position += F_RandomDropPosition();               // ��ġ ���� ( -2 ~ 2 )
        }

        // ������ ��ġ
        for (int i = 0; i < recipeCount; i++)
        {
            int roomIndex = Random.Range(0, noStairRooms.Count);                 // ���� ��
            GameObject obj = _dropItemSystem.F_GetDropItem(DropitemName.RECIPE); // ������Ʈ ����
            obj.transform.position = v_nodes[roomIndex].transform.position;      // ��ġ ����
            obj.transform.position += F_RandomDropPosition();                    // ��ġ ���� ( -2 ~ 2 )
        }

        F_PlaceEnemy(ref v_nodes, ref noStairRooms);
    }

    /// <summary>
    /// ���� ��ġ �Լ�
    /// </summary>
    public void F_PlaceEnemy(ref List<RoomNode> v_nodes, ref List<int> v_noStairIndexs)
    {
        int enemyCount = 5;                 // ���� ���� ��


        string[] names = { "SPIDER_BLACK", "SPIDER_SAND" };     // enemy name �迭
        string[] enemyNames = new string[enemyCount];           // ������ ������ name �迭
        for(int i = 0; i < enemyCount; i++)
        {
            enemyNames[i] = names[Random.Range(0, names.Length)];
        }

        // 1. Enemy ����
        List<GameObject> enemys = EnemyManager.Instance.F_GetEnemys(enemyNames);   

        // 2. Enemy ��ġ
        foreach(GameObject enemy in enemys)
        {
            int roomIndex = Random.Range(0, v_noStairIndexs.Count);
            enemy.transform.position = v_nodes[roomIndex].transform.position;
            enemy.transform.position += new Vector3(2, 2, 2);
        }

        // -. ���θ� NavMesh Bake
        EnemyManager.Instance.F_NavMeshBake(NavMeshType.INSIDE);
    }

    public Vector3 F_RandomDropPosition()
    {
        float x = Random.Range(-2, 2);
        float z = Random.Range(-2, 2);

        return new Vector3(x, 2f, z);
    }
}
