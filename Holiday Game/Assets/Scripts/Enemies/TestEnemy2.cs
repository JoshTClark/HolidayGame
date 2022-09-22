using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy2 : Enemy
{
    public override void OnUpdate()
    {
        shooterMove();
    }

    private void Start()
    {
        base.Start();

        // Set player ranges
        minPlayerDist = 6f;
        maxPlayerDist = 8f;

        AddAttack(GameManager.instance.GetWeaponFromIndex(GameManager.WeaponIndex.Test));
    }
}
