using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] MazeNode[] _roomPrefab;
    [SerializeField] MazeNode _lastRoom;
    [SerializeField] Vector3Int _mazeSize;
    [SerializeField] int _roomScale;
    bool isGenerate = false;
    bool isFirst = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Quote) && !isGenerate)
        {
            F_GenerateMaze(_mazeSize);
            isGenerate = true;
        }
    }

    public void F_GenerateMaze(Vector3Int v_size)
    {
        List<MazeNode> nodes = new List<MazeNode>(); //전체 노드 리스트

        // 노드 생성
        for (int x = 0; x < v_size.x; x++)
        {
            for (int y = 0; y < v_size.y; y++)
            {
                for(int z = 0; z < v_size.z; z++)
                {
                    MazeNode newNode;
                    Vector3 nodePos = new Vector3(x * (5 * _roomScale), y * (5 * _roomScale) + 500, z * (5 * _roomScale) + 100); //노드 위치
                    if (x == v_size.x - 1 && y == v_size.y - 1 && z == v_size.z - 1)
                    {
                        newNode = Instantiate(_lastRoom, nodePos, Quaternion.identity, transform);
                    }
                    else
                    {
                        newNode = Instantiate(_roomPrefab[Random.Range(0, _roomPrefab.Length)], nodePos, Quaternion.identity, transform);
                    }
                    newNode.transform.localScale = new Vector3(_roomScale, _roomScale, _roomScale); //방 크기
                    nodes.Add(newNode);
                }
            }
        }

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
            int currentNodeX = currentNodeIndex / (v_size.y * v_size.z); //현재 노드의 x, y, z좌표
            int currentNodeY = (currentNodeIndex % (v_size.y * v_size.z)) / v_size.z;
            int currentNodeZ = currentNodeIndex % v_size.z;

            if (currentNodeX < v_size.x - 1)
            {
                // 현재 노드의 오른쪽 노드 확인
                int rightNodeIndex = (currentNodeX + 1) * v_size.y * v_size.z + currentNodeY * v_size.z + currentNodeZ;
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
                int leftNodeIndex = (currentNodeX - 1) * v_size.y * v_size.z + currentNodeY * v_size.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[leftNodeIndex]) &&
                    !currentPath.Contains(nodes[leftNodeIndex]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(leftNodeIndex);
                }
            }
            if (currentNodeY < v_size.y - 1)
            {
                // 현재 노드의 위쪽 노드 확인
                int upNodeIndex = currentNodeX * v_size.y * v_size.z + (currentNodeY + 1) * v_size.z + currentNodeZ;
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
                int downNodeIndex = currentNodeX * v_size.y * v_size.z + (currentNodeY - 1) * v_size.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[downNodeIndex]) &&
                    !currentPath.Contains(nodes[downNodeIndex]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(downNodeIndex);
                }
            }
            if (currentNodeZ < v_size.z - 1)
            {
                // 현재 노드의 앞쪽 노드 확인
                int frontNodeIndex = currentNodeX * v_size.y * v_size.z + currentNodeY * v_size.z + currentNodeZ + 1;
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
                int backNodeIndex = currentNodeX * v_size.y * v_size.z + currentNodeY * v_size.z + currentNodeZ - 1;
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
                        chosenNode.F_RemoveWall(1);
                        currentPath[currentPath.Count - 1].F_RemoveWall(0);
                        break;
                    case 2: //왼쪽
                        chosenNode.F_RemoveWall(0);
                        currentPath[currentPath.Count - 1].F_RemoveWall(1);
                        break;
                    case 3: //위쪽
                        chosenNode.F_RemoveWall(3); 
                        currentPath[currentPath.Count - 1].F_InstallStair(currentPath[currentPath.Count - 1], chosenNode);
                        currentPath[currentPath.Count - 1].F_RemoveWall(2); 
                        break;
                    case 4: //아래쪽
                        chosenNode.F_RemoveWall(2);
                        chosenNode.F_InstallStair(currentPath[currentPath.Count - 1], chosenNode);
                        currentPath[currentPath.Count - 1].F_RemoveWall(3);
                        break;
                    case 5: // 앞쪽
                        chosenNode.F_RemoveWall(5);
                        currentPath[currentPath.Count - 1].F_RemoveWall(4);
                        break;
                    case 6: // 뒤쪽
                        chosenNode.F_RemoveWall(4);
                        currentPath[currentPath.Count - 1].F_RemoveWall(5);
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
}
