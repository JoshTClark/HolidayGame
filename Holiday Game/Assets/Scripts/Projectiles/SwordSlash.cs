using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : ProjectileBase
{
    private float degrees = 180f;
    private float moved = 0f;
    public override void Move()
    {
        if (degrees > moved)
        {
            float rotate = degrees / (Lifetime / 3) * Time.deltaTime;
            this.transform.Rotate(new Vector3(0, 0, rotate));
            moved += rotate;
        }
    }

    public override void OnClean()
    {
        this.gameObject.SetActive(true);
        moved = 0;
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
        /*
        speedMult = 50 * ((Lifetime - (TimeAlive*2)) / Lifetime);
        speedMult = Mathf.Clamp(speedMult, 0, 100);
        */
    }
}
