using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyCornWeapon : Weapon
{
    private bool doVolley = false;
    private float volleyTimer = 0.2f;
    private float shotDelay = 0.2f;
    private int shotsPerVolley = 3;
    private int firedShots = 0;

    public override void OnUpdate()
    {
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray))
        {
            this.attackSpeedMultiplier = 1.5f + 0.5f * (owner.GetUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray).CurrentLevel - 1);
        }

        Enemy e = GetClosestEnemy();
        if (e)
        {
            Vector2 direction = e.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (canFire && e)
        {
            doVolley = true;
        }

        if (doVolley)
        {
            if (firedShots < shotsPerVolley)
            {
                volleyTimer += Time.deltaTime;
                if (volleyTimer >= (shotDelay / owner.AttackSpeed) / attackSpeedMultiplier)
                {
                    float accuracyOff = 0;
                    if (owner.HasUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray))
                    {
                        accuracyOff = Random.Range(-20 * (owner.GetUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray).CurrentLevel), 20 * (owner.GetUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray).CurrentLevel));
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
