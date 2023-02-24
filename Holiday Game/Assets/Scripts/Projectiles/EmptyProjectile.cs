using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyProjectile : ProjectileBase
{
    public override void Move()
    {
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

    public override void OnClean()
    {
        this.gameObject.SetActive(true);
    }
}
