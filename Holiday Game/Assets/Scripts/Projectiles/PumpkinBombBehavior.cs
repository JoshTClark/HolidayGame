using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinBombBehavior : ProjectileBase
{
    [SerializeField]
    float ExploRadius;

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
        
    }

}
