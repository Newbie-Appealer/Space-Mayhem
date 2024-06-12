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
    MANTICORE,          // animal
    MOSQUITO,           // animal
    SPIDER_BLACK,       // monster
    SPIDER_SAND,        // monster
    GHOST,              // monster
}

public class EnemyManager : Singleton<EnemyManager>
{
    private NavMeshController _navMeshController;

    [SerializeField] private GameObject[] _enemyPrefabs;
    private GameObject _enemyParent;
    protected override void InitManager()
    {
        _navMeshController = GetComponent<NavMeshController>();
        F_CreateEnemyGroup();
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

                // 4. 몬스터 Parent 설정
                tmpEnemy.transform.SetParent(_enemyParent.transform);
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

    public void F_RemoveEnemy()
    {
        Destroy(_enemyParent);
        F_CreateEnemyGroup();
    }

    private void F_CreateEnemyGroup()
    {
        _enemyParent = new GameObject();
        _enemyParent.name = "EnemyGroup";
        _enemyParent.transform.position = Vector3.zero;
    }
}
