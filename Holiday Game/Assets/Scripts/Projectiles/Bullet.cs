using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ProjectileBase
{
    private void Start()
    {
    }

    public override void Move()
    {
        Vector2 velocity = Direction * Speed;
        GetComponent<Rigidbody2D>().velocity = velocity;
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
    }
}
