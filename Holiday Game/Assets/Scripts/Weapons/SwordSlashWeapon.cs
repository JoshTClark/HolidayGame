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
        if (owner.HasItem(ResourceManager.ItemIndex.SwordWeapon))
        {
            float damageMult = GetBaseStat("Damage");
            float pierceAdd = GetBaseStat("Pierce");
            float sizeMult = GetBaseStat("Size");
            float atckSpdMult = GetBaseStat("Attack Speed");

            Item i = owner.GetItem(ResourceManager.ItemIndex.SwordWeapon);
            // Level 2
            if (i.Level >= 2)
            {
                damageMult += 0.25f;
            }
            // Level 3
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

            trueStats["Damage"] = damageMult;
            trueStats["Pierce"] = pierceAdd;
            trueStats["Size"] = sizeMult;
            trueStats["Attack Speed"] = atckSpdMult;
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
            if (swingNum == 0)
            {
                ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.SwordSlash);
                p.transform.position = this.transform.position;
                p.transform.rotation = transform.rotation;
                //p.Direction = transform.right;
                p.Pierce = GetStat("Pierce");
                p.SizeMultiplier = GetStat("Size");
                DamageInfo info = new DamageInfo();
                info.damage = owner.Damage * GetStat("Damage");
                info.attacker = owner;
                info.knockbackDirection = p.Direction;
                info.weapon = ResourceManager.WeaponIndex.CupidArrow;
                p.SetDamageInfo(info);
                swingNum++;
            }
            if (multiStrike5 && swingNum == 1)
            {
                if (swingTimer >= swingDelay)
                {
                    ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.SwordSlash);
                    p.transform.position = this.transform.position;
                    p.transform.rotation = transform.rotation;
                    ((SwordSlash)p).InvertSwing();
                    //p.Direction = transform.right;
                    p.Pierce = GetStat("Pierce");
                    p.SizeMultiplier = GetStat("Size");
                    DamageInfo info = new DamageInfo();
                    info.damage = owner.Damage * GetStat("Damage");
                    info.attacker = owner;
                    info.knockbackDirection = p.Direction;
                    info.weapon = ResourceManager.WeaponIndex.CupidArrow;
                    p.SetDamageInfo(info);
                    swingNum++;
                    swingTimer = 0.0f;
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
    }
}
