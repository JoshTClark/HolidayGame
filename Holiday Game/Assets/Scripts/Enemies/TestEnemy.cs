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
        seekPlayer();
    }
}
