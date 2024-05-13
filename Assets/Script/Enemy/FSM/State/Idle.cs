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

    }

    public override void Excute()
    {
        enemy.F_EnemyIdle();
    }

    public override void Exit()
    {
        
    }
}
