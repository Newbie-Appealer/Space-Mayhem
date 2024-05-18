using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public enum EnemyName
{
    SWAN,
    SPIDER_BLACK,
    SPIDER_SAND,
}

public class EnemyManager : MonoBehaviour
{
    [SerializeField] NavMeshSurface _outsideMapMeshSurface;
    [SerializeField] NavMeshSurface _insideMapMeshSurface;
    [SerializeField] GameObject[] _enemyPrefabs;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {


            //GameObject obj = Instantiate(_enemyPrefabs[(int)EnemyName.SWAN]);
            //obj.transform.position = PlayerManager.Instance.playerTransform.position;

            //// NavMesh Navigation Navmesh 동적 Bake
            //_outsideMapMeshSurface.collectObjects = CollectObjects.Children;
            //_outsideMapMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
            //_outsideMapMeshSurface.BuildNavMesh();

            //===============================================================//
            
            GameObject obj1 = Instantiate(_enemyPrefabs[(int)EnemyName.SPIDER_BLACK]);
            obj1.transform.position = PlayerManager.Instance.playerTransform.position;

            GameObject obj2 = Instantiate(_enemyPrefabs[(int)EnemyName.SPIDER_SAND]);
            obj2.transform.position = PlayerManager.Instance.playerTransform.position;

            // NavMesh Navigation Navmesh
            _insideMapMeshSurface.collectObjects = CollectObjects.Children;
            _insideMapMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
            _insideMapMeshSurface.BuildNavMesh();



            // 몬스터 오브젝트 ( NavMeshAgent가 부착된 오브젝트 ) 가 먼저 생성된 이후
            // Navmesh를 동적 Bake 해야함!
        }
    }
}
