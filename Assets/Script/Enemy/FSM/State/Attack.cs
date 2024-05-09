using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : EnemyFSM
{
    public Attack(Enemy v_enemy)
    {
        enemy = v_enemy;
    }

    public override void Enter()
    {
        Debug.Log("Attack Enter");
    }

    public override void Excute()
    {
        Debug.Log("Attack Excute");
    }

    public override void Exit()
    {
        Debug.Log("Attack Exit");
    }
}
