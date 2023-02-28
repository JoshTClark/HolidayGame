using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileProjectile : ProjectileBase
{
    public override void Move()
    {
        GetComponent<Rigidbody2D>().velocity = Direction * Speed;
    }

    public override void OnClean()
    {
        Rigidbody2D body = this.gameObject.GetComponent<Rigidbody2D>();
        this.transform.rotation = Quaternion.identity;
        body.angularVelocity = 0;
        body.velocity = Vector2.zero;
    }

    public override void OnCollision(Collider2D other)
    {
    }

    public override void OnDeath()
    {
    }

    public override void OnHit(StatsComponent receiver, DamageInfo info)
    {
    }

    public override void OnUpdate()
    {
    }
}