using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FireworkWeapon : Weapon
{
    public override void CalcStats()
    {
        // Calculating stats
        float count = GetBaseStat("Projectiles");
        float chance = GetBaseStat("Stun Chance");

        /*
        // More fireworks upgrade
        if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.FireworkCount))
        {
            count += 1f * (GameManager.instance.Player.GetItem(ResourceManager.UpgradeIndex.FireworkCount).CurrentLevel);
        }

        if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.StunningFireworks))
        {
            chance += GameManager.instance.Player.GetItem(ResourceManager.UpgradeIndex.StunningFireworks).CurrentLevel * 0.05f;
        }
        */
        trueStats["Projectiles"] = count;
        trueStats["Stun Chance"] = chance;
    }

    public override void OnUpdate()
    {
        float delta = Time.deltaTime;
        Enemy e = GetClosestEnemy();
        if (e && canFire)
        {
            for (int i = 0; i < GetStat("Projectiles"); i++)
            {
                FireworkProjectile p = (FireworkProjectile)ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.Firework);
                p.transform.position = this.transform.position;
                p.target = e;
                DamageInfo info = new DamageInfo();
                /*
                if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.StunningFireworks))
                {
                    if (GameManager.RollCheck(GetStat("Stun Chance")))
                    {
                        info.debuffs.Add(ResourceManager.BuffIndex.Stunned);
                    }
                }
                */
                info.damage = GetStat("Damage") * owner.Damage;
                info.attacker = owner;
                info.debuffs.Add(ResourceManager.BuffIndex.Burning);
                info.weapon = ResourceManager.WeaponIndex.Fireworks;
                p.SetDamageInfo(info);
                p.SizeMultiplier = GetStat("Size");
                float rotation = Random.Range(0, 360);
                p.gameObject.GetComponent<Rigidbody2D>().SetRotation(rotation);
                p.RotateDirection(rotation);
                if (i < GetStat("Projectiles") / 2f)
                {
                    e = GetClosestEnemy();
                }
                else
                {
                    e = GetRandomEnemy();
                }
            }
            ResetTimer();
        }
    }
}
