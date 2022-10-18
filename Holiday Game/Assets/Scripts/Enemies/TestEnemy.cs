using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    public override void OnStart()
    {
    }

    public override void OnUpdate()
    {
        Move();
    }
    protected override void CalcMoves()
    {
        // Basic enemy just wants to seek the player & to separate from other enemies
        movements.Add(SeekPlayer() * 2f);
        movements.Add(Separation() * .01f);
    }
}
