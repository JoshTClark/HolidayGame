using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupidArrow : Weapon
{
    public override void OnUpdate()
    {
        Enemy e = GetClosestEnemy();
        if (e)
        {
            Vector2 direction = e.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (canFire && e)
        {
            // Play weapon sound
            SoundManager.instance.ArrowHit();
            float pierceAdd = 0;
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.ArrowPierce1))
            {
                pierceAdd += 1 * owner.GetUpgrade(ResourceManager.UpgradeIndex.ArrowPierce1).CurrentLevel;
            }
            if (owner.HasUpgrade(ResourceManager.UpgradeIndex.ArrowPierce2))
            {   
                pierceAdd += 2 * owner.GetUpgrade(ResourceManager.UpgradeIndex.ArrowPierce2).CurrentLevel;
            }
            

            ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.CupidArrow);
            p.transform.position = this.transform.position;
            p.transform.rotation = transform.rotation;
            p.Direction = transform.right;
            DamageInfo info = new DamageInfo();
            info.damage = owner.Damage * baseDamageMultiplier;
            info.attacker = owner;
            info.knockbackDirection = p.Direction;
            info.weapon = ResourceManager.WeaponIndex.CupidArrow;
            p.SetDamageInfo(info);
            ResetTimer();
        }
    }
}
