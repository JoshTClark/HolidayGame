using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordStab : ProjectileBase
{
    public override void Move()
    {
        GetComponent<Rigidbody2D>().velocity = Direction * Mathf.Clamp(Speed - (Speed * (TimeAlive / Lifetime)), 0, Speed);
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

    public override void OnHit(StatsComponent receiver, DamageInfo info)
    {

    }

    public override void OnUpdate()
    {
        float val = -Mathf.Pow((((TimeAlive + Lifetime) / Lifetime) - 1f), 5) + 1;
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(val, val, 1, val);
    }
}
