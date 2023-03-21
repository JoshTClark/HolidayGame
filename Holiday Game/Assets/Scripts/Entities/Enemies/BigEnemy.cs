using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemy : Enemy
{
    // Start is called before the first frame update
    public override void OnStart()
    {
        
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        Move();
    }
    protected override void CalcMoves()
    {
        // Seek & Separate
        movements.Add(SeekPlayer() * 3f);
        movements.Add(Separation() * .01f);
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        // For testing purposes
        base.OnTriggerStay2D(collision);

        //Debug.Break();

    }
}
