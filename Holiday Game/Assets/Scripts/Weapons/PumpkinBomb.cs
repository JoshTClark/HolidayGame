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
            ProjectileBase p = Instantiate<ProjectileBase>(projectile, transform.position, Quaternion.identity);
            p.Direction = Vector2.zero;
            DamageInfo info = new DamageInfo();
            info.damage = baseDamageMultiplier * owner.Damage;
            p.SetDamageInfo(info);
            ResetTimer();
        }
    }

}
