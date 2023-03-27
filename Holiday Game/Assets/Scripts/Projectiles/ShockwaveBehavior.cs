using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveBehavior : ProjectileBase
{
    public override void Move()
    {
        this.SizeMultiplier = 0.2f + Mathf.Lerp(0, 5, TimeAlive / Lifetime);
        float a = Mathf.Lerp(1, 0, TimeAlive / Lifetime);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, a);
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
        float a = Mathf.Lerp(1, 0, TimeAlive / Lifetime);
        BuffInfo stun = new BuffInfo();
        stun.duration = 2f * a;
        stun.index = ResourceManager.BuffIndex.Stunned;
        stun.isDebuff = true;
        info.buffs.Add(stun);
    }

    public override void OnUpdate()
    {
    }
}
