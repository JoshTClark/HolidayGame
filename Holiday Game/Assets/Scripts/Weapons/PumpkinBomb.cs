using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PumpkinBomb : Weapon
{
    public override void CalcStats()
    {
        if (owner.HasItem(ResourceManager.ItemIndex.PumpkinBomb))
        {
            float damage = GetBaseStat("Damage");
            float attackSpeed = GetBaseStat("Attack Speed");
            float size = GetBaseStat("Explosion Size");
            float fuse = GetBaseStat("Fuse");
            Item i = owner.GetItem(ResourceManager.ItemIndex.PumpkinBomb);

            // Explosion size boost
            if (i.Level >= 2)
            {
                size += 0.2f;
            }

            // Explosion size boost
            if (i.Level >= 3)
            {
                damage += 0.2f;
            }

            // Explosion delay
            if (i.Level >= 4)
            {
                fuse *= 0.8f;
                attackSpeed += 0.1f;
            }

            trueStats["Damage"] = damage;
            trueStats["Attack Speed"] = attackSpeed;
            trueStats["Explosion Size"] = size;
            trueStats["Fuse"] = fuse;
        }
    }

    public override void OnUpdate()
    {
        float delta = Time.deltaTime;

        if (canFire)
        {
            BombProjectileBase p = (BombProjectileBase)ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.PumpkinBomb);
            p.transform.position = this.transform.position;
            p.Direction = Vector2.zero;
            DamageInfo info = new DamageInfo();
            info.damage = GetStat("Damage") * owner.Damage;
            info.attacker = owner;
            info.weapon = ResourceManager.WeaponIndex.PumpkinBomb;
            p.SetDamageInfo(info);
            p.explosionSizeMultiplier = GetStat("Explosion Size");
            p.LifetimeMultiplier = GetStat("Fuse");

            if (owner.HasItem(ResourceManager.ItemIndex.PumpkinBomb))
            {
                Item i = owner.GetItem(ResourceManager.ItemIndex.PumpkinBomb);
                if (i.HasTakenPath("Cluster Pumpkins"))
                {
                    if (i.Level >= 7)
                    {
                        ((PumpkinBombBehavior)p).isRecursive = true;
                    }
                    else if (i.Level >= 5)
                    {
                        ((PumpkinBombBehavior)p).isCluster = true;
                    }

                    if (i.Level >= 6)
                    {
                        ((PumpkinBombBehavior)p).clusterSpeed *= 1.3f;
                    }

                    if (i.Level >= 8)
                    {
                        ((PumpkinBombBehavior)p).clusterNum = 6;
                    }
                }

                if (i.HasTakenPath("Shockwave"))
                {
                    ((PumpkinBombBehavior)p).shockWave = true;
                }
            }

            /*
            // Pumkin Launcher
            if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.PumpkinLauncher))
            {
                Enemy e = GetRandomEnemyInRange(10f);
                if (e)
                {
                    p.Direction = (e.transform.position - transform.position).normalized;
                }
                else
                {
                    p.RotateDirection(Random.Range(0, 360));
                }
                p.Speed = 10f;
                p.SpeedMultiplier = 1f;
                //float torque = Random.Range(-500f, 500f);
                //p.gameObject.GetComponent<Rigidbody2D>().AddTorque(torque);
                //p.gameObject.GetComponent<Rigidbody2D>().angularDrag = 1.75f;
            }
            */
            ResetTimer();
        }
    }
}
