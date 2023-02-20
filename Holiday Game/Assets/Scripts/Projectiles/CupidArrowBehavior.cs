using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupidArrowBehavior : ProjectileBase
{
    Enemy e;
    private void Start()
    {
    }

    public override void Move()
    {
        GetComponent<Rigidbody2D>().velocity = Direction * Speed;
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
        e = GetClosestEnemyForArrow();

        if (e)
        {
            Vector2 direction = e.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Direction = transform.right;

            /*
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.CupidArrowSwiftFlight))
            {
                SpeedMultiplier += .5f;
            }
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.ArrowBounceDamage))
            {
                DamageMultiplier += .2f;
            }
            */
        }
    }

    public override void OnUpdate()
    {
    }

    public override void OnClean()
    {


    }
    protected Enemy GetClosestEnemyForArrow()
    {
        List<Enemy> enemies = EnemyManager.instance.AllEnemies;
        if (enemies.Count > 0)
        {
            Enemy closest = null;
            float distance = 1000;
            foreach (Enemy e in enemies)
            {
                float newDistance = Vector2.Distance(transform.position, e.transform.position);
                if (newDistance < distance && !hitTargets.Contains(e))
                {
                    closest = e;
                    distance = newDistance;
                }
            }
            return closest;
        }
        else
        {
            return null;
        }
    }

    void OnDrawGizmos()
    {
        if (e)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(e.gameObject.transform.position, 1);
        }
    }
}
