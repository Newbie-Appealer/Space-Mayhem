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

    }

    public override void Excute()
    {
        enemy.F_EnemyAttack();
    }

    public override void Exit()
    {

    }
}
