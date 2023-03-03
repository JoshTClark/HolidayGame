using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon : Weapon
{
    private bool isAttacking;
    private float volleyDelay = 0.25f;
    private int volleys = 16;
    private float volleyTimer = 0.0f;
    private float firedVolleys = 0;
    private float volleySpeed = 3f;
    private float volleySeperation = 15f;

    public override void CalcStats()
    {
    }

    public override void OnUpdate()
    {
        float delta = Time.deltaTime;

        Player player = GameManager.instance.Player;

        // Attack timer it will do the attack every "delay" seconds
        if (canFire)
        {
            isAttacking = true;
        }


        if (isAttacking)
        {
            volleyTimer += delta;
            if (volleyTimer >= volleyDelay)
            {
                DamageInfo info = new DamageInfo();
                info.damage = owner.Damage * GetStat("Damage");
                info.attacker = owner;

                ProjectileBase p1 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
                p1.transform.position = this.transform.position;
                p1.Direction = transform.right;
                p1.RotateDirection(0 + firedVolleys * volleySeperation);
                p1.SetDamageInfo(info.CreateCopy());
                p1.SpeedMultiplier = volleySpeed;

                ProjectileBase p2 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
                p2.transform.position = this.transform.position;
                p2.Direction = transform.right;
                p2.RotateDirection(60 + firedVolleys * volleySeperation);
                p2.SetDamageInfo(info.CreateCopy());
                p2.SpeedMultiplier = volleySpeed;

                ProjectileBase p3 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
                p3.transform.position = this.transform.position;
                p3.Direction = transform.right;
                p3.RotateDirection(120 + firedVolleys * volleySeperation);
                p3.SetDamageInfo(info.CreateCopy());
                p3.SpeedMultiplier = volleySpeed;

                ProjectileBase p4 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
                p4.transform.position = this.transform.position;
                p4.Direction = transform.right;
                p4.RotateDirection(180 + firedVolleys * volleySeperation);
                p4.SetDamageInfo(info.CreateCopy());
                p4.SpeedMultiplier = volleySpeed;

                ProjectileBase p5 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
                p5.transform.position = this.transform.position;
                p5.Direction = transform.right;
                p5.RotateDirection(240 + firedVolleys * volleySeperation);
                p5.SetDamageInfo(info.CreateCopy());
                p5.SpeedMultiplier = volleySpeed;

                ProjectileBase p6 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyProjectile);
                p6.transform.position = this.transform.position;
                p6.Direction = transform.right;
                p6.RotateDirection(300 + firedVolleys * volleySeperation);
                p6.SetDamageInfo(info.CreateCopy());
                p6.SpeedMultiplier = volleySpeed;

                volleyTimer = 0.0f;
                firedVolleys++;
                if (firedVolleys >= volleys)
                {
                    ResetTimer();
                    firedVolleys = 0.0f;
                    isAttacking = false;
                }
            }
        }
    }

    protected override void WeaponSound()
    {
        return;
    }
}
