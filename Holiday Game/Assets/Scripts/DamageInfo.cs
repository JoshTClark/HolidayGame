using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    public enum DamageColor
    {
        Basic,
        Crit
    }

    public float damage;
    public bool radialKnockback = false;
    public float knockback = 1f;
    public Vector2 knockbackDirection = new Vector2(0, 0);
    public Vector2 damagePos;
    public Collider2D otherCollider;
    public DamageColor damageColor = DamageColor.Basic;
    public StatsComponent receiver;
    public StatsComponent attacker;
    public bool isCrit;
    public bool critOveride = false;
    public List<ResourceManager.BuffIndex> debuffs = new List<ResourceManager.BuffIndex>();
    public ResourceManager.WeaponIndex weapon;
    public List<ResourceManager.BuffIndex> mask = new List<ResourceManager.BuffIndex>();

    public void CalculateAll()
    {
        if (attacker)
        {
            isCrit = GameManager.RollCheck(attacker.CritChance);
            if (critOveride)
            {
                isCrit = true;
            }
            if (isCrit)
            {
                damage *= attacker.CritDamage;
                damageColor = DamageColor.Crit;
            }
        }
    }

    public DamageInfo CreateCopy()
    {
        DamageInfo info = new DamageInfo();
        info.damage = this.damage;
        info.receiver = this.receiver;
        info.attacker = this.attacker;
        info.isCrit = this.isCrit;
        info.debuffs = debuffs;
        info.knockback = this.knockback;
        info.knockbackDirection = this.knockbackDirection;
        info.radialKnockback = this.radialKnockback;
        info.weapon = this.weapon;
        info.critOveride = this.critOveride;
        info.otherCollider = this.otherCollider;
        return info;
    }

    public Color GetColor()
    {
        if (damageColor == DamageColor.Crit)
        {
            return new Color(1, 0.35f, 0, 1);
        }
        else
        {
            return new Color(1f, 1f, 1f, 1f);
        }
    }
}
