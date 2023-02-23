using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlashWeapon : Weapon
{
    public override void CalcStats()
    {
        if (owner.HasItem(ResourceManager.ItemIndex.SwordWeapon))
        {
            float damageMult = GetBaseStat("Damage");
            
            float pierceAdd = GetBaseStat("Pierce");
      

            Item i = owner.GetItem(ResourceManager.ItemIndex.SwordWeapon);
            if (i.Level >= 1)
            {
                damageMult *= 1.2f;
            }
            // Level 2
            if (i.Level >= 2)
            {
                damageMult *= 1.2f;
            }
            // Level 3
            if (i.Level >= 3)
            {
                damageMult *= 1.2f;
            }
        }
    }

    public override void OnUpdate()
    {
        Vector3 target = MousePosition();
        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (canFire)
        {
            ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.SwordSlash);
            p.transform.position = this.transform.position;
            p.transform.rotation = transform.rotation;
            //p.Direction = transform.right;
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
}
