using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupidArrow : Weapon
{
    public override void CalcStats()
    {
        float pierce = GetBaseStat("Pierce");
        /*
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.ArrowPierce1))
        {
            pierce += 1 * owner.GetItem(ResourceManager.UpgradeIndex.ArrowPierce1).CurrentLevel;
        }
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.ArrowPierce2))
        {
            pierce += 2 * owner.GetItem(ResourceManager.UpgradeIndex.ArrowPierce2).CurrentLevel;
        }
        */
        trueStats["Pierce"] = pierce;
    }

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
            

            ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.CupidArrow);
            p.transform.position = this.transform.position;
            p.transform.rotation = transform.rotation;
            p.Direction = transform.right;
            p.Pierce = GetStat("Pierce");
            DamageInfo info = new DamageInfo();
            info.damage = owner.Damage * GetStat("Damage");
            info.attacker = owner;
            info.knockbackDirection = p.Direction;
            info.weapon = ResourceManager.WeaponIndex.CupidArrow;
            p.SetDamageInfo(info);
            ResetTimer();
        }
    }
    protected override void WeaponSound()
    {
        return;
    }
}
