using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PumpkinBombBehavior : BombProjectileBase
{
    SpriteRenderer sr;

    [SerializeField]
    GameObject explosion;

    public void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    public override void Move()
    {
        
    }

    public override void OnCollision()
    {
        // Nothing special
    }

    public override void OnDeath()
    {
        if (explosion)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        base.OnDeath();
    }

    public override void OnHit(StatsComponent receiver)
    {
        // Nothing special
    }

    public override void OnUpdate()
    {
        sr.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(TimeAlive * TimeAlive * 1.2f, 1));
    }
}
