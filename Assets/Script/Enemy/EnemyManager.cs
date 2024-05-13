using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public enum EnemyName
{
    TEST
}

public class EnemyManager : MonoBehaviour
{
    [SerializeField] NavMeshSurface _outsideMapMeshSurface;

    [SerializeField] GameObject[] _enemyPrefabs;


    void Update()
    {
        // �ӽ� UŰ ������ �÷��̾� ��ġ�� ���� ����
        if(Input.GetKeyDown(KeyCode.U))
        {
            //            Instantiate(_enemyPrefabs[(int)EnemyName.TEST], PlayerManager.Instance.PlayerController.transform);
            _outsideMapMeshSurface.collectObjects = CollectObjects.Children;
            _outsideMapMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
            _outsideMapMeshSurface.BuildNavMesh();
        }
    }
}
