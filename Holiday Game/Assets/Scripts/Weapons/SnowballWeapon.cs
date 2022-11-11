using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SnowballWeapon : Weapon
{
    public override void OnUpdate()
    {
        float delta = Time.deltaTime;


        Vector3 target = MousePosition();
        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (canFire)
        {
            // Calculating stats
            // Damage
            float damageMult = 1f;
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

            // Size and pierce
            float sizeMult = baseSizeMultiplier;
            float pierceAdd = 0;
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSize1))
            {
                sizeMult += 0.1f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballSize1).CurrentLevel;
            }
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSize2))
            {
                sizeMult += 0.2f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballSize2).CurrentLevel;
                pierceAdd += 1 * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballSize2).CurrentLevel;
            }
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSize3))
            {
                sizeMult += 0.3f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballSize3).CurrentLevel;
                pierceAdd += 2 * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballSize3).CurrentLevel;
            }

            // Speed
            float speedMult = 1f;
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSpeed1))
            {
                speedMult += 0.15f * owner.GetUpgrade(ResourceManager.UpgradeIndex.SnowballSpeed1).CurrentLevel;
            }

            // Making the projectile
            ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.Snowball);

            // Making it spin if it has the snowballing upgrade
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.Snowballing))
            {
                float torque = 250f;
                p.gameObject.GetComponent<Rigidbody2D>().AddTorque(torque);
                p.gameObject.GetComponent<Rigidbody2D>().angularDrag = 0f;
            }

            p.transform.position = this.transform.position;
            p.Direction = transform.right;
            DamageInfo info = new DamageInfo();
            info.damage = owner.Damage * baseDamageMultiplier;
            info.attacker = owner;
            info.knockbackDirection = p.Direction;
            p.SetDamageInfo(info);
            p.Pierce += pierceAdd;
            p.DamageMultiplier = damageMult;
            p.SizeMultiplier = sizeMult;
            p.SpeedMultiplier = speedMult;
            ResetTimer();
        }
    }
}
