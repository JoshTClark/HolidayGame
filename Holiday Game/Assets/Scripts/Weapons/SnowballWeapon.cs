using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SnowballWeapon : Weapon
{
    public override void CalcStats()
    {
        // Calculating stats
        // Damage
        float damageMult = GetBaseStat("Damage");
        float sizeMult = GetBaseStat("Size");
        float pierceAdd = GetBaseStat("Pierce");
        float speedMult = GetBaseStat("Speed");
        /*
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballDamage1))
        {
            damageMult += 0.1f * owner.GetItem(ResourceManager.UpgradeIndex.SnowballDamage1).CurrentLevel;
        }
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballDamage2))
        {
            damageMult += 0.2f * owner.GetItem(ResourceManager.UpgradeIndex.SnowballDamage2).CurrentLevel;
        }
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballDamage3))
        {
            damageMult += 0.3f * owner.GetItem(ResourceManager.UpgradeIndex.SnowballDamage3).CurrentLevel;
        }

        // Size and pierce
        float sizeMult = GetBaseStat("Size");
        float pierceAdd = GetBaseStat("Pierce");
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSize1))
        {
            sizeMult += 0.1f * owner.GetItem(ResourceManager.UpgradeIndex.SnowballSize1).CurrentLevel;
        }
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSize2))
        {
            sizeMult += 0.2f * owner.GetItem(ResourceManager.UpgradeIndex.SnowballSize2).CurrentLevel;
            pierceAdd += 1 * owner.GetItem(ResourceManager.UpgradeIndex.SnowballSize2).CurrentLevel;
        }
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSize3))
        {
            sizeMult += 0.3f * owner.GetItem(ResourceManager.UpgradeIndex.SnowballSize3).CurrentLevel;
            pierceAdd += 2 * owner.GetItem(ResourceManager.UpgradeIndex.SnowballSize3).CurrentLevel;
        }

        // Speed
        float speedMult = GetBaseStat("Speed");
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.SnowballSpeed1))
        {
            speedMult += 0.15f * owner.GetItem(ResourceManager.UpgradeIndex.SnowballSpeed1).CurrentLevel;
        }
        */

        trueStats["Speed"] = speedMult;
        trueStats["Damage"] = damageMult;
        trueStats["Pierce"] = pierceAdd;
        trueStats["Size"] = sizeMult;
    }

    public override void OnUpdate()
    {
        float delta = Time.deltaTime;


        Vector3 target = MousePosition();
        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (canFire)
        {
            // Making the projectile
            ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.Snowball);

            // Making it spin if it has the snowballing upgrade
            /*
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.Snowballing))
            {
                float torque = 250f;
                p.gameObject.GetComponent<Rigidbody2D>().AddTorque(torque);
                p.gameObject.GetComponent<Rigidbody2D>().angularDrag = 0f;
            }
            */
            p.transform.position = this.transform.position;
            p.Direction = transform.right;
            DamageInfo info = new DamageInfo();
            info.damage = owner.Damage * GetStat("Damage");
            info.attacker = owner;
            info.knockbackDirection = p.Direction;
            info.weapon = ResourceManager.WeaponIndex.Snowball;
            p.SetDamageInfo(info);
            p.Pierce += GetStat("Pierce");
            p.SizeMultiplier = GetStat("Size");
            p.SpeedMultiplier = GetStat("Speed");
            ResetTimer();
        }
    }
}
