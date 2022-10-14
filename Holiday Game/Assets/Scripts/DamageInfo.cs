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
    public DamageColor damageColor = DamageColor.Basic;
    public StatsComponent receiver;
    public StatsComponent attacker;
    public bool isCrit;

    public void CalculateAll()
    {
        isCrit = GameManager.RollCheck(attacker.CritChance);
        if (isCrit)
        {
            damage *= attacker.CritDamage;
            damageColor = DamageColor.Crit;
        }
    }

    public DamageInfo CreateCopy()
    {
        DamageInfo info = new DamageInfo();
        info.damage = this.damage;
        info.receiver = this.receiver;
        info.attacker = this.attacker;
        info.isCrit = this.isCrit;
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
