using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManager;

public abstract class StatsComponent : MonoBehaviour
{
    // All of these are base stats
    // They shouldn't be changed during runtime and should be set in the inspector
    // If you add a new stat you shuld also add an add and mult variable and a property for the base and true values
    [SerializeField]
    private float baseMaxHP, baseSpeed, baseDamage, baseAttackSpeed, baseArmor, baseRegen, baseRegenInterval, baseCritChance, baseCritDamage, knockbackMult;

    // Level up stuff
    [SerializeField]
    private float levelScaling, hpLevelUp, damageLevelUp;

    // CurrentHP is here and is handled by this script
    public float currentHP;

    // Level
    [SerializeField]
    private float xpAmount, level;

    [SerializeField]
    // Flat additions to stats
    private float hpAdd, speedAdd, damageAdd, attackSpeedAdd, armorAdd, regenAdd, critChanceAdd, critDamageAdd;

    [SerializeField]
    private AnimationCurve expCurve;

    // Multipliers to stats
    private float hpMult, speedMult, damageMult, attackSpeedMult, armorMult, regenMult, regenIntervalMult, critChanceMult, critDamageMult;

    // Flags
    [SerializeField]
    private bool isDead = false;

    // Weapon List
    public List<Weapon> weapons = new List<Weapon>();

    // Inventory for upgrades
    public List<Upgrade> inventory = new List<Upgrade>();

    // Debuffs and Buffs
    public List<BuffInfo> buffs = new List<BuffInfo>();

    protected SpriteRenderer sr;
    Color ogColor;
    bool damaged;
    float fadeTimer;
    float vFade;
    float regenTimer = 0f;

    [SerializeField]
    float fadeTotalTime;

    // Used to get the base stats without changing them at all
    public float BaseMaxHp { get; }
    public float BaseSpeed { get; }
    public float BaseDamage { get; }
    public float BaseArmor { get; }
    public float BaseRegen { get; }
    public float BaseRegenInterval { get; }
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

    public float RegenInterval { get { return (baseRegenInterval * regenIntervalMult); } }
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
        regenIntervalMult = 1.0f;

        fadeTotalTime = .2f;
        damaged = false;

        currentHP = MaxHp;
        xpAmount = 0;
        if (!this.gameObject.GetComponent<Enemy>())
        {
            level = 1;
        }


        sr = gameObject.GetComponent<SpriteRenderer>();
        ogColor = sr.color;

        if (baseRegenInterval <= 0)
        {
            baseRegenInterval = 10;
        }

        OnStart();
    }

    protected void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.Normal)
        {
            float delta = Time.deltaTime;
            CalculateStats();
            CheckWeapons();

            OnUpdate();

            if (damaged)
            {
                fadeTimer += delta;
                sr.color = Color.Lerp(Color.red, ogColor, fadeTimer / fadeTotalTime);
                if (fadeTimer > fadeTotalTime)
                {
                    damaged = false;
                }
            }

            // Doing regen
            regenTimer += delta;
            if (regenTimer >= RegenInterval)
            {
                regenTimer = 0;
                if (Regen > 0)
                {
                    Heal(Regen);
                }
            }

            // Checks if should be dead
            if (currentHP <= 0)
            {
                isDead = true;
            }

            // Adding things for burning effects
            if (HasBuff(ResourceManager.BuffIndex.Burning))
            {
            }

            // Buff updating
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                buffs[i].DoTick();
                if (!buffs[i].active)
                {
                    buffs.RemoveAt(i);
                }
            }

            // Checking if dead
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

    /// <summary>
    /// Checks what weapons the player should have
    /// </summary>
    private void CheckWeapons()
    {
        if (HasUpgrade(ResourceManager.UpgradeIndex.SnowballWeaponUpgrade))
        {
            bool giveSnowball = true;
            foreach (Weapon w in weapons)
            {
                if (w.GetType() == typeof(SnowballWeapon))
                {
                    giveSnowball = false;
                }
            }
            if (giveSnowball)
            {
                AddAttack(ResourceManager.GetWeapon(ResourceManager.WeaponIndex.Snowball));
            }
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CupidArrowWeaponUpgrade))
        {
            bool giveArrow = true;
            foreach (Weapon w in weapons)
            {
                if (w.GetType() == typeof(CupidArrow))
                {
                    giveArrow = false;
                }
            }
            if (giveArrow)
            {
                AddAttack(ResourceManager.GetWeapon(ResourceManager.WeaponIndex.CupidArrow));
            }
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.PumpkinBombWeaponUpgrade))
        {
            bool givePumkin = true;
            foreach (Weapon w in weapons)
            {
                if (w.GetType() == typeof(PumpkinBomb))
                {
                    givePumkin = false;
                }
            }
            if (givePumkin)
            {
                AddAttack(ResourceManager.GetWeapon(ResourceManager.WeaponIndex.PumpkinBomb));
            }
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.FireworkWeaponUpgrade))
        {
            bool giveFirework = true;
            foreach (Weapon w in weapons)
            {
                if (w.GetType() == typeof(FireworkWeapon))
                {
                    giveFirework = false;
                }
            }
            if (giveFirework)
            {
                AddAttack(ResourceManager.GetWeapon(ResourceManager.WeaponIndex.Fireworks));
            }
        }
    }

    //Checks to see if leveled up since last tick
    public void CalculateLevel()
    {
        if (!this.gameObject.GetComponent<Enemy>())
        {
            float expToLevelUp = GetXpToNextLevel();
            if (XP > expToLevelUp)
            {
                level++;
                OnLevelUp();
            }
        }
    }

    //What happens when the player levels up
    private void OnLevelUp()
    {
        //checks if missing hp, heals for 20 if so
        CalculateStats();
        if (this.gameObject.GetComponent<Player>())
        {
            GameManager.instance.DoPlayerLevelUp();
        }
    }

    // Deals damage here
    public virtual void TakeDamage(DamageInfo info)
    {
        info.receiver = this;
        info.CalculateAll();
        if (Armor > 0)
        {
            float damageReduction = (-1f / ((0.1f * Mathf.Sqrt(Armor)) + 1f)) + 1f;
            info.damage *= damageReduction;
        }

        if (this.gameObject.GetComponent<Enemy>())
        {
            if (info.radialKnockback)
            {
                Vector2 knockbackDirection = (info.damagePos - (Vector2)this.gameObject.transform.position).normalized;
                this.gameObject.GetComponent<Enemy>().AddKnockback(knockbackDirection * info.knockback * knockbackMult);
            }
            else
            {
                this.gameObject.GetComponent<Enemy>().AddKnockback(info.knockbackDirection * info.knockback * knockbackMult);
            }
        }

        foreach (ResourceManager.BuffIndex i in info.debuffs)
        {
            BuffDef def = ResourceManager.GetBuffDef(i);
            BuffInfo buff = new BuffInfo();
            buff.def = def;
            DamageInfo buffInfo = new DamageInfo();
            buffInfo.attacker = info.attacker;
            buffInfo.receiver = info.receiver;
            buff.infoTemplate = buffInfo;
            buff.afflicting = this;
            if (i == ResourceManager.BuffIndex.Burning)
            {
                buff.totalDamage = info.attacker.Damage;
            }
            buffs.Add(buff);
        }

        // reducing health
        currentHP -= info.damage;
        GameManager.instance.DisplayDamage(info);
        sr.color = Color.red;
        damaged = true;
        fadeTimer = 0;
    }

    // Adds an attack
    public void AddAttack(Weapon attack)
    {
        attack.owner = this;
        weapons.Add(Instantiate(attack, transform));
    }

    //Called when xp collides with player, adds an amount of xp to players total
    public void AddXP(float amount)
    {
        float increase = 1f;
        if (HasUpgrade(ResourceManager.UpgradeIndex.XP1))
        {
            increase += 0.05f * GetUpgrade(ResourceManager.UpgradeIndex.XP1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.XP2))
        {
            increase += 0.1f * GetUpgrade(ResourceManager.UpgradeIndex.XP2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.XP3))
        {
            increase += 0.15f * GetUpgrade(ResourceManager.UpgradeIndex.XP3).CurrentLevel;
        }
        amount *= increase;

        xpAmount += amount;
    }

    public void SetLevel(int i)
    {
        level = i;
        CalculateStats();
        currentHP = MaxHp;
    }

    public float GetXpToNextLevel()
    {
        //create inverse speedcurve
        return expCurve.Evaluate(level + 1);
    }

    public float GetPercentToNextLevel()
    {
        // Testing for right now until we get an actual level curve
        return (XP - expCurve.Evaluate(level)) / (GetXpToNextLevel() - expCurve.Evaluate(level));
    }

    public float GetPercentHealth()
    {
        // Testing for right now until we get an actual level curve
        return CurrentHP / MaxHp;
    }

    /// <summary>
    /// Adds the upgrade to the players inventory
    /// </summary>
    public void AddUpgrade(ResourceManager.UpgradeIndex index)
    {
        if (HasUpgrade(index))
        {
            GetUpgrade(index).CurrentLevel++;
        }
        else
        {
            inventory.Add(ResourceManager.GetUpgrade(index));
        }
        CalculateStats();
        OnGainUpgrade(ResourceManager.GetUpgrade(index));
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

    public Upgrade GetUpgrade(ResourceManager.UpgradeIndex index)
    {
        foreach (Upgrade i in inventory)
        {
            if (i.index == index)
            {
                return i;
            }
        }
        return null;
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, MaxHp);
        GameManager.instance.DisplayHealing(amount, this);
    }

    public bool HasBuff(ResourceManager.BuffIndex index)
    {
        foreach (BuffInfo b in buffs)
        {
            if (b.def.index == index)
            {
                return true;
            }
        }
        return false;
    }

    public abstract void OnUpdate();

    public abstract void OnDeath();

    public abstract void OnStart();

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
        CalculateLevel();
        hpAdd += ((level - 1) * levelScaling) * hpLevelUp;
        damageAdd += ((level - 1) * levelScaling) * damageLevelUp;

        // UPGRADES

        // HP1 - HP3
        if (HasUpgrade(ResourceManager.UpgradeIndex.Health1))
        {
            hpAdd += 15 * GetUpgrade(ResourceManager.UpgradeIndex.Health1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Health2))
        {
            hpAdd += 30 * GetUpgrade(ResourceManager.UpgradeIndex.Health2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Health3))
        {
            hpAdd += 45 * GetUpgrade(ResourceManager.UpgradeIndex.Health3).CurrentLevel;
        }

        // Damage1 - Damage3
        if (HasUpgrade(ResourceManager.UpgradeIndex.Damage1))
        {
            damageAdd += (0.05f * baseDamage) * GetUpgrade(ResourceManager.UpgradeIndex.Damage1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Damage2))
        {
            damageAdd += (0.1f * baseDamage) * GetUpgrade(ResourceManager.UpgradeIndex.Damage2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Damage3))
        {
            damageAdd += (0.15f * baseDamage) * GetUpgrade(ResourceManager.UpgradeIndex.Damage3).CurrentLevel;
        }

        // Speed1 - Speed3
        if (HasUpgrade(ResourceManager.UpgradeIndex.Speed1))
        {
            speedMult += 0.05f * GetUpgrade(ResourceManager.UpgradeIndex.Speed1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Speed2))
        {
            speedMult += 0.1f * GetUpgrade(ResourceManager.UpgradeIndex.Speed2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Speed3))
        {
            speedMult += 0.2f * GetUpgrade(ResourceManager.UpgradeIndex.Speed3).CurrentLevel;
        }

        // AttackSpeed1 - AttackSpeed3
        if (HasUpgrade(ResourceManager.UpgradeIndex.AttackSpeed1))
        {
            attackSpeedAdd += 0.05f * GetUpgrade(ResourceManager.UpgradeIndex.AttackSpeed1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.AttackSpeed2))
        {
            attackSpeedAdd += 0.10f * GetUpgrade(ResourceManager.UpgradeIndex.AttackSpeed2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.AttackSpeed3))
        {
            attackSpeedAdd += 0.20f * GetUpgrade(ResourceManager.UpgradeIndex.AttackSpeed3).CurrentLevel;
        }

        // Glass Cannon
        if (HasUpgrade(ResourceManager.UpgradeIndex.GlassCannon1))
        {
            attackSpeedMult *= 2f * GetUpgrade(ResourceManager.UpgradeIndex.GlassCannon1).CurrentLevel;
            damageMult *= MathF.Pow(0.75f, GetUpgrade(ResourceManager.UpgradeIndex.GlassCannon1).CurrentLevel);
            hpMult *= MathF.Pow(0.75f, GetUpgrade(ResourceManager.UpgradeIndex.GlassCannon1).CurrentLevel);
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.GlassCannon2))
        {
            attackSpeedAdd *= 4f * GetUpgrade(ResourceManager.UpgradeIndex.GlassCannon2).CurrentLevel;
            damageMult *= MathF.Pow(0.50f, GetUpgrade(ResourceManager.UpgradeIndex.GlassCannon2).CurrentLevel);
            hpMult *= MathF.Pow(0.50f, GetUpgrade(ResourceManager.UpgradeIndex.GlassCannon2).CurrentLevel);
        }

        // Making sure hp is <= maxHP
        if (currentHP > MaxHp)
        {
            currentHP = MaxHp;
        }

        // Crit Chance
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritChance1))
        {
            critChanceAdd += 0.025f * GetUpgrade(ResourceManager.UpgradeIndex.CritChance1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritChance2))
        {
            critChanceAdd += 0.05f * GetUpgrade(ResourceManager.UpgradeIndex.CritChance2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritChance3))
        {
            critChanceAdd += 0.075f * GetUpgrade(ResourceManager.UpgradeIndex.CritChance3).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritChance4))
        {
            critChanceAdd += 0.1f * GetUpgrade(ResourceManager.UpgradeIndex.CritChance4).CurrentLevel;
        }

        // Crit Damage
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritDamage1))
        {
            critDamageAdd += 0.05f * GetUpgrade(ResourceManager.UpgradeIndex.CritDamage1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritDamage2))
        {
            critDamageAdd += 0.1f * GetUpgrade(ResourceManager.UpgradeIndex.CritDamage2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritDamage3))
        {
            critDamageAdd += 0.2f * GetUpgrade(ResourceManager.UpgradeIndex.CritDamage3).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritDamage4))
        {
            critDamageAdd += 0.3f * GetUpgrade(ResourceManager.UpgradeIndex.CritDamage4).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritDamage5))
        {
            critDamageAdd += 0.5f * GetUpgrade(ResourceManager.UpgradeIndex.CritDamage5).CurrentLevel;
        }
    }

    public void OnGainUpgrade(Upgrade upgrade)
    {
        // Heal when taking these upgrades
        if (upgrade.index == ResourceManager.UpgradeIndex.Health1)
        {
            Heal(20);
        }
        if (upgrade.index == ResourceManager.UpgradeIndex.Health2)
        {
            Heal(40);
        }
        if (upgrade.index == ResourceManager.UpgradeIndex.Health3)
        {
            Heal(60);
        }
    }
}
