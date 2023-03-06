using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon2 : Weapon
{
    public override void CalcStats()
    {
    }

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
            info.damage = GetStat("Damage") * owner.Damage;
            info.attacker = owner;

            ProjectileBase p1 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
            p1.transform.position = this.transform.position;
            p1.Direction = transform.right;
            p1.SetDamageInfo(info.CreateCopy());
            p1.SpeedMultiplier = 4f;
            p1.SizeMultiplier = 2f;
            p1.LifetimeMultiplier = 0.5f;
            ((Bullet)p1).isCluster = true;

            ResetTimer();
        }
    }
}
