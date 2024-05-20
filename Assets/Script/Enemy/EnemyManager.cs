using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyName
{
    SWAN,               // animal
    TURTLE,             // animal
    SPIDER_BLACK,       // monster
    SPIDER_SAND,        // monster
}

public class EnemyManager : Singleton<EnemyManager>
{
    private NavMeshController _navMeshController;

    [SerializeField] GameObject[] _enemyPrefabs;
    [SerializeField] Transform _enemyParentTransform;
    protected override void InitManager()
    {
        _navMeshController = GetComponent<NavMeshController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            string[] tmpInside = { "SPIDER_BLACK", "SPIDER_SAND", "INSIDE_TEST" };   // 생성하고싶은 enemy name
            string[] tmpOutside = { "SWAN", "TURTLE", "OUTSIDE_TEST" };


            //// 내부 몹
            List<GameObject> inSideEnemys = F_GetEnemys(tmpInside);         // 몹 생성
            foreach (GameObject enemy in inSideEnemys)
            {
                enemy.transform.position = PlayerManager.Instance.playerTransform.position;
                enemy.transform.SetParent(_enemyParentTransform);
            }
            F_NavMeshBake(NavMeshType.INSIDE);

            // 외부 몹
            //List<GameObject> outSideEnemys = F_GetEnemys(tmpOutside);       // 몹 생성
            //foreach (GameObject enemy in outSideEnemys)
            //{
            //    enemy.transform.position = PlayerManager.Instance.playerTransform.position;
            //    enemy.transform.SetParent(_enemyParentTransform);
            //}
            //F_NavMeshBake(NavMeshType.OUTSIDE);

        }
        // 몬스터 오브젝트 ( NavMeshAgent가 부착된 오브젝트 ) 가 먼저 생성된 이후
        // Navmesh를 동적 Bake 해야함!
    }


    public List<GameObject> F_GetEnemys(string[] v_enemyNames)
    {
        List<GameObject> retEnemys = new List<GameObject>();

        for (int i = 0; i < v_enemyNames.Length; i++)
        {
            try
            {
                // 1. string -> enum 변환
                EnemyName name = (EnemyName)Enum.Parse(typeof(EnemyName), v_enemyNames[i]); 

                // 2. 몬스터 생성
                GameObject tmpEnemy = Instantiate(_enemyPrefabs[(int)name]);

                // 3. 배열에 추가
                retEnemys.Add(tmpEnemy);
            }
            catch
            {
                // 1. 몬스터 생성 실패
                Debug.LogError("EnemyName Error : Failed Get Enemy [" + v_enemyNames[i] +"]");
            }
        }

        return retEnemys;
    }

    public void F_NavMeshBake(NavMeshType v_type)
    {
        _navMeshController.F_NavMeshBake(v_type);
    }
}
