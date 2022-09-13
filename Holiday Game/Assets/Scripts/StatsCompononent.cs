using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsCompononent : MonoBehaviour
{
    private float currentHP, baseMaxHP, baseArmor, baseRegen, hpMult, armorMult, regenMult;

    public float BaseMaxHp { get; set; }

    public float BaseArmor { get; set; }

    public float BaseRegen { get; set; }

    public float CurrentHP { get; }

    public float MaxHp { get { return baseMaxHP * hpMult; } }
    public float Armor { get; }
    public float Regen { get; }


}
