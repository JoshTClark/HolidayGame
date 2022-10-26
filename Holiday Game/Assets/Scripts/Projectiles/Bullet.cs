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

    public override void OnCollision(Collider2D other)
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

    public override void OnClean()
    {
        this.gameObject.SetActive(true);
    }
}
