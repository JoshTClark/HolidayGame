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
        SeekPlayer();
        Move();
    }
}
