using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatsComponent : MonoBehaviour
{
    // All of these are base stats
    // They shouldn't be changed during runtime and should be set in the inspector
    // If you add a new stat you shuld also add an add and mult variable and a property for the base and true values
    [SerializeField]
    private float baseMaxHP, baseSpeed, baseDamage, baseAttackSpeed, baseArmor, baseRegen, baseCritChance, baseCritDamage;

    // Level up stuff
    [SerializeField]
    private float levelScaling, hpLevelUp, damageLevelUp;

    // CurrentHP is here and is handled by this script
    public float currentHP;

    // Level
    private float xpAmount, level;

    [SerializeField]
    // Flat additions to stats
    private float hpAdd, speedAdd, damageAdd, attackSpeedAdd, armorAdd, regenAdd, critChanceAdd, critDamageAdd;

    // Multipliers to stats
    private float hpMult, speedMult, damageMult, attackSpeedMult, armorMult, regenMult, critChanceMult, critDamageMult;

    // Flags
    [SerializeField]
    private bool isDead = false;

    // Weapon List
    private List<Weapon> attacks = new List<Weapon>();

    // Inventory for upgrades
    private List<Upgrade> inventory = new List<Upgrade>();

    // Used to get the base stats without changing them at all
    public float BaseMaxHp { get; }
    public float BaseSpeed { get; }
    public float BaseDamage { get; }
    public float BaseArmor { get; }
    public float BaseRegen { get; }
    public float BaseCritChance { get; }
    public float BaseCritDamage { get; }


    // Don't use this to do damage there should be a damage method
    public float CurrentHP { get { return currentHP; } }

    // Level
    public float XP { get { return xpAmount; } }
    public float Level { get { return level; } }

    // Used to get the "true" values of stats after calculating any additions from upgrades etc
    public float MaxHp { get { return (baseMaxHP + hpAdd) * hpMult; } }
    public float Speed { get { return (baseSpeed + speedAdd) * speedMult; } }
    public float Damage { get { return (baseDamage + damageAdd) * damageMult; } }
    public float AttackSpeed { get { return (baseAttackSpeed + attackSpeedAdd) * attackSpeedMult; } }
    public float Armor { get { return (baseArmor + armorAdd) * armorMult; } }
    public float Regen { get { return (baseRegen + regenAdd) * regenMult; } }
    public float CritChance { get { return (baseCritChance + critChanceAdd) * critChanceMult; } }
    public float CritDamage { get { return (baseCritDamage + critDamageAdd) * critDamageMult; } }

    // Flags
    public bool IsDead { get { return isDead; } }

    protected void Start()
    {
        hpMult = 1.0f;
        speedMult = 1.0f;
        damageMult = 1.0f;
        attackSpeedMult = 1.0f;
        armorMult = 1.0f;
        regenMult = 1.0f;
        critChanceMult = 1.0f;
        critDamageMult = 1.0f;

        currentHP = MaxHp;
        xpAmount = 0;
        level = 1;

        OnStart();
    }

    protected void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.Normal)
        {
            CalculateStats();

            OnUpdate();

            // Checks if should be dead
            if (currentHP <= 0)
            {
                isDead = true;
            }

            if (IsDead)
            {
                OnDeath();
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2();
        }
    }

    // This method is where all the stat multipliers and additions are calculated based on upgrades
    // Every frame all the stat changes are set to their default values and then recaclculated
    // This makes it so this is the only place that we need to check for stat changes because of upgrades
    // It also makes the whole system much more expandable
    private void CalculateStats()
    {
        // Resetting stats
        hpAdd = 0f;
        speedAdd = 0f;
        damageAdd = 0f;
        attackSpeedAdd = 0f;
        armorAdd = 0f;
        regenAdd = 0f;
        critChanceAdd = 0f;
        critDamageAdd = 0f;

        hpMult = 1.0f;
        speedMult = 1.0f;
        damageMult = 1.0f;
        attackSpeedMult = 1.0f;
        armorMult = 1.0f;
        regenMult = 1.0f;
        critChanceMult = 1.0f;
        critDamageMult = 1.0f;

        // LEVELS
        hpAdd += ((level - 1) * levelScaling) * hpLevelUp;
        damageAdd += ((level - 1) * levelScaling) * damageLevelUp;
        CalculateLevel();

        // UPGRADES
    }

    //Checks to see if leveled up since last tick
    public void CalculateLevel()
    {

        float tempLevel = level + (float)Math.Floor(xpAmount / GetXpToNextLevel()); // what the level should be based on xp gained

        if (tempLevel > level) // if the calculated level is higher than the plaers level, then level up
        {
            level++;
            OnLevelUp();
        }
    }

    //What happens when the player levels up
    private void OnLevelUp()
    {
        //checks if missing hp, heals for 20 if so
        if (currentHP < MaxHp)
        {
            currentHP += 20;
        }
        else currentHP = MaxHp;
    }

    // Deals damage here
    public virtual void DealDamage(float damage)
    {
        currentHP -= damage;
    }

    // Adds an attack
    public void AddAttack(Weapon attack)
    {
        attacks.Add(Instantiate(attack, transform));
    }

    //Called when xp collides with player, adds an amount of xp to players total
    public void AddXP(float amount)
    {
        xpAmount += amount;

        if (currentHP < MaxHp) // heals for 5 when pick up xp // should be removed once health drops/health regen are incorporated.
        {
            currentHP += 5;
        }
        else currentHP = MaxHp;

    }

    public void SetLevel(int i)
    {
        level = i;
        CalculateStats();
        currentHP = MaxHp;
    }

    public float GetXpToNextLevel()
    {
        // Testing for right now until we get an actual level curve
        return 50 * Level;
    }

    public float GetPercentToNextLevel()
    {
        // Testing for right now until we get an actual level curve
        return (XP - (50 * (level - 1))) / (GetXpToNextLevel() - (50 * (level - 1)));
    }

    /// <summary>
    /// Returns true if the upgrade is in the inventory
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool HasUpgrade(ResourceManager.UpgradeIndex index)
    {
        foreach (Upgrade i in inventory)
        {
            if (i.index == index)
            {
                return true;
            }
        }
        return false;
    }

    public abstract void OnUpdate();

    public abstract void OnDeath();

    public abstract void OnStart();
}
