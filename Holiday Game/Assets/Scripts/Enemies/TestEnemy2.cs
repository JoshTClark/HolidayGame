using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy2 : Enemy
{
    public override void OnUpdate()
    {
    }

    private void Start()
    {
        base.Start();

        AddAttack(GameManager.instance.GetWeaponFromIndex(GameManager.WeaponIndex.Test));
    }
}
