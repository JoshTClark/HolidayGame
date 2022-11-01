using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupidArrow : Weapon
{
    public override void OnUpdate()
    {
        Enemy e = GetClosestEnemy();
        if (e)
        {
            Vector2 direction = e.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (canFire && e)
        {
            ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.CupidArrow);
            p.transform.position = this.transform.position;
            p.Direction = transform.right;
            DamageInfo info = new DamageInfo();
            info.damage = owner.Damage;
            info.attacker = owner;
            info.knockbackDirection = p.Direction;
            p.SetDamageInfo(info);
            ResetTimer();
        }
    }
}
