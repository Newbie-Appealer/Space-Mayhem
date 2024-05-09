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
        Debug.Log("Prowl Enter");
    }

    public override void Excute()
    {
        Debug.Log("Prowl Excute");
    }

    public override void Exit()
    {
        Debug.Log("Prowl Exit");
    }
}
