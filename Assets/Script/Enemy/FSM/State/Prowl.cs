using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prowl : EnemyFSM
{
    public Prowl(Enemy v_enemy)
    {
        enemy = v_enemy;
    }

    public override void Enter()
    {
        
    }

    public override void Excute()
    {
        enemy.F_EnemyProwl();   
    }

    public override void Exit()
    {
        
    }
}
