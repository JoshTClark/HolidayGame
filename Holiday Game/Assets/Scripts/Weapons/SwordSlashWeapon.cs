using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlashWeapon : Weapon
{
    public override void CalcStats()
    {
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
