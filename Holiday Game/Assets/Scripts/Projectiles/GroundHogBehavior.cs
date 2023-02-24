using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHogBehavior : ProjectileBase
{
    SpriteRenderer sr;
    public void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public override void OnUpdate()
    {
        float delta = Time.deltaTime;
        sr.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(TimeAlive * TimeAlive * 1.2f, 1));

        if (SpeedMultiplier > 0)
        {
            SpeedMultiplier -= delta * 1.25f;
            if (SpeedMultiplier <= 0)
            {
                SpeedMultiplier = 0;
            }
        }
    }
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
    public override void OnHit(StatsComponent receiver, DamageInfo info)
    {

    }
    public override void OnDeath()
    {

    }
}
