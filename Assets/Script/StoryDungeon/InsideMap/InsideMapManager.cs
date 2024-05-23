using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class InsideMapManager : Singleton<InsideMapManager>
{
    [Header("Map Object")]
    [SerializeField] MazeNode[] _roomPrefab;
    public MazeNode _lastRoom;
    public MazeNode _startRoom;
    [SerializeField] GameObject _generateParent;
    [SerializeField] GameObject _mapLight;
    public GameObject mapLight => _mapLight;

    [Header("Map Size")]
    [SerializeField] Vector3Int _mazeSize;
    [SerializeField] int _roomScale;

    List<MazeNode> nodes;

    bool isFirst = false;

    private void Start()
    {
        nodes = new List<MazeNode>(); //전체 노드 리스트

        // 노드 생성
        for (int x = 0; x < _mazeSize.x; x++)
        {
            for (int y = 0; y < _mazeSize.y; y++)
            {
                for (int z = 0; z < _mazeSize.z; z++)
                {
                    MazeNode newNode;
                    Vector3 nodePos = new Vector3(x * (5 * _roomScale), y * (5 * _roomScale) + 500, z * (5 * _roomScale) + 100); //노드 위치
                    int nodeIndex = x + (y * _mazeSize.x) + (z * _mazeSize.x * _mazeSize.y);

                    if (x == _mazeSize.x - 1 && y == _mazeSize.y - 1 && z == _mazeSize.z - 1)
                    {
                        newNode = Instantiate(_roomPrefab[_roomPrefab.Length - 1], nodePos, Quaternion.identity, _generateParent.transform); //마지막 방
                        newNode.F_InstallLight(nodeIndex, nodePos, newNode);
                        _lastRoom = newNode;
                    }
                    else
                    {
                        newNode = Instantiate(_roomPrefab[Random.Range(0, _roomPrefab.Length - 1)], nodePos, Quaternion.identity, _generateParent.transform); //나머지 방
                        newNode.F_InstallLight(nodeIndex, nodePos, newNode);
                        if (x == 0 && y == 0 && z == 0)
                            _startRoom = newNode;
                    }
                    newNode.transform.localScale = new Vector3(_roomScale, _roomScale, _roomScale); //방 크기
                    nodes.Add(newNode);
                }
            }
        }
        _generateParent.SetActive(false);
    }

    public void F_GenerateMaze()
    {
        _generateParent.SetActive(true);
        List<MazeNode> currentPath = new List<MazeNode>();
        List<MazeNode> clearNodes = new List<MazeNode>();
        //두 목록에 들어가지 않은 노드가 사용 가능한 노드

        currentPath.Add(nodes[0]);//현재까지 탐색중인 경로 저장
        //nodes[0] = 첫 방
        //nodes[nodes.Count - 1] = 가장 멀리있는 방

        while (clearNodes.Count < nodes.Count) //방문한 노드보다 방문하지 않은 노드 수가 더 많은 동안
        {
            // 현재 노드 옆의 노드 확인
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            //currentPath의 마지막 요소 = currentPath에 가장 마지막에 추가된 요소
            //nodes의 리스트에서 일치하는 인덱스 번호를 넣어줌 없으면 -1
            int currentNodeX = currentNodeIndex / (_mazeSize.y * _mazeSize.z); //현재 노드의 x, y, z좌표
            int currentNodeY = (currentNodeIndex % (_mazeSize.y * _mazeSize.z)) / _mazeSize.z;
            int currentNodeZ = currentNodeIndex % _mazeSize.z;

            if (currentNodeX < _mazeSize.x - 1)
            {
                // 현재 노드의 오른쪽 노드 확인
                int rightNodeIndex = (currentNodeX + 1) * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[rightNodeIndex]) &&
                    !currentPath.Contains(nodes[rightNodeIndex])) //이미 방문한 노드나 현재 경로에 포함되지 않으면
                {
                    possibleDirections.Add(1); //가능한 방향에 추가
                    possibleNextNodes.Add(rightNodeIndex); //다음 노드에 오른쪽 노드 추가
                }
            }
            if (currentNodeX > 0)
            {
                // 현재 노드의 왼쪽 노드 확인
                int leftNodeIndex = (currentNodeX - 1) * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[leftNodeIndex]) &&
                    !currentPath.Contains(nodes[leftNodeIndex]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(leftNodeIndex);
                }
            }
            if (currentNodeY < _mazeSize.y - 1)
            {
                // 현재 노드의 위쪽 노드 확인
                int upNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + (currentNodeY + 1) * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[upNodeIndex]) &&
                    !currentPath.Contains(nodes[upNodeIndex]))
                {
                    if (isFirst)
                    {
                        possibleDirections.Add(3);
                        possibleNextNodes.Add(upNodeIndex);
                    }
                    isFirst = true;
                }
            }
            if (currentNodeY > 0)
            {
                // 현재 노드의 아래쪽 노드 확인
                int downNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + (currentNodeY - 1) * _mazeSize.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[downNodeIndex]) &&
                    !currentPath.Contains(nodes[downNodeIndex]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(downNodeIndex);
                }
            }
            if (currentNodeZ < _mazeSize.z - 1)
            {
                // 현재 노드의 앞쪽 노드 확인
                int frontNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ + 1;
                if (!clearNodes.Contains(nodes[frontNodeIndex]) &&
                    !currentPath.Contains(nodes[frontNodeIndex]))
                {
                    possibleDirections.Add(5);
                    possibleNextNodes.Add(frontNodeIndex);
                }
            }

            if (currentNodeZ > 0)
            {
                // 현재 노드의 뒤쪽 노드 확인
                int backNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ - 1;
                if (!clearNodes.Contains(nodes[backNodeIndex]) &&
                    !currentPath.Contains(nodes[backNodeIndex]))
                {
                    possibleDirections.Add(6);
                    possibleNextNodes.Add(backNodeIndex);
                }
            }

            // 다음 노드 선택
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count); //가능한 방향 중 랜덤 선택
                MazeNode chosenNode = nodes[possibleNextNodes[chosenDirection]]; //노드 리스트에서 가능한 방향 선택

                switch (possibleDirections[chosenDirection])
                {
                    //currentPath[currentPath.Count - 1] = 현재 노드
                    //chosenNode = 다음 방향 노드
                    case 1: //오른쪽
                        chosenNode.F_OffWall(1);
                        currentPath[currentPath.Count - 1].F_OffWall(0);
                        break;
                    case 2: //왼쪽
                        chosenNode.F_OffWall(0);
                        currentPath[currentPath.Count - 1].F_OffWall(1);
                        break;
                    case 3: //위쪽
                        chosenNode.F_OffWall(3);
                        currentPath[currentPath.Count - 1].F_InstallStair();
                        currentPath[currentPath.Count - 1].F_OffWall(2); 
                        break;
                    case 4: //아래쪽
                        chosenNode.F_OffWall(2);
                        chosenNode.F_InstallStair();
                        currentPath[currentPath.Count - 1].F_OffWall(3);
                        break;
                    case 5: // 앞쪽
                        chosenNode.F_OffWall(5);
                        currentPath[currentPath.Count - 1].F_OffWall(4);
                        break;
                    case 6: // 뒤쪽
                        chosenNode.F_OffWall(4);
                        currentPath[currentPath.Count - 1].F_OffWall(5);
                        break;
                }
                currentPath.Add(chosenNode);
            }
            else
            {
                clearNodes.Add(currentPath[currentPath.Count - 1]);

                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }
    }

    public void F_DestroyInsideMap()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].F_OnWall();
        }
        _generateParent.SetActive(false);
    }

    protected override void InitManager()
    {
        return;
    }
}
