using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class InsideMapManager : Singleton<InsideMapManager>
{
    [Header("InsideMap Object")]
    [SerializeField] private RoomNode[] _roomPrefab; //방 프리펩
    [SerializeField] private GameObject _generateParent; //모든 방의 부모
    [SerializeField] private GameObject _mapLight; //맵 전체 라이트
    public GameObject mapLight => _mapLight;
    [HideInInspector] public RoomNode _startRoom; //시작 방
    [SerializeField] GameObject _DungeonPortal;

    [Header("Stair Limit")]
    [SerializeField] private int _stairsLimitCount; //계단 제한 개수

    [Header("Map Size")]
    [SerializeField] private Vector3Int _mazeSize; //내부 던전 크기
    [SerializeField] private int _roomScale; //방 크기

    private List<RoomNode> nodes; //전체 방 리스트

    private void Start()
    {
        F_BuildRoad();

    }

    public void F_BuildRoad()
    {
        nodes = new List<RoomNode>();

        // 모든 방 만들어놓음
        for (int y = 0; y < _mazeSize.y; y++)
        {
            for (int x = 0; x < _mazeSize.x; x++)
            {
                for (int z = 0; z < _mazeSize.z; z++)
                {
                    RoomNode newNode;
                    //생성될 노드 위치
                    Vector3 nodePos = new Vector3(x * (5 * _roomScale), y * (5 * _roomScale) + 500, z * (5 * _roomScale) + 100);

                    //방 생성
                    newNode = Instantiate(_roomPrefab[Random.Range(0, _roomPrefab.Length - 1)], nodePos, Quaternion.identity, _generateParent.transform);

                    if (x == 0 && y == 0 && z == 0)
                        _startRoom = newNode; //시작 방 지정

                    //방 크기 지정
                    newNode.transform.localScale = new Vector3(_roomScale, _roomScale, _roomScale);
                    nodes.Add(newNode);
                }
            }
        }

        //던전 포탈 생성
        Instantiate(_DungeonPortal, new Vector3(nodes[nodes.Count - 3].transform.position.x, 
                                                nodes[nodes.Count - 3].transform.position.y + 0.24f, 
                                                nodes[nodes.Count - 3].transform.position.z), Quaternion.identity, nodes[nodes.Count - 3].transform);
        //생성하고 꺼놓기
        _generateParent.SetActive(false);
    }

    public void F_GenerateInsideMap()
    {
        _generateParent.SetActive(true);
        List<RoomNode> clearNodes = new List<RoomNode>();
        Stack<RoomNode> nodeStack = new Stack<RoomNode>();
        nodeStack.Push(nodes[0]);

        int[] _stairsLimit = new int[_mazeSize.y];
        int[] _stairsCount = new int[_mazeSize.y];

        for (int i = 0; i < _mazeSize.y; i++)
        {
            _stairsLimit[i] = _stairsLimitCount;
        }

        while (nodeStack.Count > 0)
        {
            RoomNode currentNode = nodeStack.Pop();
            clearNodes.Add(currentNode);

            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentNodeIndex = nodes.IndexOf(currentNode);

            int currentNodeX = (currentNodeIndex % (_mazeSize.x * _mazeSize.z)) / _mazeSize.z;
            int currentNodeY = currentNodeIndex / (_mazeSize.x * _mazeSize.z);
            int currentNodeZ = currentNodeIndex % _mazeSize.z;

            if (currentNodeX < _mazeSize.x - 1)
            {
                int rightNodeIndex = currentNodeIndex + _mazeSize.z;
                if (!clearNodes.Contains(nodes[rightNodeIndex]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(rightNodeIndex);
                }
            }

            if (currentNodeX > 0)
            {
                int leftNodeIndex = currentNodeIndex - _mazeSize.z;
                if (!clearNodes.Contains(nodes[leftNodeIndex]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(leftNodeIndex);
                }
            }

            if (currentNodeY < _mazeSize.y - 1)
            {
                int upNodeIndex = currentNodeIndex + _mazeSize.x * _mazeSize.z;
                if (!clearNodes.Contains(nodes[upNodeIndex]) && _stairsCount[currentNodeY] < _stairsLimit[currentNodeY])
                {
                    if (currentNodeIndex != 0)
                    {
                        possibleDirections.Add(3);
                        possibleNextNodes.Add(upNodeIndex);
                    }
                }
            }

            if (currentNodeY > 0)
            {
                int downNodeIndex = currentNodeIndex - _mazeSize.x * _mazeSize.z;
                if (!clearNodes.Contains(nodes[downNodeIndex]) && _stairsCount[currentNodeY - 1] < _stairsLimit[currentNodeY - 1])
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(downNodeIndex);
                }
            }

            if (currentNodeZ < _mazeSize.z - 1)
            {
                int frontNodeIndex = currentNodeIndex + 1;
                if (!clearNodes.Contains(nodes[frontNodeIndex]))
                {
                    possibleDirections.Add(5);
                    possibleNextNodes.Add(frontNodeIndex);
                }
            }

            if (currentNodeZ > 0)
            {
                int backNodeIndex = currentNodeIndex - 1;
                if (!clearNodes.Contains(nodes[backNodeIndex]))
                {
                    possibleDirections.Add(6);
                    possibleNextNodes.Add(backNodeIndex);
                }
            }

            if (possibleDirections.Count > 0)
            {
                F_RandomDirection(possibleDirections, possibleNextNodes);
                int randRoad = Random.Range(1, possibleDirections.Count + 1);

                for (int i = 0; i < randRoad; i++)
                {
                    int direction = possibleDirections[i];
                    RoomNode nextNode = nodes[possibleNextNodes[i]];

                    bool createdPath = false;

                    switch (direction)
                    {
                        case 1:
                            currentNode.F_OffWall(0);
                            currentNode.F_OnDoor(0);
                            nextNode.F_OffWall(1);
                            createdPath = true;
                            break;
                        case 2:
                            currentNode.F_OffWall(1);
                            currentNode.F_OnDoor(1);
                            nextNode.F_OffWall(0);
                            createdPath = true;
                            break;
                        case 3:
                            if (_stairsCount[currentNodeY] < _stairsLimit[currentNodeY])
                            {
                                currentNode.F_OnStair();
                                currentNode.F_OffWall(2);
                                nextNode.F_OffWall(3);
                                _stairsCount[currentNodeY]++;
                                createdPath = true;
                            }
                            break;
                        case 4:
                            if (_stairsCount[currentNodeY - 1] < _stairsLimit[currentNodeY - 1])
                            {
                                currentNode.F_OffWall(3);
                                nextNode.F_OffWall(2);
                                nextNode.F_OnStair();
                                _stairsCount[currentNodeY - 1]++;
                                createdPath = true;
                            }
                            break;
                        case 5:
                            currentNode.F_OffWall(4);
                            currentNode.F_OnDoor(2);
                            nextNode.F_OffWall(5);
                            createdPath = true;
                            break;
                        case 6:
                            currentNode.F_OffWall(5);
                            currentNode.F_OnDoor(3);
                            nextNode.F_OffWall(4);
                            createdPath = true;
                            break;
                    }

                    if (createdPath)
                    {
                        nodeStack.Push(nextNode);
                    }
                    else if ((direction == 3 && !createdPath) || (direction == 4 && !createdPath))
                    {
                        // 계단 제한 때문에 길이 끊길 경우, 대체 경로 생성
                        F_CreateRoad(currentNode, currentNodeX, currentNodeY, currentNodeZ, clearNodes);
                    }
                }
            }
        }
        this.GetComponent<ObjectPlace>().F_PlaceItem(ref nodes);
    }

    private void F_CreateRoad(RoomNode currentNode, int currentNodeX, int currentNodeY, int currentNodeZ, List<RoomNode> clearNodes)
    {
        List<int> fallbackDirections = new List<int> { 1, 2, 5, 6 };
        List<int> fallbackNodes = new List<int>();

        // 대체 경로를 위한 노드 결정
        if (currentNodeX < _mazeSize.x - 1 && !clearNodes.Contains(nodes[currentNodeY * _mazeSize.x * _mazeSize.z + (currentNodeX + 1) * _mazeSize.z + currentNodeZ]))
        {
            fallbackNodes.Add(currentNodeY * _mazeSize.x * _mazeSize.z + (currentNodeX + 1) * _mazeSize.z + currentNodeZ);
        }
        if (currentNodeX > 0 && !clearNodes.Contains(nodes[currentNodeY * _mazeSize.x * _mazeSize.z + (currentNodeX - 1) * _mazeSize.z + currentNodeZ]))
        {
            fallbackNodes.Add(currentNodeY * _mazeSize.x * _mazeSize.z + (currentNodeX - 1) * _mazeSize.z + currentNodeZ);
        }
        if (currentNodeZ < _mazeSize.z - 1 && !clearNodes.Contains(nodes[currentNodeY * _mazeSize.x * _mazeSize.z + currentNodeX * _mazeSize.z + (currentNodeZ + 1)]))
        {
            fallbackNodes.Add(currentNodeY * _mazeSize.x * _mazeSize.z + currentNodeX * _mazeSize.z + (currentNodeZ + 1));
        }
        if (currentNodeZ > 0 && !clearNodes.Contains(nodes[currentNodeY * _mazeSize.x * _mazeSize.z + currentNodeX * _mazeSize.z + (currentNodeZ - 1)]))
        {
            fallbackNodes.Add(currentNodeY * _mazeSize.x * _mazeSize.z + currentNodeX * _mazeSize.z + (currentNodeZ - 1));
        }

        if (fallbackNodes.Count > 0)
        {
            int fallbackIndex = Random.Range(0, fallbackNodes.Count);
            RoomNode fallbackNode = nodes[fallbackNodes[fallbackIndex]];

            // 대체 경로를 생성
            fallbackDirections.Sort();
            int fallbackDirection = fallbackDirections[fallbackIndex];

            switch (fallbackDirection)
            {
                case 1:
                    currentNode.F_OffWall(0);
                    currentNode.F_OnDoor(0);
                    fallbackNode.F_OffWall(1);
                    break;
                case 2:
                    currentNode.F_OffWall(1);
                    currentNode.F_OnDoor(1);
                    fallbackNode.F_OffWall(0);
                    break;
                case 5:
                    currentNode.F_OffWall(4);
                    currentNode.F_OnDoor(2);
                    fallbackNode.F_OffWall(5);
                    break;
                case 6:
                    currentNode.F_OffWall(5);
                    currentNode.F_OnDoor(3);
                    fallbackNode.F_OffWall(4);
                    break;
            }

            clearNodes.Add(fallbackNode);
        }
    }

    // 두 리스트를 동일한 순서로 무작위 섞기 위한 함수
    private void F_RandomDirection(List<int> directions, List<int> nodes)
    {
        for (int i = directions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // directions 리스트에서 요소 교환
            int tempDirection = directions[i];
            directions[i] = directions[randomIndex];
            directions[randomIndex] = tempDirection;

            // nodes 리스트에서 요소 교환
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
        _generateParent.SetActive(false); //내부 던전 끄기
    }

    protected override void InitManager()
    {
        return;
    }
}
