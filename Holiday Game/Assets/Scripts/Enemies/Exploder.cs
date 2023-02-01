using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : Enemy
{
    private bool inRange = false;
    private float range = 2f; // Test later

    public override void OnStart()
    {
        
    }
    public override void OnUpdate()
    {
        if(PlayerDistance() <= range)
        {
            inRange= true;
        }
    }
    protected override void CalcMoves()
    {
        if (inRange)
        {
            // Stop movement and explode
            movements.Add(Vector2.zero);
            
        }
        else
        {
            movements.Add(SeekPlayer() * 1.5f);

        }
    }
}
