using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    [SerializeField]
    protected AudioSource spawnSound;
    public override void OnStart()
    {
    }

    public override void OnUpdate()
    {
        Move();
    }

    protected override void CalcMoves()
    {
        movements.Add(SeekPlayer());
    }
}
