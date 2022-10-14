using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinBomb : Weapon
{
    public override void OnUpdate()
    {
        float delta = Time.deltaTime;

        if (canFire)
        {
            BombProjectileBase p = Instantiate<BombProjectileBase>((BombProjectileBase)projectile, transform.position, Quaternion.identity);
            p.Direction = Vector2.zero;
            DamageInfo info = new DamageInfo();
            info.damage = baseDamageMultiplier * owner.Damage;
            p.SetDamageInfo(info);
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
            ResetTimer();
        }
    }

}
