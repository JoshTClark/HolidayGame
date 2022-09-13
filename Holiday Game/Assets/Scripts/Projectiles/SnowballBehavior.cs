using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballBehavior : ProjectileBase
{
    public override void Move()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * Speed;
    }

    public override void OnCollision()
    {
        // Nothing special
    }

    public override void OnDeath()
    {
        // Nothing special
    }

    public override void OnHit(StatsComponent receiver)
    {
        // Nothing special
    }

    public override void OnUpdate()
    {
        // Nothing special
    }
}
