using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyName
{
    SWAN,               // animal
    SPIDER_BLACK,       // monster
    SPIDER_SAND,        // monster
}

public class EnemyManager : Singleton<EnemyManager>
{
    private NavMeshController _navMeshController;

    [SerializeField] GameObject[] _enemyPrefabs;

    protected override void InitManager()
    {
        _navMeshController = GetComponent<NavMeshController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            string[] tmp = { "SPIDER_BLACK", "SPIDER_SAND", "SPDIER_TEST" };   // �����ϰ���� enemy name
            List<GameObject> enemys = F_GetEnemys(tmp);         // �� ����

            // �� ��ġ �ű��
            foreach(GameObject enemy in enemys)             
                enemy.transform.position = PlayerManager.Instance.playerTransform.position;

            // navmesh Bake
            F_NavMeshBake(NavMeshType.INSIDE);

            // ���� ������Ʈ ( NavMeshAgent�� ������ ������Ʈ ) �� ���� ������ ����
            // Navmesh�� ���� Bake �ؾ���!
        }
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
