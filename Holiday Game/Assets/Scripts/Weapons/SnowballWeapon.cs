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
            // Calculating stats
            // Damage
            float damageMult = baseDamageMultiplier;
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballDamage1))
            {
                damageMult += 0.1f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballDamage1).CurrentLevel;
            }
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballDamage2))
            {
                damageMult += 0.2f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballDamage2).CurrentLevel;
            }
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballDamage3))
            {
                damageMult += 0.3f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballDamage3).CurrentLevel;
            }

            // Size
            float sizeMult = baseSizeMultiplier;
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSize1))
            {
                sizeMult += 0.1f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballSize1).CurrentLevel;
            }
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSize2))
            {
                sizeMult += 0.2f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballSize2).CurrentLevel;
            }
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSize3))
            {
                sizeMult += 0.3f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballSize3).CurrentLevel;
            }

            // Speed
            float speedMult = 1f;
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSpeed1))
            {
                speedMult += 0.15f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballSpeed1).CurrentLevel;
            }

            // Making the projectile
            ProjectileBase p = Instantiate<ProjectileBase>(projectile, transform.position, Quaternion.identity);
            p.Direction = transform.right;
            DamageInfo info = new DamageInfo();
            info.damage = damageMult * owner.Damage;
            p.SetDamageInfo(info);
            p.SizeMultiplier = sizeMult;
            p.SpeedMultiplier = speedMult;
            ResetTimer();
        }
    }
}
