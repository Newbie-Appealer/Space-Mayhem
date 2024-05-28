using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class InsideMapManager : Singleton<InsideMapManager>
{
    [Header("Map Object")]
    [SerializeField] RoomNode[] _roomPrefab; //방 프리펩
    [SerializeField] GameObject _generateParent; //모든 방의 부모
    [SerializeField] GameObject _mapLight; //맵 전체 라이트
    public RoomNode _lastRoom; //마지막 방
    public RoomNode _startRoom; //시작 방
    public GameObject mapLight => _mapLight;

    [Header("Map Size")]
    [SerializeField] Vector3Int _mazeSize; //내부 던전 크기
    [SerializeField] int _roomScale; //방 크기

    List<RoomNode> nodes; //전체 방 리스트


    private void Start()
    {
        F_BuildRoad();
    }

    public void F_BuildRoad()
    {
        nodes = new List<RoomNode>();

        // 모든 방 만들어놓음
        for (int x = 0; x < _mazeSize.x; x++)
        {
            for (int y = 0; y < _mazeSize.y; y++)
            {
                for (int z = 0; z < _mazeSize.z; z++)
                {
                    RoomNode newNode;
                    Vector3 nodePos = new Vector3(x * (5 * _roomScale), y * (5 * _roomScale) + 500, z * (5 * _roomScale) + 100); //노드 위치
                    int nodeIndex = x * (_mazeSize.y * _mazeSize.z) + y * _mazeSize.z + z; //배열 인덱스
                    if (x == _mazeSize.x - 1 && y == _mazeSize.y - 1 && z == _mazeSize.z - 1)
                    {
                        //마지막 방 생성
                        newNode = Instantiate(_roomPrefab[_roomPrefab.Length - 1], nodePos, Quaternion.identity, _generateParent.transform);
                        //라이트 설치
                        //newNode.F_OnLight(nodeIndex);
                        _lastRoom = newNode;
                    }
                    else
                    {
                        //마지막 방을 제외한 나머지 방 생성
                        newNode = Instantiate(_roomPrefab[Random.Range(0, _roomPrefab.Length - 1)], nodePos, Quaternion.identity, _generateParent.transform);
                        //newNode.F_OnLight(nodeIndex);
                        if (x == 0 && y == 0 && z == 0)
                            _startRoom = newNode;
                    }
                    //방 크기 지정
                    newNode.transform.localScale = new Vector3(_roomScale, _roomScale, _roomScale);
                    nodes.Add(newNode);
                }
            }
        }
        //생성하고 꺼놓기
        _generateParent.SetActive(false);
    }

    public void F_GenerateInsideMap()
    {
        //함수가 실행되면 켜기
        _generateParent.SetActive(true);

        // 이미 방문한 노드를 추적하기 위한 리스트
        List<RoomNode> clearNodes = new List<RoomNode>();

        // 탐색할 노드들을 저장할 스택
        Stack<RoomNode> nodeStack = new Stack<RoomNode>();
        nodeStack.Push(nodes[0]);

        // 탐색 시작
        while (nodeStack.Count > 0)
        {
            RoomNode currentNode = nodeStack.Pop(); //nodeStack 마지막으로 들어간 요소가 현재 노드가 됨.
            clearNodes.Add(currentNode);

            // 현재 노드 주위의 가능한 노드와 방향을 찾기
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentNodeIndex = nodes.IndexOf(currentNode); //전체 노드 리스트에서 현재 노드와 일치하는 인덱스
            int currentNodeX = currentNodeIndex / (_mazeSize.y * _mazeSize.z);
            int currentNodeY = (currentNodeIndex % (_mazeSize.y * _mazeSize.z)) / _mazeSize.z;
            int currentNodeZ = currentNodeIndex % _mazeSize.z;

            // 현재 노드의 오른쪽 노드 확인
            if (currentNodeX < _mazeSize.x - 1)
            {
                int rightNodeIndex = (currentNodeX + 1) * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[rightNodeIndex])) //우측 노드 인덱스가 이미 방문한 노드가 아니라면
                {
                    possibleDirections.Add(1); //가능한 방향으로 추가
                    possibleNextNodes.Add(rightNodeIndex); //가능한 다음 노드에 인덱스 추가
                }
            }

            // 현재 노드의 왼쪽 노드 확인
            if (currentNodeX > 0)
            {
                int leftNodeIndex = (currentNodeX - 1) * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[leftNodeIndex]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(leftNodeIndex);
                }
            }

            // 현재 노드의 위쪽 노드 확인
            if (currentNodeY < _mazeSize.y - 1)
            {
                int upNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + (currentNodeY + 1) * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[upNodeIndex]))
                {
                    if (currentNodeIndex != 0) //첫 방은 위쪽 경로 배제
                    {
                        possibleDirections.Add(3);
                        possibleNextNodes.Add(upNodeIndex);
                    }
                }
            }

            // 현재 노드의 아래쪽 노드 확인
            if (currentNodeY > 0)
            {
                int downNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + (currentNodeY - 1) * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[downNodeIndex]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(downNodeIndex);
                }
            }

            // 현재 노드의 앞쪽 노드 확인
            if (currentNodeZ < _mazeSize.z - 1)
            {
                int frontNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ + 1;
                if (!clearNodes.Contains(nodes[frontNodeIndex]))
                {
                    possibleDirections.Add(5);
                    possibleNextNodes.Add(frontNodeIndex);
                }
            }

            // 현재 노드의 뒤쪽 노드 확인
            if (currentNodeZ > 0)
            {
                int backNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ - 1;
                if (!clearNodes.Contains(nodes[backNodeIndex]))
                {
                    possibleDirections.Add(6);
                    possibleNextNodes.Add(backNodeIndex);
                }
            }

            // 가능한 방향이 있다면
            if (possibleDirections.Count > 0)
            {
                // 가능한 방향과 노드를 무작위로 섞기
                F_RandomDirection(possibleDirections, possibleNextNodes);

                // 1에서 방향 갯수 사이 갈래길 생성
                for (int i = 0; i < Random.Range(1, possibleDirections.Count); i++)
                {
                    int direction = possibleDirections[i];
                    RoomNode nextNode = nodes[possibleNextNodes[i]];

                    switch (direction)
                    {
                        case 1: // 오른쪽
                            currentNode.F_OffWall(0);
                            currentNode.F_OnDoor(0); //현재 노드 문 생성
                            nextNode.F_OffWall(1); //현재 노드와 다음 노드 사이의 벽 제거
                            break;
                        case 2: // 왼쪽
                            currentNode.F_OffWall(1);
                            currentNode.F_OnDoor(1);
                            nextNode.F_OffWall(0);
                            break;
                        case 3: // 위쪽
                            currentNode.F_OnStair(); //위쪽과 아래쪽은 계단 생성
                            currentNode.F_OffWall(2);
                            nextNode.F_OffWall(3);
                            break;
                        case 4: // 아래쪽
                            currentNode.F_OffWall(3);
                            nextNode.F_OnStair();
                            nextNode.F_OffWall(2);
                            break;
                        case 5: // 앞쪽
                            currentNode.F_OffWall(4);
                            currentNode.F_OnDoor(2);
                            nextNode.F_OffWall(5);
                            break;
                        case 6: // 뒤쪽
                            currentNode.F_OffWall(5);
                            currentNode.F_OnDoor(3);
                            nextNode.F_OffWall(4);
                            break;
                    }

                    // 스택에 다음 노드 추가
                    nodeStack.Push(nextNode);
                }
            }
        }

        // 오브젝트 설치 테스트
        this.GetComponent<ObjectPlace>().F_PlaceItem(ref nodes);
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
