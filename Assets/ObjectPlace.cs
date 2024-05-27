using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class ObjectPlace : MonoBehaviour
{
    private int _item_MIN;      // ������ ���� �ּ� ����
    private int _item_MAX;      // ������ ���� �ִ� ����

    private int _recipe_MIN;    // ������ �߰� ���� �ּ� ���� ( �����濡 ������ 1�� )
    private int _recipe_MAX;    // ������ �߰� ���� �ִ� ����

    private DropItemSystem _dropItemSystem;

    private void Start()
    {
        // �ʱ�ȭ
        _dropItemSystem = ItemManager.Instance.dropItemSystem;

        _item_MIN = 3;       // ������ ���� �ּ� ���� �ʱ�ȭ
        _item_MAX = 10;      // ������ ���� �ִ� ���� �ʱ�ȭ

        _recipe_MIN = 0;     // ������ �߰� ���� �ּ� ���� �ʱ�ȭ ( �����濡 ������ 1�� )
        _recipe_MAX = 2;     // ������ �߰� ���� �ִ� ���� �ʱ�ȭ
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
    }

    /// <summary>
    /// ���� ��ġ �Լ�
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
