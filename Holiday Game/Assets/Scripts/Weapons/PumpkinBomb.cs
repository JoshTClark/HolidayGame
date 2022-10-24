using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PumpkinBomb : Weapon
{
    public override void OnUpdate()
    {
        float delta = Time.deltaTime;

        if (canFire)
        {
            BombProjectileBase p = (BombProjectileBase)pool.Get();
            p.transform.position = this.transform.position;
            p.Direction = Vector2.zero;
            DamageInfo info = new DamageInfo();
            info.damage = baseDamageMultiplier * owner.Damage;
            info.attacker = owner;
            p.SetDamageInfo(info);

            // Pumkin Launcher
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.PumpkinLauncher))
            {
                Enemy e = GetRandomEnemyInRange(10f);
                if (e)
                {
                    p.Direction = (e.transform.position - transform.position).normalized;
                }
                else
                {
                    p.RotateDirection(Random.Range(0, 360));
                }
                p.Speed = 10f;
                p.SpeedMultiplier = 1f;
                //float torque = Random.Range(-500f, 500f);
                //p.gameObject.GetComponent<Rigidbody2D>().AddTorque(torque);
                //p.gameObject.GetComponent<Rigidbody2D>().angularDrag = 1.75f;
                p.LifetimeMultiplier = 0.8f;
            }

            // Explosion size boost
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.PumkinRadius1))
            {
                p.explosionSizeMultiplier += 0.1f * (GameManager.instance.Player.GetUpgrade(ResourceManager.UpgradeIndex.PumkinRadius1).CurrentLevel);
            }
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.PumkinRadius2))
            {
                p.explosionSizeMultiplier += 0.2f * (GameManager.instance.Player.GetUpgrade(ResourceManager.UpgradeIndex.PumkinRadius2).CurrentLevel);
            }
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.PumkinRadius3))
            {
                p.explosionSizeMultiplier += 0.3f * (GameManager.instance.Player.GetUpgrade(ResourceManager.UpgradeIndex.PumkinRadius3).CurrentLevel);
            }

            // Damage
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.PumpkinDamage1))
            {
                p.DamageMultiplier += 0.5f * (GameManager.instance.Player.GetUpgrade(ResourceManager.UpgradeIndex.PumpkinDamage1).CurrentLevel);
            }
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.PumpkinDamage2))
            {
                p.DamageMultiplier += 1f * (GameManager.instance.Player.GetUpgrade(ResourceManager.UpgradeIndex.PumpkinDamage2).CurrentLevel);
            }
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.PumpkinDamage3))
            {
                p.DamageMultiplier += 1.5f * (GameManager.instance.Player.GetUpgrade(ResourceManager.UpgradeIndex.PumpkinDamage3).CurrentLevel);
            }

            ResetTimer();
        }
    }

}
