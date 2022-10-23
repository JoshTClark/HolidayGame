using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletTestWeapon : Weapon
{
    public override void OnUpdate()
    {
        float delta = Time.deltaTime;

        Player player = GameManager.instance.Player;
        Vector2 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Attack timer it will do the attack every "delay" seconds
        if (canFire)
        {
            DamageInfo info = new DamageInfo();
            info.damage = baseDamageMultiplier * owner.Damage;
            info.attacker = owner;

            ProjectileBase p1 = pool.Get();
            p1.transform.position = this.transform.position;
            p1.Direction = transform.right;
            p1.RotateDirection(15);
            p1.SetDamageInfo(info.CreateCopy());

            ProjectileBase p2 = pool.Get();
            p2.transform.position = this.transform.position;
            p2.Direction = transform.right;
            p2.RotateDirection(-15);
            p2.SetDamageInfo(info.CreateCopy());

            ProjectileBase p3 = pool.Get();
            p3.transform.position = this.transform.position;
            p3.Direction = transform.right;
            p3.RotateDirection(0);
            p3.SetDamageInfo(info.CreateCopy());

            ResetTimer();
        }
    }
}
