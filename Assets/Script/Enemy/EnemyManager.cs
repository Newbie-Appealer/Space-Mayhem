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
            //몬스터 생성 ( 임시 )
            GameObject obj = Instantiate(_enemyPrefabs[(int)EnemyName.SWAN]);
            obj.transform.position = PlayerManager.Instance.playerTransform.position;

            // NavMesh Navigation Navmesh 동적 Bake
            _outsideMapMeshSurface.collectObjects = CollectObjects.Children;
            _outsideMapMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
            _outsideMapMeshSurface.BuildNavMesh();


            // 몬스터 오브젝트 ( NavMeshAgent가 부착된 오브젝트 ) 가 먼저 생성된 이후
            // Navmesh를 동적 Bake 해야함!
        }
    }
}
