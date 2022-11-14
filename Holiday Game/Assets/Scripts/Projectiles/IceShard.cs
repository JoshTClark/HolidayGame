using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShard : ProjectileBase
{
    public override void Move()
    {
        GetComponent<Rigidbody2D>().velocity = Direction * Speed;
    }

    public override void OnClean()
    {
    }

    public override void OnCollision(Collider2D other)
    {
    }

    public override void OnDeath()
    {
    }

    public override void OnHit(StatsComponent receiver)
    {
    }

    public override void OnUpdate()
    {
    }
}
