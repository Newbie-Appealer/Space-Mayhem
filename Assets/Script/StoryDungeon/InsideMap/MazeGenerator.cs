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
        List<MazeNode> nodes = new List<MazeNode>(); //��ü ��� ����Ʈ

        // ��� ����
        for (int x = 0; x < v_size.x; x++)
        {
            for (int y = 0; y < v_size.y; y++)
            {
                for(int z = 0; z < v_size.z; z++)
                {
                    MazeNode newNode;
                    Vector3 nodePos = new Vector3(x * (5 * _roomScale), y * (5 * _roomScale) + 500, z * (5 * _roomScale) + 100); //��� ��ġ
                    if (x == v_size.x - 1 && y == v_size.y - 1 && z == v_size.z - 1)
                    {
                        newNode = Instantiate(_lastRoom, nodePos, Quaternion.identity, transform);
                    }
                    else
                    {
                        newNode = Instantiate(_roomPrefab[Random.Range(0, _roomPrefab.Length)], nodePos, Quaternion.identity, transform);
                    }
                    newNode.transform.localScale = new Vector3(_roomScale, _roomScale, _roomScale); //�� ũ��
                    nodes.Add(newNode);
                }
            }
        }

        List<MazeNode> currentPath = new List<MazeNode>();
        List<MazeNode> clearNodes = new List<MazeNode>();
        //�� ��Ͽ� ���� ���� ��尡 ��� ������ ���

        currentPath.Add(nodes[0]);//������� Ž������ ��� ����
        //nodes[0] = ù ��
        //nodes[nodes.Count - 1] = ���� �ָ��ִ� ��

        while (clearNodes.Count < nodes.Count) //�湮�� ��庸�� �湮���� ���� ��� ���� �� ���� ����
        {
            // ���� ��� ���� ��� Ȯ��
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            //currentPath�� ������ ��� = currentPath�� ���� �������� �߰��� ���
            //nodes�� ����Ʈ���� ��ġ�ϴ� �ε��� ��ȣ�� �־��� ������ -1
            int currentNodeX = currentNodeIndex / (v_size.y * v_size.z); //���� ����� x, y, z��ǥ
            int currentNodeY = (currentNodeIndex % (v_size.y * v_size.z)) / v_size.z;
            int currentNodeZ = currentNodeIndex % v_size.z;

            if (currentNodeX < v_size.x - 1)
            {
                // ���� ����� ������ ��� Ȯ��
                int rightNodeIndex = (currentNodeX + 1) * v_size.y * v_size.z + currentNodeY * v_size.z + currentNodeZ;
                if (!clearNodes.Contains(nodes[rightNodeIndex]) &&
                    !currentPath.Contains(nodes[rightNodeIndex])) //�̹� �湮�� ��峪 ���� ��ο� ���Ե��� ������
                {
                    possibleDirections.Add(1); //������ ���⿡ �߰�
                    possibleNextNodes.Add(rightNodeIndex); //���� ��忡 ������ ��� �߰�
                }
            }
            if (currentNodeX > 0)
            {
                // ���� ����� ���� ��� Ȯ��
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
                // ���� ����� ���� ��� Ȯ��
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
                // ���� ����� �Ʒ��� ��� Ȯ��
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
                // ���� ����� ���� ��� Ȯ��
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
                // ���� ����� ���� ��� Ȯ��
                int backNodeIndex = currentNodeX * v_size.y * v_size.z + currentNodeY * v_size.z + currentNodeZ - 1;
                if (!clearNodes.Contains(nodes[backNodeIndex]) &&
                    !currentPath.Contains(nodes[backNodeIndex]))
                {
                    possibleDirections.Add(6);
                    possibleNextNodes.Add(backNodeIndex);
                }
            }

            // ���� ��� ����
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count); //������ ���� �� ���� ����
                MazeNode chosenNode = nodes[possibleNextNodes[chosenDirection]]; //��� ����Ʈ���� ������ ���� ����

                switch (possibleDirections[chosenDirection])
                {
                    //currentPath[currentPath.Count - 1] = ���� ���
                    //chosenNode = ���� ���� ���
                    case 1: //������
                        chosenNode.F_RemoveWall(1);
                        currentPath[currentPath.Count - 1].F_RemoveWall(0);
                        break;
                    case 2: //����
                        chosenNode.F_RemoveWall(0);
                        currentPath[currentPath.Count - 1].F_RemoveWall(1);
                        break;
                    case 3: //����
                        chosenNode.F_RemoveWall(3); 
                        currentPath[currentPath.Count - 1].F_InstallStair(currentPath[currentPath.Count - 1], chosenNode);
                        currentPath[currentPath.Count - 1].F_RemoveWall(2); 
                        break;
                    case 4: //�Ʒ���
                        chosenNode.F_RemoveWall(2);
                        chosenNode.F_InstallStair(currentPath[currentPath.Count - 1], chosenNode);
                        currentPath[currentPath.Count - 1].F_RemoveWall(3);
                        break;
                    case 5: // ����
                        chosenNode.F_RemoveWall(5);
                        currentPath[currentPath.Count - 1].F_RemoveWall(4);
                        break;
                    case 6: // ����
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
