using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy2 : Enemy
{
    public override void OnUpdate()
    {
        ShooterMove();
        Move();
    }

    public override void OnStart()
    {
        // Set player ranges
        minPlayerDist = 6f;
        maxPlayerDist = 8f;

        AddAttack(ResourceManager.GetWeapon(ResourceManager.WeaponIndex.Test));
    }
}
