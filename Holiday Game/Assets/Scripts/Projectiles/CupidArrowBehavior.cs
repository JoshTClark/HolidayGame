using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupidArrowBehavior : ProjectileBase
{
    List<Enemy> enemies;
    private void Start()
    {
        enemies = EnemyManager.instance.CurrentEnemies;
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
        Enemy e = GetClosestEnemyForArrow();

        Vector2 direction = e.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.position = this.transform.position;
        Direction = transform.right;
    }

    public override void OnUpdate()
    {
    }

    public override void OnClean()
    {


    }
    protected Enemy GetClosestEnemyForArrow()
    {
        List<Enemy> enemies = EnemyManager.instance.CurrentEnemies;
        if (enemies.Count > 0)
        {
            Enemy closest = null;
            float distance = GameManager.instance.Player.attackActivationRange;
            foreach (Enemy e in enemies)
            {
                float newDistance = e.PlayerDistance();
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
}
