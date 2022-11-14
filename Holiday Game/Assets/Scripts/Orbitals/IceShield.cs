using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShield : OrbitalBase
{
    private float shotDelay = 0.07f;
    private float timer = 0.0f;
    private float totalDamage = 0f;

    public override void OnUpdate()
    {
        if (totalDamage > 0)
        {
            timer += Time.deltaTime;
            if(timer > shotDelay)
            {
                int i = Random.Range(1, 4);
                for (; i > 0 && totalDamage > 0; i--)
                {
                    StatsComponent owner = parent.owner;
                    Enemy e = GetClosestEnemy();
                    Vector2 direction = e.transform.position - transform.position;
                    ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.IceShard);
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + Random.Range(-20f, 20f);
                    p.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    p.transform.position = this.transform.position;
                    p.Direction = p.transform.right;
                    DamageInfo passingInfo = new DamageInfo();
                    passingInfo.damage = owner.Damage / 3;
                    passingInfo.attacker = owner;
                    passingInfo.knockback = 0;
                    passingInfo.knockbackDirection = p.Direction;
                    p.SetDamageInfo(passingInfo);
                    totalDamage -= owner.Damage / 3;
                }
                timer = Random.Range(0f, 0.2f);
            }
        }
        else
        {
            totalDamage = 0f;
            timer = 0.0f;
        }
    }

    public void Shoot(DamageInfo info)
    {
        totalDamage += info.damage * 1.5f;
    }
}
