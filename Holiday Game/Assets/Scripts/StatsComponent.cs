using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static ResourceManager;

public abstract class StatsComponent : MonoBehaviour
{
    // All of these are base stats
    // They shouldn't be changed during runtime and should be set in the inspector
    // If you add a new stat you shuld also add an add and mult variable and a property for the base and true values
    [SerializeField]
    private float baseMaxHP, baseSpeed, baseDamage, baseAttackSpeed, baseArmor, baseRegen, baseRegenInterval, baseCritChance, baseCritDamage, knockbackMult;

    // CurrentHP is here and is handled by this script
    public float currentHP;

    // Level
    [SerializeField]
    private float xpAmount;
    private int level;

    [HideInInspector]
    [SerializeField]
    // Flat additions to stats
    public float hpAdd, speedAdd, damageAdd, attackSpeedAdd, armorAdd, regenAdd, critChanceAdd, critDamageAdd;

    [SerializeField]
    private AnimationCurve expCurve;

    [HideInInspector]
    // Multipliers to stats
    public float hpMult, speedMult, damageMult, attackSpeedMult, armorMult, regenMult, regenIntervalMult, critChanceMult, critDamageMult, coolDownMult;

    // Multipliers for enemies
    [HideInInspector]
    public float hpMultConst = 1.0f;
    [HideInInspector]
    public float damageMultConst = 1.0f;
    [HideInInspector]
    public float speedMultConst = 1.0f;


    // Flags
    [SerializeField]
    protected bool isDead = false;

    // Weapon List
    public List<Weapon> weapons = new List<Weapon>();

    // Inventory for Items
    public List<Item> inventory = new List<Item>();

    // Debuffs and Buffs
    public List<BuffInfo> buffs = new List<BuffInfo>();

    protected SpriteRenderer sr;
    Color ogColor;
    bool damaged;
    float fadeTimer;
    float vFade;
    float regenTimer = 0f;
    private float patienceTimer = 0.0f;
    protected bool isMoving = true;

    [SerializeField]
    float fadeTotalTime;

    // Used to get the base stats without changing them at all
    public float BaseMaxHp { get { return baseMaxHP; } }
    public float BaseSpeed { get { return baseSpeed; } }
    public float BaseDamage { get { return baseDamage; } }
    public float BaseAttackSpeed { get { return baseAttackSpeed; } }
    public float BaseArmor { get { return baseArmor; } }
    public float BaseRegen { get { return baseRegen; } }
    public float BaseRegenInterval { get { return baseRegenInterval; } }
    public float BaseCritChance { get { return baseCritChance; } }
    public float BaseCritDamage { get { return baseCritDamage; } }


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
    public float CooldownMultiplier { get { return coolDownMult; } }

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
        coolDownMult = 1.0f;

        fadeTotalTime = .2f;
        damaged = false;

        currentHP = MaxHp;
        if (this.gameObject.GetComponent<Player>() && xpAmount == 0)
        {
            level = 1;
            xpAmount = expCurve.Evaluate(1);
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

            // Patience
            /*
            if (HasUpgrade(ResourceManager.UpgradeIndex.Patience) && !isMoving)
            {
                patienceTimer += delta;
            }
            else if (isMoving)
            {
                patienceTimer = 0;
            }
            */

            CalculateStats();

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
                if (Regen != 0)
                {
                    Heal(Regen);
                }
            }

            // Checks if should be dead
            //if (currentHP <= 0)
            //{
            //    isDead = true;
            //}

            // Adding things for burning effects
            if (HasBuff(ResourceManager.BuffIndex.Burning))
            {
                // Nothing
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
            //if (IsDead)
            //{
            //    OnDeath(DamageInfo info);
            //}
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2();
        }
    }

    //Checks to see if leveled up since last tick
    public void CalculateLevel()
    {
        if (!this.gameObject.GetComponent<Enemy>())
        {
            int tempLevel = level;
            float xpToLevelUp = expCurve.Evaluate(tempLevel + 1);
            while (XP > xpToLevelUp)
            {
                tempLevel++;
                xpToLevelUp = expCurve.Evaluate(tempLevel + 1);
            }
            if (tempLevel > level)
            {
                OnLevelUp(tempLevel - level);
                level = tempLevel;
            }
        }
    }

    //What happens when the player levels up
    private void OnLevelUp(int levels)
    {
        if (this.gameObject.GetComponent<Player>())
        {
            ((Player)this).waitingForLevels++;
            SoundManager.instance.LevelUp();
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

        if (this.HasItem(ItemIndex.Armor))
        {
            Item i = GetItem(ItemIndex.Armor);
            if (i.HasTakenPath("Spiked Armor"))
            {
                DamageInfo reflectInfo = new DamageInfo();
                reflectInfo.attacker = info.receiver;
                reflectInfo.receiver = info.attacker;
                reflectInfo.damage = (this.Armor / 2);
                info.attacker.TakeDamage(reflectInfo);
            }
            if (i.HasTakenPath("Mythril Armor"))
            {
                info.damage *= 0.75f;
            }
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

        // Damage Tracking
        if (info.weapon == WeaponIndex.CandyCornRifle)
        {
            GameManager.instance.cornDamageDone += info.damage;
        }
        else if (info.weapon == WeaponIndex.Snowball)
        {
            GameManager.instance.snowballDamageDone += info.damage;
        }
        else if (info.weapon == WeaponIndex.PumpkinBomb)
        {
            GameManager.instance.pumpkinDamageDone += info.damage;
        }
        else if (info.weapon == WeaponIndex.CupidArrow)
        {
            GameManager.instance.arrowDamageDone += info.damage;
        }
        else if (info.weapon == WeaponIndex.Fireworks)
        {
            GameManager.instance.fireworkDamageDone += info.damage;
        }

        // reducing health
        currentHP -= info.damage;
        GameManager.instance.DisplayDamage(info);
        sr.color = Color.red;
        damaged = true;
        fadeTimer = 0;
        if (currentHP <= 0)
        {
            isDead = true;

            // Kill Tracking
            if (info.weapon == WeaponIndex.CandyCornRifle)
            {
                GameManager.instance.cornKills++;
            }
            else if (info.weapon == WeaponIndex.Snowball)
            {
                GameManager.instance.snowballKills++;
            }
            else if (info.weapon == WeaponIndex.PumpkinBomb)
            {
                GameManager.instance.pumpkinKills++;
            }
            else if (info.weapon == WeaponIndex.CupidArrow)
            {
                GameManager.instance.arrowKills++;
            }
            else if (info.weapon == WeaponIndex.Fireworks)
            {
                GameManager.instance.fireworkKills++;
            }

            OnDeath(info);
            //Debug.Log("I'm Dying");
        }


    }

    // Adds an attack
    public void AddWeapon(Weapon attack)
    {
        attack.owner = this;
        weapons.Add(Instantiate(attack, transform));
    }

    //Called when xp collides with player, adds an amount of xp to players total
    public void AddXP(float amount)
    {
        float increase = 1f;
        /*
        if (HasUpgrade(ResourceManager.UpgradeIndex.XP1))
        {
            increase += 0.05f * GetItem(ResourceManager.UpgradeIndex.XP1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.XP2))
        {
            increase += 0.1f * GetItem(ResourceManager.UpgradeIndex.XP2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.XP3))
        {
            increase += 0.15f * GetItem(ResourceManager.UpgradeIndex.XP3).CurrentLevel;
        }
        */
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
        if (currentHP != 0 && MaxHp != 0)
        {
            return CurrentHP / MaxHp;
        }
        return 0;
    }

    /// <summary>
    /// Adds the item to the players inventory
    /// </summary>
    public void AddItem(ResourceManager.ItemIndex index)
    {
    }

    /// <summary>
    /// Returns true if the item is in the inventory
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool HasItem(ResourceManager.ItemIndex index)
    {
        foreach (Item i in inventory)
        {
            if (i.itemDef.index == index)
            {
                return true;
            }
        }
        return false;
    }

    public Item GetItem(ResourceManager.ItemIndex index)
    {
        foreach (Item i in inventory)
        {
            if (i.itemDef.index == index)
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
        if (amount > 0)
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

    public bool HasWeapon(ResourceManager.WeaponIndex index)
    {
        foreach (Weapon w in weapons)
        {
            if (w.index == index)
            {
                return true;
            }
        }
        return false;
    }

    public Weapon GetWeapon(ResourceManager.WeaponIndex index)
    {
        foreach (Weapon w in weapons)
        {
            if (w.index == index)
            {
                return w;
            }
        }
        return null;
    }

    public void RemoveItem(ResourceManager.ItemIndex index)
    {
        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            if (inventory[i].itemDef.index == index)
            {
                inventory.RemoveAt(i);
                break;
            }
        }
    }

    public abstract void OnUpdate();

    public abstract void OnDeath(DamageInfo info);

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

        // Attack speed item for testing --- If you write a new item base how you program stat changes off of this
        if (HasItem(ItemIndex.AttackSpeed))
        {
            Item i = GetItem(ItemIndex.AttackSpeed);
            // Level 1
            if (i.Level >= 1)
            {
                attackSpeedAdd += 0.15f;
            }
            // Level 2
            if (i.Level >= 2)
            {
                attackSpeedAdd += 0.15f;
            }
            // Level 3
            if (i.Level >= 3)
            {
                attackSpeedAdd += 0.15f;
            }
            // Path 1
            if (i.HasTakenPath("Pure Attack Speed"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    attackSpeedAdd += 0.25f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    attackSpeedAdd += 0.25f;
                }
                // Level 6
                if (i.Level >= 6)
                {
                    attackSpeedAdd += 0.50f;
                }
            }
            // Path 2
            if (i.HasTakenPath("Situational"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    attackSpeedAdd += 0.25f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    attackSpeedAdd += 0.25f;
                }
                // Level 6
                if (i.Level >= 6)
                {
                    attackSpeedAdd += 0.50f;
                }
            }
        }

        //Crit Upgrade Tree
        if (HasItem(ItemIndex.CritChance))
        {
            Item i = GetItem(ItemIndex.CritChance);
            // Level 1
            if (i.Level >= 1)
            {
                critChanceAdd += 0.05f;
                critDamageAdd += 0.25f;
            }
            // Level 2
            if (i.Level >= 2)
            {
                critChanceAdd += 0.15f;
            }
            // Level 3
            if (i.Level >= 3)
            {
                critDamageAdd += 0.5f;
            }
            // Path 1
            if (i.HasTakenPath("Weak Spots"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    critChanceAdd += 0.15f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    critChanceAdd += 0.15f;
                }

            }
            //path 1 capstone
            if(i.HasTakenPath("Armor Piercer"))
            {
                // Level 6
                if (i.Level >= 6)
                {
                    critChanceAdd += 0.15f;
                }
            }
            // Path 2
            if (i.HasTakenPath("Big Red Numbers"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    critDamageAdd += 0.5f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    critDamageAdd += 0.5f;
                }

            }
            //path 2 capstone
            if(i.HasTakenPath("Vicious Wounds"))
            {
                // Level 6
                if (i.Level >= 6)
                {
                    critDamageAdd += 0.5f;
                }
            }
        }

        //Plate Armor Item
        if (HasItem(ItemIndex.Armor))
        {
            Item i = GetItem(ItemIndex.Armor);
            // Level 1
            if (i.Level >= 1)
            {
                armorAdd += 1f;
                hpAdd += 10f;
            }
            // Level 2
            if (i.Level >= 2)
            {
                armorAdd += 1f;
                hpAdd += 10f;
            }
            // Level 3
            if (i.Level >= 3)
            {
                armorAdd += 1f;
                hpAdd += 10f;
            }
            // Path 1
            if (i.HasTakenPath("Heavy Plating"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    armorAdd += 2f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    armorAdd += 2f;
                }

            }
            //path 1 capstone
            if (i.HasTakenPath("Spiked Armor"))
            {
                // Level 6
                if (i.Level >= 6)
                {
                    armorAdd += 2f;
             
                }
            }
            // Path 2
            if (i.HasTakenPath("Better Blacksmithing"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    armorAdd += 1f;
                    hpAdd += 10f;
                    speedAdd += 0.5f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    armorAdd += 1f;
                    hpAdd += 10f;
                    speedAdd += 0.5f;
                }

            }
            //path 2 capstone
            if (i.HasTakenPath("Mythril Coating"))
            {
                // Level 6
                if (i.Level >= 6)
                {
                    armorAdd += 1f;
                    hpAdd += 10f;
                    speedAdd += 0.5f;
                }
            }

        }
     

        /*
        // UPGRADES

        // HP1 - HP3
        if (HasUpgrade(ResourceManager.UpgradeIndex.Health1))
        {
            hpAdd += 15 * GetItem(ResourceManager.UpgradeIndex.Health1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Health2))
        {
            hpAdd += 30 * GetItem(ResourceManager.UpgradeIndex.Health2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Health3))
        {
            hpAdd += 45 * GetItem(ResourceManager.UpgradeIndex.Health3).CurrentLevel;
        }

        // Damage1 - Damage3
        if (HasUpgrade(ResourceManager.UpgradeIndex.Damage1))
        {
            damageAdd += (0.5f) * GetItem(ResourceManager.UpgradeIndex.Damage1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Damage2))
        {
            damageAdd += (1f) * GetItem(ResourceManager.UpgradeIndex.Damage2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Damage3))
        {
            damageAdd += (2f) * GetItem(ResourceManager.UpgradeIndex.Damage3).CurrentLevel;
        }

        // Speed1 - Speed3
        if (HasUpgrade(ResourceManager.UpgradeIndex.Speed1))
        {
            speedMult += 0.05f * GetItem(ResourceManager.UpgradeIndex.Speed1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Speed2))
        {
            speedMult += 0.1f * GetItem(ResourceManager.UpgradeIndex.Speed2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.Speed3))
        {
            speedMult += 0.2f * GetItem(ResourceManager.UpgradeIndex.Speed3).CurrentLevel;
        }

        // AttackSpeed1 - AttackSpeed3
        if (HasUpgrade(ResourceManager.UpgradeIndex.AttackSpeed1))
        {
            attackSpeedAdd += 0.1f * GetItem(ResourceManager.UpgradeIndex.AttackSpeed1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.AttackSpeed2))
        {
            attackSpeedAdd += 0.20f * GetItem(ResourceManager.UpgradeIndex.AttackSpeed2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.AttackSpeed3))
        {
            attackSpeedAdd += 0.30f * GetItem(ResourceManager.UpgradeIndex.AttackSpeed3).CurrentLevel;
        }

        // Glass Cannon
        if (HasUpgrade(ResourceManager.UpgradeIndex.GlassCannon1))
        {
            attackSpeedMult *= 2f * GetItem(ResourceManager.UpgradeIndex.GlassCannon1).CurrentLevel;
            hpMult *= MathF.Pow(0.5f, GetItem(ResourceManager.UpgradeIndex.GlassCannon1).CurrentLevel);
        }

        // Making sure hp is <= maxHP
        if (currentHP > MaxHp)
        {
            currentHP = MaxHp;
        }

        // Crit Chance
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritChance1))
        {
            critChanceAdd += 0.025f * GetItem(ResourceManager.UpgradeIndex.CritChance1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritChance2))
        {
            critChanceAdd += 0.05f * GetItem(ResourceManager.UpgradeIndex.CritChance2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritChance3))
        {
            critChanceAdd += 0.075f * GetItem(ResourceManager.UpgradeIndex.CritChance3).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritChance4))
        {
            critChanceAdd += 0.1f * GetItem(ResourceManager.UpgradeIndex.CritChance4).CurrentLevel;
        }

        // Crit Damage
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritDamage1))
        {
            critDamageAdd += 0.05f * GetItem(ResourceManager.UpgradeIndex.CritDamage1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritDamage2))
        {
            critDamageAdd += 0.1f * GetItem(ResourceManager.UpgradeIndex.CritDamage2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritDamage3))
        {
            critDamageAdd += 0.2f * GetItem(ResourceManager.UpgradeIndex.CritDamage3).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritDamage4))
        {
            critDamageAdd += 0.3f * GetItem(ResourceManager.UpgradeIndex.CritDamage4).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.CritDamage5))
        {
            critDamageAdd += 0.5f * GetItem(ResourceManager.UpgradeIndex.CritDamage5).CurrentLevel;
        }

        // Low HP Damage
        if (HasUpgrade(ResourceManager.UpgradeIndex.LowHPDamage))
        {
            if (currentHP != 0)
            {
                damageMult *= 1 + ((MaxHp - currentHP) / 100);
                speedMult *= 1 + (1 - (currentHP / MaxHp));
                attackSpeedMult *= 1 + ((MaxHp - currentHP) / 100);
            }
        }

        // Patience
        if (HasUpgrade(ResourceManager.UpgradeIndex.Patience))
        {
            attackSpeedMult *= Mathf.Clamp(1 + (4 * (patienceTimer / (30f * CooldownMultiplier))), 1f, 4f);
        }

        if (this.GetComponent<Player>())
        {
            if (SessionManager.data != null)
            {
                damageMult += SessionManager.data.GetUpgradeLevel(MetaUpgrade.MetaUpgradeID.Damage) * 0.05f;
                attackSpeedMult += SessionManager.data.GetUpgradeLevel(MetaUpgrade.MetaUpgradeID.AttackSpeed) * 0.05f;
                speedMult += SessionManager.data.GetUpgradeLevel(MetaUpgrade.MetaUpgradeID.Speed) * 0.05f;
                armorAdd += SessionManager.data.GetUpgradeLevel(MetaUpgrade.MetaUpgradeID.Armor) * 1f;
                regenAdd += SessionManager.data.GetUpgradeLevel(MetaUpgrade.MetaUpgradeID.Regen) * 0.1f;
                hpAdd += SessionManager.data.GetUpgradeLevel(MetaUpgrade.MetaUpgradeID.Health) * 5f;
            }
        */
    }

    public void OnGainUpgrade(Item item)
    {
        // Heal when taking these upgrades
        /*
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

        if (upgrade.index == ResourceManager.UpgradeIndex.Reflector)
        {
            OrbitalParent p = Instantiate<OrbitalParent>(GetOrbital(OrbitalIndex.IceShield), transform);
            p.owner = this;
        }
        */
    }


    /// <summary>
    /// Used for Predictive behavior, tells where the object should be in the future using current velocity
    /// </summary>
    /// <param name="seconds"> How many seconds into the future your looking</param>
    /// <returns>The objects future position</returns>
    public Vector2 CalcFuturePosition(float seconds)
    {
        return (Vector2)transform.position + (GetComponent<Rigidbody2D>().velocity * seconds);
    }

    public void SetXPWithoutLevelUp(float xp)
    {
        xpAmount = xp;
        if (!this.gameObject.GetComponent<Enemy>())
        {
            int tempLevel = level;
            float xpToLevelUp = expCurve.Evaluate(tempLevel + 1);
            while (XP > xpToLevelUp)
            {
                tempLevel++;
                xpToLevelUp = expCurve.Evaluate(tempLevel + 1);
            }
            if (tempLevel > level)
            {
                level = tempLevel;
            }
        }
    }
}
