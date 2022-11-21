using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy2 : Enemy
{
    public override void OnUpdate()
    {
        Move();
    }
    protected override void CalcMoves()
    {
        movements.Add(ShooterMove() * 1.5f);
        movements.Add(Separation() * .1f);
    }

    public override void OnStart()
    {
        // Set player ranges
        minPlayerDist = 6f;
        maxPlayerDist = 8f;

        AddAttack(ResourceManager.GetWeapon(ResourceManager.WeaponIndex.Crows));
    }
}
