using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class InsideMapManager : Singleton<InsideMapManager>
{
    [Header("InsideMap Object")]
    [SerializeField] private RoomNode[] _roomPrefab; //�� ������
    [SerializeField] private GameObject _generateParent; //��� ���� �θ�
    [SerializeField] private GameObject _mapLight; //�� ��ü ����Ʈ
    public GameObject mapLight => _mapLight;
    [HideInInspector] public RoomNode _lastRoom; //������ ��
    [HideInInspector] public RoomNode _startRoom; //���� ��

    [Header("Stair Limit")]
    [SerializeField] private int _stairsLimitCount; //��� ���� ����
    private int[] _stairsLimit; //���� ��� ����
    private int[] _stairsCount; // �� ���� ������ ��� ��

    [Header("Map Size")]
    [SerializeField] private Vector3Int _mazeSize; //���� ���� ũ��
    [SerializeField] private int _roomScale; //�� ũ��

    [SerializeField] List<RoomNode> nodes; //��ü �� ����Ʈ

    private void Start()
    {
        F_BuildRoad();
        _stairsLimit = new int[_mazeSize.y]; //�� �� ����ŭ ����Ʈ ����
        _stairsCount = new int[_mazeSize.y]; //�� �� ����ŭ ����Ʈ ����
        for (int i = 0; i < _mazeSize.y; i++)
        {
            _stairsLimit[i] = _stairsLimitCount;//���� ��� ���� ����
        }
    }

    public void F_BuildRoad()
    {
        nodes = new List<RoomNode>();

        // ��� �� ��������
        for (int y = 0; y < _mazeSize.y; y++)
        {
            for (int x = 0; x < _mazeSize.x; x++)
            {
                for (int z = 0; z < _mazeSize.z; z++)
                {
                    RoomNode newNode;
                    //������ ��� ��ġ
                    Vector3 nodePos = new Vector3(x * (5 * _roomScale), y * (5 * _roomScale) + 500, z * (5 * _roomScale) + 100); 
                    if (x == _mazeSize.x - 1 && y == _mazeSize.y - 1 && z == _mazeSize.z - 1)
                    {
                        //������ �� ����
                        newNode = Instantiate(_roomPrefab[_roomPrefab.Length - 1], nodePos, Quaternion.identity, _generateParent.transform);
                        _lastRoom = newNode;
                    }
                    else
                    {
                        //������ ���� ������ ������ �� ����
                        newNode = Instantiate(_roomPrefab[Random.Range(0, _roomPrefab.Length - 1)], nodePos, Quaternion.identity, _generateParent.transform);
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

            //���� ����� X, Y, Z ��ǥ
            int currentNodeX = (currentNodeIndex % (_mazeSize.x * _mazeSize.z)) / _mazeSize.z;
            int currentNodeY = currentNodeIndex / (_mazeSize.x * _mazeSize.z);
            int currentNodeZ = currentNodeIndex % _mazeSize.z;

            // ���� ����� ������ ��� Ȯ��
            if (currentNodeX < _mazeSize.x - 1)
            {
                int rightNodeIndex = currentNodeIndex + _mazeSize.z;
                if (!clearNodes.Contains(nodes[rightNodeIndex])) //���� ��� �ε����� �̹� �湮�� ��尡 �ƴ϶��
                {
                    possibleDirections.Add(1); //������ �������� �߰�
                    possibleNextNodes.Add(rightNodeIndex); //������ ���� ��忡 �ε��� �߰�
                }
            }

            // ���� ����� ���� ��� Ȯ��
            if (currentNodeX > 0)
            {
                int leftNodeIndex = currentNodeIndex - _mazeSize.z;
                if (!clearNodes.Contains(nodes[leftNodeIndex]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(leftNodeIndex);
                }
            }

            // ���� ����� ���� ��� Ȯ��
            if (currentNodeY < _mazeSize.y - 1)
            {
                int upNodeIndex = currentNodeIndex + _mazeSize.x * _mazeSize.z;
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
                int downNodeIndex = currentNodeIndex - _mazeSize.x * _mazeSize.z; ;
                if (!clearNodes.Contains(nodes[downNodeIndex]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(downNodeIndex);
                }
            }

            // ���� ����� ���� ��� Ȯ��
            if (currentNodeZ < _mazeSize.z - 1)
            {
                int frontNodeIndex = currentNodeIndex + 1;
                if (!clearNodes.Contains(nodes[frontNodeIndex]))
                {
                    possibleDirections.Add(5);
                    possibleNextNodes.Add(frontNodeIndex);
                }
            }

            // ���� ����� ���� ��� Ȯ��
            if (currentNodeZ > 0)
            {
                int backNodeIndex = currentNodeIndex - 1;
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
                int randRoad = Random.Range(1, possibleDirections.Count + 1);
                for (int i = 0; i < randRoad; i++)
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
                            if (_stairsCount[currentNodeY] < _stairsLimit[currentNodeY])
                            {
                                currentNode.F_OnStair(); //���ʰ� �Ʒ����� ��� ����
                                currentNode.F_OffWall(2);
                                nextNode.F_OffWall(3);
                                _stairsCount[currentNodeY]++; // ���� ���� ��� �� ����
                            }
                            break;
                        case 4: // �Ʒ���
                            if (_stairsCount[currentNodeY - 1] < _stairsLimit[currentNodeY - 1])
                            {
                                currentNode.F_OffWall(3);
                                nextNode.F_OffWall(2);
                                nextNode.F_OnStair();
                                _stairsCount[currentNodeY - 1]++; // �Ʒ� ���� ��� �� ����
                            }
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
