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
        throw new System.NotImplementedException();
    }

    public override void OnDeath()
    {
        throw new System.NotImplementedException();
    }

    public override void OnHit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}
