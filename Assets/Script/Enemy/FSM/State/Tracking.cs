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
        Debug.Log("Tracking Enter");
    }

    public override void Excute()
    {
        Debug.Log("Tracking Excute");
    }

    public override void Exit()
    {
        Debug.Log("Tracking Exit");
    }
}
