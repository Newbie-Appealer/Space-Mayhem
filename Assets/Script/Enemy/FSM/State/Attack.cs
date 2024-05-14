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
        enemy.animator.SetBool("Attack",true);
    }

    public override void Excute()
    {
        enemy.F_EnemyAttack();
    }

    public override void Exit()
    {
        enemy.animator.SetBool("Attack", false);
    }
}
