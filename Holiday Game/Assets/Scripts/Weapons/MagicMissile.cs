using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : Weapon
{
    private bool doVolley = false;
    private float volleyTimer = 0.2f;
    private float fireTime = 0.65f;
    private int firedShots = 0;
    

    public override void CalcStats()
    {
        // Calculating stats
        if (owner.HasItem(ResourceManager.ItemIndex.MagicMissile))
        {
            float damageMult = GetBaseStat("Damage");
            float pierceAdd = GetBaseStat("Pierce");
            float sizeMult = GetBaseStat("Size");
            float atckSpdMult = GetBaseStat("Attack Speed");
            float projectileCount = GetBaseStat("Projectiles");
            Item i = owner.GetItem(ResourceManager.ItemIndex.MagicMissile);

            // Level 3 attack speed
            if (i.Level >= 3)
            {
                atckSpdMult += 0.1f;
            }

            // Level 4 damage
            if (i.Level >= 4)
            {
                damageMult += 0.15f;
            }

            if (i.HasTakenPath("Multi Missile"))
            {
                // Level 5 projectile
                if (i.Level >= 5)
                {
                    projectileCount += 1;
                }

                // Level 6 attack speed
                if (i.Level >= 6)
                {
                    atckSpdMult += 0.4f;
                }

                // Level 7 projectile
                if (i.Level >= 7)
                {
                    projectileCount += 1;
                }

                // Level 8 attack speed
                if (i.Level >= 8)
                {
                    atckSpdMult *= 1.25f;
                }
            }

            if (i.HasTakenPath("Burning"))
            {
                // Level 6 peirce
                if (i.Level >= 6)
                {
                    pierceAdd += 3;
                    sizeMult *= 1.25f;
                }
            }

            // Set the true stats of the weapon to the correct values
            trueStats["Damage"] = damageMult;
            trueStats["Pierce"] = pierceAdd;
            trueStats["Size"] = sizeMult;
            trueStats["Attack Speed"] = atckSpdMult;
            trueStats["Projectiles"] = projectileCount;
        }
    }

    public override void OnUpdate()
    {

        Vector3 target = MousePosition();
        if (!doVolley)
        {
            Vector2 direction = target - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (canFire)
        {
            doVolley = true;

        }

        if (doVolley)
        {

            if (firedShots < GetStat("Projectiles"))
            {
                volleyTimer += Time.deltaTime;

                // This helps control how much time is between each shot in the volley
                // The base fireTime which is the total amount of time firing all projectiles should take
                // Divided by the number of projectiles, attack speed of the owner, and any attack speed boosts from the weapon
                float timeBetweenShots = fireTime / GetStat("Projectiles") / owner.AttackSpeed / GetStat("Attack Speed");
                // Minimum fire time
                fireTime = Mathf.Clamp(fireTime, fireTime / GetStat("Projectiles"), float.MaxValue);

                if (volleyTimer >= timeBetweenShots)
                {
                    Item i = owner.GetItem(ResourceManager.ItemIndex.MagicMissile);

                    // Accuracy
                    float accuracyOff = 0;
                    if (i.HasTakenPath("Burning"))
                    {
                        // Level 6 accuracy decrease
                        if (i.Level >= 6)
                        {
                            accuracyOff = Random.Range(-15f, 15f);
                        }
                    }

                    ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.MagicMissile);
                    p.transform.position = this.transform.position;
                    p.transform.rotation = this.transform.rotation;
                    p.transform.Rotate(Quaternion.AngleAxis(accuracyOff, transform.forward).eulerAngles);
                    p.Direction = transform.right;
                    p.Pierce = GetStat("Pierce");
                    p.SizeMultiplier = GetStat("Size");
                    if (i.HasTakenPath("Multi Missile"))
                    {
                        // Level 7 seeking
                        if (i.Level >= 7)
                        {
                            ((MagicMissileProjectile)p).seeking = true;
                            p.LifetimeMultiplier += 0.5f;
                        }
                    }
                    DamageInfo info = new DamageInfo();
                    info.damage = owner.Damage * GetStat("Damage");
                    info.attacker = owner;
                    info.knockbackDirection = p.Direction;
                    info.knockback = 0.2f;
                    info.weapon = ResourceManager.WeaponIndex.MagicMissile;

                    // Burn Effect
                    DotInfo burn = new DotInfo();
                    burn.duration = 2f;
                    burn.index = ResourceManager.BuffIndex.Burning;
                    burn.damagePerTick = owner.Damage * 0.1f;
                    burn.tickRate = 0.5f;
                    // Level 2 burn
                    if (i.Level >= 2)
                    {
                        burn.damagePerTick = owner.Damage * 0.15f;
                    }

                    if (i.HasTakenPath("Burning"))
                    {
                        // Level 5 duration multiplier
                        if (i.Level >= 5)
                        {
                            burn.duration *= 2;
                        }

                        // Level 7 burn speed
                        if (i.Level >= 5)
                        {
                            burn.tickRate /= 1.25f;
                            burn.damagePerTick = owner.Damage * 0.4f;
                        }
                    }

                    burn.isDebuff = true;
                    info.buffs.Add(burn);
                    p.SetDamageInfo(info);
                    firedShots++;
                    volleyTimer = 0.0f;

                    if (i.HasTakenPath("Multi Missile"))
                    {
                        // Level 8 extra projectiles
                        if (i.Level >= 8)
                        {
                            FireExtra(info);
                        }
                    }

                    AudioManager.instance.PlaySound(soundEffect, .5f, 1f);
                }
            }
            else
            {
                ResetTimer();
                doVolley = false;
                volleyTimer = 0.2f;
                firedShots = 0;
            }
        }
    }

    private void FireExtra(DamageInfo info)
    {
        // Projectile 1
        ProjectileBase p1 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.MagicMissile);
        p1.transform.position = this.transform.position;
        p1.transform.rotation = transform.rotation;
        p1.Direction = transform.right;
        ((MagicMissileProjectile)p1).seeking = true;
        p1.LifetimeMultiplier += 0.5f;
        p1.Pierce = GetStat("Pierce");
        p1.SizeMultiplier = GetStat("Size") * 0.75f;
        p1.transform.rotation = Quaternion.AngleAxis(Random.Range(20, 340), transform.forward);
        p1.SetDamageInfo(info.CreateCopy());

        // Projectile 2
        ProjectileBase p2 = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.MagicMissile);
        p2.transform.position = this.transform.position;
        p2.transform.rotation = transform.rotation;
        p2.Direction = transform.right;
        ((MagicMissileProjectile)p2).seeking = true;
        p2.LifetimeMultiplier += 0.5f;
        p2.Pierce = GetStat("Pierce");
        p2.SizeMultiplier = GetStat("Size") * 0.75f;
        p2.transform.rotation = Quaternion.AngleAxis(Random.Range(20, 340), transform.forward);
        p2.SetDamageInfo(info.CreateCopy());
    }
}
