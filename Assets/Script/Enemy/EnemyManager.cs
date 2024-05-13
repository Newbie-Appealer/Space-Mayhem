using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public enum EnemyName
{
    SWAN,
}

public class EnemyManager : MonoBehaviour
{
    [SerializeField] NavMeshSurface _outsideMapMeshSurface;

    [SerializeField] GameObject[] _enemyPrefabs;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            //���� ���� ( �ӽ� )
            GameObject obj = Instantiate(_enemyPrefabs[(int)EnemyName.SWAN]);
            obj.transform.position = PlayerManager.Instance.playerTransform.position;

            // NavMesh Navigation Navmesh ���� Bake
            _outsideMapMeshSurface.collectObjects = CollectObjects.Children;
            _outsideMapMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
            _outsideMapMeshSurface.BuildNavMesh();


            // ���� ������Ʈ ( NavMeshAgent�� ������ ������Ʈ ) �� ���� ������ ����
            // Navmesh�� ���� Bake �ؾ���!
        }
    }
}
