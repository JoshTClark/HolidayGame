using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyCornWeapon : Weapon
{
    private bool doVolley = false;
    private float volleyTimer = 0.2f;
    private float shotDelay = 0.6f;
    private int shotsPerVolley = 3;
    private int firedShots = 0;

    public override void OnUpdate()
    {
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray))
        {
            this.attackSpeedMultiplier = 1.5f + 0.5f * (owner.GetUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray).CurrentLevel - 1);
        }

        Vector3 target = MousePosition();
        if (!doVolley)
        {
            Vector2 direction = target - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (canFire)
        {
            doVolley = true;
        }

        if (doVolley)
        {
            int extra = 0;
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.MoreCorn))
            {
                extra = owner.GetUpgrade(ResourceManager.UpgradeIndex.MoreCorn).CurrentLevel;
            }
            if (firedShots < shotsPerVolley + extra)
            {
                volleyTimer += Time.deltaTime;
                if (volleyTimer >= (((shotDelay / (shotsPerVolley + extra)) / owner.AttackSpeed) / attackSpeedMultiplier))
                {
                    float accuracyOff = 0;
                    if (owner.HasUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray))
                    {
                        accuracyOff = Random.Range(-10 * (owner.GetUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray).CurrentLevel), 10 * (owner.GetUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray).CurrentLevel));
                    }
                    ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.CandyCorn);
                    p.transform.position = this.transform.position;
                    p.transform.rotation = transform.rotation;
                    p.Direction = transform.right;
                    p.RotateDirection(accuracyOff);
                    DamageInfo info = new DamageInfo();
                    info.damage = owner.Damage * baseDamageMultiplier;
                    info.attacker = owner;
                    info.knockbackDirection = p.Direction;
                    info.knockback = 0.2f;
                    p.SetDamageInfo(info);
                    float torque = 500f;
                    p.gameObject.GetComponent<Rigidbody2D>().AddTorque(torque);
                    firedShots++;
                    volleyTimer = 0.0f;
                }
            }
            else
            {
                ResetTimer();
                doVolley = false;
                volleyTimer = 0.2f;
                firedShots = 0;
            }
        }
    }
}
