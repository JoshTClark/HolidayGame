using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkWeapon : Weapon
{
    public override void OnUpdate()
    {
        float delta = Time.deltaTime;
        Enemy e = GetRandomEnemy();
        if (e && canFire)
        {
            // Calculating stats
            float damageMult = baseDamageMultiplier;
            float countAdd = 0f;
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.FireworkCount))
            {
                countAdd += 1f * (GameManager.instance.Player.GetUpgrade(ResourceManager.UpgradeIndex.FireworkCount).CurrentLevel);
            }

            for (int i = 0; i < 3 + countAdd; i++)
            {
                FireworkProjectile p = Instantiate<FireworkProjectile>((FireworkProjectile)projectile, transform.position, Quaternion.identity);
                p.target = e;
                DamageInfo info = new DamageInfo();
                info.damage = damageMult * owner.Damage;
                p.SetDamageInfo(info);
                p.SizeMultiplier = baseSizeMultiplier;
                p.SpeedMultiplier = baseSizeMultiplier;
                float rotation = Random.Range(0, 360);
                p.gameObject.GetComponent<Rigidbody2D>().SetRotation(rotation);
                p.RotateDirection(rotation);
                e = GetRandomEnemy();
            }
            ResetTimer();
        }
    }
}
