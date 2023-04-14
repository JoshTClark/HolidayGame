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
    public bool alwaysCrit = false;
    public bool neverCrit = false;
    public List<BuffInfo> buffs = new List<BuffInfo>();
    public ResourceManager.WeaponIndex weapon;
    public float proc = 1.0f;

    /// <summary>
    /// Calculates the total damage after effects like critical strike
    /// </summary>
    public void CalculateAll()
    {
        if (attacker)
        {
            isCrit = GameManager.RollCheck(attacker.CritChance);
            if (alwaysCrit)
            {
                isCrit = true;
            }
            if (neverCrit)
            {
                isCrit = false;
            }
            if (isCrit)
            {
                damage *= attacker.CritDamage;
                damageColor = DamageColor.Crit;
            }
        }
    }

    /// <summary>
    /// Creates a copy of the damage info
    /// </summary>
    /// <returns></returns>
    public DamageInfo CreateCopy()
    {
        DamageInfo info = new DamageInfo();
        info.damage = this.damage;
        info.receiver = this.receiver;
        info.attacker = this.attacker;
        info.isCrit = this.isCrit;
        info.buffs = buffs;
        info.knockback = this.knockback;
        info.knockbackDirection = this.knockbackDirection;
        info.radialKnockback = this.radialKnockback;
        info.weapon = this.weapon;
        info.alwaysCrit = this.alwaysCrit;
        info.otherCollider = this.otherCollider;
        return info;
    }

    /// <summary>
    /// Returns the color that the damage numbers should be
    /// </summary>
    /// <returns></returns>
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
