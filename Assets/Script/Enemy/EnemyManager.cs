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
            string[] tmpInside = { "SPIDER_BLACK", "SPIDER_SAND", "INSIDE_TEST" };   // �����ϰ���� enemy name
            string[] tmpOutside = { "SWAN", "TURTLE", "OUTSIDE_TEST" };


            //// ���� ��
            List<GameObject> inSideEnemys = F_GetEnemys(tmpInside);         // �� ����
            foreach (GameObject enemy in inSideEnemys)
            {
                enemy.transform.position = PlayerManager.Instance.playerTransform.position;
                enemy.transform.SetParent(_enemyParentTransform);
            }
            F_NavMeshBake(NavMeshType.INSIDE);

            // �ܺ� ��
            //List<GameObject> outSideEnemys = F_GetEnemys(tmpOutside);       // �� ����
            //foreach (GameObject enemy in outSideEnemys)
            //{
            //    enemy.transform.position = PlayerManager.Instance.playerTransform.position;
            //    enemy.transform.SetParent(_enemyParentTransform);
            //}
            //F_NavMeshBake(NavMeshType.OUTSIDE);

        }
        // ���� ������Ʈ ( NavMeshAgent�� ������ ������Ʈ ) �� ���� ������ ����
        // Navmesh�� ���� Bake �ؾ���!
    }


    public List<GameObject> F_GetEnemys(string[] v_enemyNames)
    {
        List<GameObject> retEnemys = new List<GameObject>();

        for (int i = 0; i < v_enemyNames.Length; i++)
        {
            try
            {
                // 1. string -> enum ��ȯ
                EnemyName name = (EnemyName)Enum.Parse(typeof(EnemyName), v_enemyNames[i]); 

                // 2. ���� ����
                GameObject tmpEnemy = Instantiate(_enemyPrefabs[(int)name]);

                // 3. �迭�� �߰�
                retEnemys.Add(tmpEnemy);
            }
            catch
            {
                // 1. ���� ���� ����
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
