using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FireworkWeapon : Weapon
{
    public override void OnUpdate()
    {
        float delta = Time.deltaTime;
        Enemy e = GetClosestEnemy();
        if (e && canFire)
        {
            // Calculating stats
            float countAdd = 0f;

            // More fireworks upgrade
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.FireworkCount))
            {
                countAdd += 1f * (GameManager.instance.Player.GetUpgrade(ResourceManager.UpgradeIndex.FireworkCount).CurrentLevel);
            }

            for (int i = 0; i < 3 + countAdd; i++)
            {
                FireworkProjectile p = (FireworkProjectile)ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.Firework);
                p.transform.position = this.transform.position;
                p.target = e;
                DamageInfo info = new DamageInfo();
                info.damage = baseDamageMultiplier * owner.Damage;
                info.attacker = owner;
                info.debuffs.Add(ResourceManager.BuffIndex.Burning);
                p.SetDamageInfo(info);
                p.SizeMultiplier = baseSizeMultiplier;
                float rotation = Random.Range(0, 360);
                p.gameObject.GetComponent<Rigidbody2D>().SetRotation(rotation);
                p.RotateDirection(rotation);
                if (i < (3 + countAdd) / 2)
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
