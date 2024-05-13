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
        // 임시 U키 누르면 플레이어 위치에 몬스터 생성
        if(Input.GetKeyDown(KeyCode.U))
        {
            //            Instantiate(_enemyPrefabs[(int)EnemyName.TEST], PlayerManager.Instance.PlayerController.transform);
            _outsideMapMeshSurface.collectObjects = CollectObjects.Children;
            _outsideMapMeshSurface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
            _outsideMapMeshSurface.BuildNavMesh();
        }
    }
}
