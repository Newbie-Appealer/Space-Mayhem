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
        Debug.Log("Idel Enter");
    }

    public override void Excute()
    {
        Debug.Log("Idel Excute");
    }

    public override void Exit()
    {
        Debug.Log("Idel Exit");
    }
}
