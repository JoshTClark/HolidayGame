using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy2 : Enemy
{
    private void Start()
    {
        AddAttack(GameManager.instance.GetWeaponFromIndex(GameManager.WeaponIndex.Test));
    }

    void Update()
    {
        // Do nothing for now
    }
}
