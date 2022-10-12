using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballWeapon : Weapon
{
    public override void OnUpdate()
    {
        float delta = Time.deltaTime;

        // Basic targetting for now just targets the closest enemy
        Enemy e = GetClosestEnemy();
        if (e)
        {
            Vector2 direction = e.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (canFire && e)
        {
            ProjectileBase p = Instantiate<ProjectileBase>(projectile, transform.position, Quaternion.identity);
            p.Direction = transform.right;
            DamageInfo info = new DamageInfo();
            info.damage = damageMultiplier * owner.Damage;
            p.SetDamageInfo(info);
            ResetTimer();
        }
    }
}
