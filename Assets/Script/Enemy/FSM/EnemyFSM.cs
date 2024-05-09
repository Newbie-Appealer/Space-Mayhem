using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyFSM
{
    public Enemy enemy;

    public abstract void Enter();       // 상태 입장
    public abstract void Excute();      // 상태 진행중
    public abstract void Exit();        // 상태 퇴장
}
