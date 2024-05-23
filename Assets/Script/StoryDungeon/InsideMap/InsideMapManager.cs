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
        nodes = new List<MazeNode>(); //��ü ��� ����Ʈ

        // ��� ����
        for (int x = 0; x < _mazeSize.x; x++)
        {
            for (int y = 0; y < _mazeSize.y; y++)
            {
                for (int z = 0; z < _mazeSize.z; z++)
                {
                    MazeNode newNode;
                    Vector3 nodePos = new Vector3(x * (5 * _roomScale), y * (5 * _roomScale) + 500, z * (5 * _roomScale) + 100); //��� ��ġ
                    int nodeIndex = x + (y * _mazeSize.x) + (z * _mazeSize.x * _mazeSize.y);

                    if (x == _mazeSize.x - 1 && y == _mazeSize.y - 1 && z == _mazeSize.z - 1)
                    {
                        newNode = Instantiate(_roomPrefab[_roomPrefab.Length - 1], nodePos, Quaternion.identity, _generateParent.transform); //������ ��
                        newNode.F_InstallLight(nodeIndex, nodePos, newNode);
                        _lastRoom = newNode;
                    }
                    else
                    {
                        newNode = Instantiate(_roomPrefab[Random.Range(0, _roomPrefab.Length - 1)], nodePos, Quaternion.identity, _generateParent.transform); //������ ��
                        newNode.F_InstallLight(nodeIndex, nodePos, newNode);
                        if (x == 0 && y == 0 && z == 0)
                            _startRoom = newNode;
                    }
                    newNode.transform.localScale = new Vector3(_roomScale, _roomScale, _roomScale); //�� ũ��
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
            int currentNodeX = currentNodeIndex / (_mazeSize.y * _mazeSize.z); //���� ����� x, y, z��ǥ
            int currentNodeY = (currentNodeIndex % (_mazeSize.y * _mazeSize.z)) / _mazeSize.z;
            int currentNodeZ = currentNodeIndex % _mazeSize.z;

            if (currentNodeX < _mazeSize.x - 1)
            {
                // ���� ����� ������ ��� Ȯ��
                int rightNodeIndex = (currentNodeX + 1) * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ;
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
                // ���� ����� ���� ��� Ȯ��
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
                // ���� ����� �Ʒ��� ��� Ȯ��
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
                // ���� ����� ���� ��� Ȯ��
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
                // ���� ����� ���� ��� Ȯ��
                int backNodeIndex = currentNodeX * _mazeSize.y * _mazeSize.z + currentNodeY * _mazeSize.z + currentNodeZ - 1;
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
                        chosenNode.F_OffWall(1);
                        currentPath[currentPath.Count - 1].F_OffWall(0);
                        break;
                    case 2: //����
                        chosenNode.F_OffWall(0);
                        currentPath[currentPath.Count - 1].F_OffWall(1);
                        break;
                    case 3: //����
                        chosenNode.F_OffWall(3);
                        currentPath[currentPath.Count - 1].F_InstallStair();
                        currentPath[currentPath.Count - 1].F_OffWall(2); 
                        break;
                    case 4: //�Ʒ���
                        chosenNode.F_OffWall(2);
                        chosenNode.F_InstallStair();
                        currentPath[currentPath.Count - 1].F_OffWall(3);
                        break;
                    case 5: // ����
                        chosenNode.F_OffWall(5);
                        currentPath[currentPath.Count - 1].F_OffWall(4);
                        break;
                    case 6: // ����
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
