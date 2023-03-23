using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeralGroundHog : Weapon
{
    public override void CalcStats()
    {
    }

    public override void OnUpdate()
    {
        // Check to see if you can fire
        if (canFire)
        {
            // Launched towards closest enemy
            Enemy e = GetClosestEnemy();
            if (e)
            {
                Vector2 direction = e.transform.position - transform.position;
                // Don't know what this angle stuff is for (Copied from snowball)
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            // See if you can fire
            if(canFire && e)
            {
                // calculate Stats
                // Damage
                // Check for relevant upgrades

                // Damage upgrades

                // Size upgrades

                // speed upgrades

                // Make the projectile
                ProjectileBase p = (ProjectileBase)ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
                p.Direction = transform.right;
                // Create damage info
                DamageInfo info = new DamageInfo();
                info.damage = GetStat("Damage") * owner.Damage;
                info.attacker = owner;
                //info.debuffs.Add(ResourceManager.BuffIndex.Stunned);
                p.SetDamageInfo(info);
                //p.SizeMultiplier = sizeMult;
                //p.SpeedMultiplier = speedMult;
                ResetTimer();

            }
        }
    }
}
