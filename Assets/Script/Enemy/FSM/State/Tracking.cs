using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracking : EnemyFSM
{
    public Tracking(Enemy v_enemy)
    {
        enemy = v_enemy;
    }

    public override void Enter()
    {
        enemy.animator.SetBool("Tracking", true);
    }

    public override void Excute()
    {
        enemy.F_EnemyTracking();
    }

    public override void Exit()
    {
        enemy.animator.SetBool("Tracking", false);
    }
}
