using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : EnemyFSM
{
    public Idle(Enemy v_enemy)
    {
        enemy = v_enemy;
    }

    public override void Enter()
    {
        enemy.animator.SetBool("Idle", true);
        enemy.animator.SetBool("Prowl", false);
        enemy.animator.SetBool("Tracking", false);
        enemy.animator.SetBool("Attack", false);
    }

    public override void Excute()
    {
        enemy.F_EnemyIdle();
    }

    public override void Exit()
    {
        enemy.animator.SetBool("Idle", false);
    }
}
