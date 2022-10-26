using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeralGroundHog : Weapon
{
    public override void OnUpdate()
    {
        // Check to see if you can fire
        if (canFire)
        {
            // Create the projectile
            //Projectile p = instantiate 



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
                float damageMult = baseDamageMultiplier;
                // Check for relevant upgrades

                // Damage upgrades

                // Size upgrades

                // speed upgrades

                // Make the projectile
                ProjectileBase p = Instantiate<ProjectileBase>(projectile, transform.position, Quaternion.identity);
                p.Direction = transform.right;
                // Create damage info
                DamageInfo info = new DamageInfo();
                info.damage = baseDamageMultiplier * owner.Damage;
                info.attacker = owner;
                p.SetDamageInfo(info);
                //p.SizeMultiplier = sizeMult;
                //p.SpeedMultiplier = speedMult;
                ResetTimer();

            }
        }
    }
}
