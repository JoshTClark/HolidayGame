using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlashWeapon : Weapon
{
    private bool multiStrike5 = false;
    private bool multiStrike6 = false;
    private bool multiStrike8 = false;
    private int swingNum = 0;
    private float swingDelay = 0.35f;
    private float swingTimer = 0.0f;

    public override void CalcStats()
    {
        // Calculating stats
        if (owner.HasItem(ResourceManager.ItemIndex.SwordWeapon))
        {
            float damageMult = GetBaseStat("Damage");
            float pierceAdd = GetBaseStat("Pierce");
            float sizeMult = GetBaseStat("Size");
            float atckSpdMult = GetBaseStat("Attack Speed");

            Item i = owner.GetItem(ResourceManager.ItemIndex.SwordWeapon);
            // Level 2
            // Damage + 25%
            if (i.Level >= 2)
            {
                damageMult += 0.25f;
            }
            // Level 3
            // Pierce + 2
            // Size + 15%
            if (i.Level >= 3)
            {
                pierceAdd += 2;
                sizeMult += 0.15f;
            }

            // Sword Beam Path
            if (i.Level > 4 && i.HasTakenPath("Multi Strike"))
            {
            }

            // Multi Strike Path
            if (i.Level > 4 && i.HasTakenPath("Multi Strike"))
            {
                // Level 5
                if (i.Level >= 5)
                {
                    multiStrike5 = true;
                    atckSpdMult += 0.1f;
                }
                // Level 6
                if (i.Level >= 6)
                {
                    multiStrike6 = true;
                }
                // Level 7
                if (i.Level >= 7)
                {
                    atckSpdMult += 0.45f;
                    damageMult -= 0.25f;
                }
                // Level 8
                if (i.Level >= 8)
                {
                    multiStrike8 = true;
                    atckSpdMult += 0.5f;
                }
            }

            // Set the true stats of the weapon to the correct values
            trueStats["Damage"] = damageMult;
            trueStats["Pierce"] = pierceAdd;
            trueStats["Size"] = sizeMult;
            trueStats["Attack Speed"] = atckSpdMult;
        }
    }

    public override void OnUpdate()
    {
        // Aiming towards mouse
        Vector3 target = MousePosition();
        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (canFire)
        {
            if (multiStrike8)
            {
                // Stab many times and swings several times
                if (swingNum == 0)
                {
                    Slash();
                    Slash(true);
                    swingNum++;
                }
                else if (swingNum == 7)
                {
                    Slash();
                    swingNum++;
                }
                else if (swingNum == 14)
                {
                    Slash(true);
                    swingNum++;
                }
                else if (swingNum <= 20)
                {
                    // Does the stabs
                    if (swingTimer >= swingDelay / (trueStats["Attack Speed"] / GetBaseStat("Attack Speed") * 5))
                    {
                        if (swingNum % 3 == 0)
                        {
                            Stab(true);
                        }
                        else
                        {
                            Stab();
                        }
                        swingNum++;
                        swingTimer = 0;
                    }
                    else
                    {
                        swingTimer += Time.deltaTime;
                    }
                }
                else if (swingNum == 21)
                {
                    // Does the second swing after a short delay
                    if (swingTimer >= swingDelay / (trueStats["Attack Speed"] / GetBaseStat("Attack Speed") * 2))
                    {
                        Slash(false, true);
                        ResetTimer();
                        swingNum = 0;
                        swingTimer = 0;
                    }
                    else
                    {
                        swingTimer += Time.deltaTime;
                    }
                }
                else
                {
                    ResetTimer();
                    swingNum = 0;
                }
            }
            else if (multiStrike6)
            {
                // Stab 3 times inbetween 2 swings
                if (swingNum == 0)
                {
                    Slash();
                    swingNum++;
                }
                else if (swingNum <= 3)
                {
                    // Does the stabs
                    if (swingTimer >= swingDelay / (trueStats["Attack Speed"] / GetBaseStat("Attack Speed") * 3))
                    {
                        Stab();
                        swingNum++;
                        swingTimer = 0;
                    }
                    else
                    {
                        swingTimer += Time.deltaTime;
                    }
                }
                else if (swingNum == 4)
                {
                    // Does the second swing after a short delay
                    if (swingTimer >= swingDelay / (trueStats["Attack Speed"] / GetBaseStat("Attack Speed") * 3))
                    {
                        Slash(true);
                        ResetTimer();
                        swingNum = 0;
                        swingTimer = 0;
                    }
                    else
                    {
                        swingTimer += Time.deltaTime;
                    }
                }
                else
                {
                    ResetTimer();
                    swingNum = 0;
                }
            }
            else if (multiStrike5)
            {
                // Double Swing
                if (swingNum == 0)
                {
                    Slash();
                    swingNum++;
                }
                else if (swingNum == 1)
                {
                    // Does the second swing after a short delay
                    if (swingTimer >= swingDelay / (trueStats["Attack Speed"] / GetBaseStat("Attack Speed")))
                    {
                        Slash(true);
                        ResetTimer();
                        swingNum = 0;
                        swingTimer = 0;
                    }
                    else
                    {
                        swingTimer += Time.deltaTime;
                    }
                }
                else
                {
                    ResetTimer();
                    swingNum = 0;
                }
            }
            else
            {
                Slash();
                ResetTimer();
                swingNum = 0;
            }
        }
    }

    private void Slash(bool invert = false, bool spin = false)
    {
        ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.SwordSlash);
        p.transform.position = this.transform.position;
        p.transform.rotation = transform.rotation;
        if (invert)
        {
            ((SwordSlash)p).InvertSwing();
        }
        p.Pierce = GetStat("Pierce");
        p.SizeMultiplier = GetStat("Size");
        DamageInfo info = new DamageInfo();
        info.damage = owner.Damage * GetStat("Damage");
        info.attacker = owner;
        info.knockbackDirection = p.Direction;
        info.weapon = ResourceManager.WeaponIndex.CupidArrow;
        p.SetDamageInfo(info);
        if (spin)
        {
            ((SwordSlash)p).spin = true;
            p.SizeMultiplier = GetStat("Size") * 1.5f;
            p.Pierce = GetStat("Pierce") + 99;
        }
    }

    private void Stab(bool strong = false)
    {
        ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.SwordStab);
        p.transform.position = this.transform.position;
        p.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-25f, 25f)) * transform.rotation;
        p.Direction = p.transform.right;
        DamageInfo info = new DamageInfo();
        info.damage = owner.Damage * GetStat("Damage") * 0.75f;
        info.attacker = owner;
        info.knockbackDirection = p.Direction;
        info.knockback *= 0.5f;
        info.weapon = ResourceManager.WeaponIndex.SwordSlash;
        p.SetDamageInfo(info);
        p.Pierce = 1;
        p.SizeMultiplier = GetStat("Size");
        if (strong)
        {
            p.SpeedMultiplier = 1.5f;
            p.Pierce += GetStat("Pierce") + 99;
            p.SizeMultiplier = GetStat("Size") * 1.25f;
        }
    }
}
