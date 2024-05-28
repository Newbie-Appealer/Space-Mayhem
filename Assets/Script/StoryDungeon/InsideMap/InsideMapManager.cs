using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class InsideMapManager : Singleton<InsideMapManager>
{
    [Header("Map Object")]
    [SerializeField] RoomNode[] _roomPrefab; //�� ������
    [SerializeField] GameObject _generateParent; //��� ���� �θ�
    [SerializeField] GameObject _mapLight; //�� ��ü ����Ʈ
    public RoomNode _lastRoom; //������ ��
    public RoomNode _startRoom; //���� ��
    public GameObject mapLight => _mapLight;

    [Header("Map Size")]
    [SerializeField] Vector3Int _mazeSize; //���� ���� ũ��
    [SerializeField] int _roomScale; //�� ũ��

    List<RoomNode> nodes; //��ü �� ����Ʈ


    private void Start()
    {
        F_BuildRoad();
    }

    public void F_BuildRoad()
    {
        nodes = new List<RoomNode>();

        // ��� �� ��������
        for (int x = 0; x < _mazeSize.x; x++)
        {
            for (int y = 0; y < _mazeSize.y; y++)
            {
                for (int z = 0; z < _mazeSize.z; z++)
                {
                    RoomNode newNode;
                    Vector3 nodePos = new Vector3(x * (5 * _roomScale), y * (5 * _roomScale) + 500, z * (5 * _roomScale) + 100); //��� ��ġ
                    int nodeIndex = x * (_mazeSize.y * _mazeSize.z) + y * _mazeSize.z + z; //�迭 �ε���
                    if (x == _mazeSize.x - 1 && y == _mazeSize.y - 1 && z == _mazeSize.z - 1)
                    {
                        //������ �� ����
                        newNode = Instantiate(_roomPrefab[_roomPrefab.Length - 1], nodePos, Quaternion.identity, _generateParent.transform);
                        //����Ʈ ��ġ
                        //newNode.F_OnLight(nodeIndex);
                        _lastRoom = newNode;
                    }
                    else
                    {
                        //������ ���� ������ ������ �� ����
                        newNode = Instantiate(_roomPrefab[Random.Range(0, _roomPrefab.Length - 1)], nodePos, Quaternion.identity, _generateParent.transform);
                        //newNode.F_OnLight(nodeIndex);
                        if (x == 0 && y == 0 && z == 0)
                            _startRoom = newNode;
                    }
                    //�� ũ�� ����
                    newNode.transform.localScale = new Vector3(_roomScale, _roomScale, _roomScale);
                    nodes.Add(newNode);
                }
            }
        }
        //�����ϰ� ������
        _generateParent.SetActive(false);
    }

    public void F_GenerateInsideMap()
    {
        //�Լ��� ����Ǹ� �ѱ�
        _generateParent.SetActive(true);

        // �̹� �湮�� ��带 �����ϱ� ���� ����Ʈ
        List<RoomNode> clearNodes = new List<RoomNode>();

        // Ž���� ������ ������ ����
        Stack<RoomNode> nodeStack = new Stack<RoomNode>();
        nodeStack.Push(nodes[0]);

        // Ž�� ����
        while (nodeStack.Count > 0)
        {
            RoomNode currentNode = nodeStack.Pop(); //nodeStack ���������� �� ��Ұ� ���� ��尡 ��.
            clearNodes.Add(currentNode);

            // ���� ��� ������ ������ ���� ������ ã��
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentNodeIndex = nodes.IndexOf(currentNode); //��ü ��� ����Ʈ���� ���� ���� ��ġ�ϴ� �ε���
            int currentNodeX = currentNodeIndex / (_mazeSize.y * _mazeSize.z);
            int currentNodeY = (currentNodeIndex % (_mazeSize.y * _mazeSize.z)) / _mazeSize.z;
            int currentNodeZ = currentNodeIndex % _mazeSize.z;

            // ���� ����� ������ ��� Ȯ��
            if (currentNodeX < _mazeSize.x - 1)
            {
                int rightNodeIndex = (currentNodeX + 1) * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[rightNodeIndex])) //���� ��� �ε����� �̹� �湮�� ��尡 �ƴ϶��
                {
                    possibleDirections.Add(1); //������ �������� �߰�
                    possibleNextNodes.Add(rightNodeIndex); //������ ���� ��忡 �ε��� �߰�
                }
            }

            // ���� ����� ���� ��� Ȯ��
            if (currentNodeX > 0)
            {
                int leftNodeIndex = (currentNodeX - 1) * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[leftNodeIndex]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(leftNodeIndex);
                }
            }

            // ���� ����� ���� ��� Ȯ��
            if (currentNodeY < _mazeSize.y - 1)
            {
                int upNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + (currentNodeY + 1) * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[upNodeIndex]))
                {
                    if (currentNodeIndex != 0) //ù ���� ���� ��� ����
                    {
                        possibleDirections.Add(3);
                        possibleNextNodes.Add(upNodeIndex);
                    }
                }
            }

            // ���� ����� �Ʒ��� ��� Ȯ��
            if (currentNodeY > 0)
            {
                int downNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + (currentNodeY - 1) * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[downNodeIndex]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(downNodeIndex);
                }
            }

            // ���� ����� ���� ��� Ȯ��
            if (currentNodeZ < _mazeSize.z - 1)
            {
                int frontNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ + 1;
                if (!clearNodes.Contains(nodes[frontNodeIndex]))
                {
                    possibleDirections.Add(5);
                    possibleNextNodes.Add(frontNodeIndex);
                }
            }

            // ���� ����� ���� ��� Ȯ��
            if (currentNodeZ > 0)
            {
                int backNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ - 1;
                if (!clearNodes.Contains(nodes[backNodeIndex]))
                {
                    possibleDirections.Add(6);
                    possibleNextNodes.Add(backNodeIndex);
                }
            }

            // ������ ������ �ִٸ�
            if (possibleDirections.Count > 0)
            {
                // ������ ����� ��带 �������� ����
                F_RandomDirection(possibleDirections, possibleNextNodes);

                // 1���� ���� ���� ���� ������ ����
                for (int i = 0; i < Random.Range(1, possibleDirections.Count); i++)
                {
                    int direction = possibleDirections[i];
                    RoomNode nextNode = nodes[possibleNextNodes[i]];

                    switch (direction)
                    {
                        case 1: // ������
                            currentNode.F_OffWall(0);
                            currentNode.F_OnDoor(0); //���� ��� �� ����
                            nextNode.F_OffWall(1); //���� ���� ���� ��� ������ �� ����
                            break;
                        case 2: // ����
                            currentNode.F_OffWall(1);
                            currentNode.F_OnDoor(1);
                            nextNode.F_OffWall(0);
                            break;
                        case 3: // ����
                            currentNode.F_OnStair(); //���ʰ� �Ʒ����� ��� ����
                            currentNode.F_OffWall(2);
                            nextNode.F_OffWall(3);
                            break;
                        case 4: // �Ʒ���
                            currentNode.F_OffWall(3);
                            nextNode.F_OnStair();
                            nextNode.F_OffWall(2);
                            break;
                        case 5: // ����
                            currentNode.F_OffWall(4);
                            currentNode.F_OnDoor(2);
                            nextNode.F_OffWall(5);
                            break;
                        case 6: // ����
                            currentNode.F_OffWall(5);
                            currentNode.F_OnDoor(3);
                            nextNode.F_OffWall(4);
                            break;
                    }

                    // ���ÿ� ���� ��� �߰�
                    nodeStack.Push(nextNode);
                }
            }
        }

        // ������Ʈ ��ġ �׽�Ʈ
        this.GetComponent<ObjectPlace>().F_PlaceItem(ref nodes);
    }

    // �� ����Ʈ�� ������ ������ ������ ���� ���� �Լ�
    private void F_RandomDirection(List<int> directions, List<int> nodes)
    {
        for (int i = directions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // directions ����Ʈ���� ��� ��ȯ
            int tempDirection = directions[i];
            directions[i] = directions[randomIndex];
            directions[randomIndex] = tempDirection;

            // nodes ����Ʈ���� ��� ��ȯ
            int tempNode = nodes[i];
            nodes[i] = nodes[randomIndex];
            nodes[randomIndex] = tempNode;
        }
    }

    public void F_DestroyInsideMap()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].F_InitDungeonState();
        }
        _generateParent.SetActive(false); //���� ���� ����
    }

    protected override void InitManager()
    {
        return;
    }
}
