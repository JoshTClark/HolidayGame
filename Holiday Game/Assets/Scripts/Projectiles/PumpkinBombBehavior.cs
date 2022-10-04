using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PumpkinBombBehavior : ProjectileBase
{
    [SerializeField]
    float ExploRadius;

    SpriteRenderer sr;

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
        List<Enemy> enemies = EnemyManager.instance.CurrentEnemies;
        foreach (Enemy e in enemies)
        {
            if (Vector2.Distance((Vector2)e.transform.position, (Vector2)transform.position) <= ExploRadius)
            {
                e.DealDamage(Damage);
            }
        }
    }

    public override void OnHit(StatsComponent receiver)
    {
        // Nothing special
    }

    public override void OnUpdate()
    {
        sr.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Mathf.Pow(TimeAlive, 2.25f), 1));
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ExploRadius);
    }

}
