using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ProjectileBase
{
    public bool isCluster;

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
        if (isCluster)
        {
            DamageInfo info = this.damageInfo;

            ProjectileBase p1 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
            p1.transform.position = this.transform.position;
            p1.Direction = transform.right;
            p1.RotateDirection(45);
            p1.SetDamageInfo(info.CreateCopy());
            p1.SpeedMultiplier = 1f;

            ProjectileBase p2 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
            p2.transform.position = this.transform.position;
            p2.Direction = transform.right;
            p2.RotateDirection(135);
            p2.SetDamageInfo(info.CreateCopy());
            p2.SpeedMultiplier = 1f;

            ProjectileBase p3 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
            p3.transform.position = this.transform.position;
            p3.Direction = transform.right;
            p3.RotateDirection(225);
            p3.SetDamageInfo(info.CreateCopy());
            p3.SpeedMultiplier = 1f;

            ProjectileBase p4 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
            p4.transform.position = this.transform.position;
            p4.Direction = transform.right;
            p4.RotateDirection(315);
            p4.SetDamageInfo(info.CreateCopy());
            p4.SpeedMultiplier = 1f;
        }
    }

    public override void OnHit(StatsComponent receiver)
    {
        // Nothing special
    }

    public override void OnUpdate()
    {
        if (isCluster)
        {
            SpeedMultiplier -= Time.deltaTime * 1.5f;
            if (SpeedMultiplier < 0)
            {
                SpeedMultiplier = 0;
            }
        }
    }

    public override void OnClean()
    {
        this.gameObject.SetActive(true);
        isCluster = false;
        projectileTeam = Team.Enemy;
    }
}
