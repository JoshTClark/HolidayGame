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
    private float xpAmount = 0;
    private float xpToNextLevel = 5f;
    private int level = 1;

    [HideInInspector]
    [SerializeField]
    // Flat additions to stats
    public float hpAdd, speedAdd, damageAdd, attackSpeedAdd, armorAdd, regenAdd, critChanceAdd, critDamageAdd;

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

    //Trackers
    public int vampKills = 0;

    // Flags
    [SerializeField]
    protected bool isDead = false;

    // Weapon List
    public List<Weapon> weapons = new List<Weapon>();

    // Inventory for Items
    public List<Item> inventory = new List<Item>();

    // Debuffs and Buffs
    protected List<BuffHolder> buffs = new List<BuffHolder>();

    protected SpriteRenderer sr;
    Color ogColor;
    bool damaged;
    float fadeTimer;
    float regenTimer = 0f;
    private float patienceTimer = 0.0f;
    protected bool isMoving = true;

    public bool canMove = true;
    public bool isStunned = false;

    public DeathEffect deathEffect;

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
    public int Level { get { return level; } }

    // Used to get the "true" values of stats after calculating any additions from upgrades etc
    public float MaxHp { get { return (baseMaxHP + hpAdd) * hpMult * hpMultConst; } }
    public float Speed { get { return (baseSpeed + speedAdd) * speedMult * speedMultConst; } }
    public float Damage { get { return (baseDamage + damageAdd) * damageMult * damageMultConst; } }
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

        // Check if entity should be stunned
        if (HasBuff(BuffIndex.Stunned))
        {
            isStunned = true;
        }
        else
        {
            isStunned = false;
        }

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

        // Buff updating
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if (buffs[i].IsActive)
            {
                // Update the holder
                buffs[i].Update(delta);
            }
            else
            {
                // Remove buff from list if it is no longer active
                buffs.RemoveAt(i);
            }
        }
    }

    //Checks to see if leveled up since last tick
    public void CalculateLevel()
    {
        // XP from level 1 to 2 is always 5
        if (level == 1)
        {
            if (xpAmount >= 5)
            {
                level++;
                xpAmount -= 5;
                if (this.gameObject.GetComponent<Player>())
                {
                    this.gameObject.GetComponent<Player>().waitingForLevels++;
                }
                xpToNextLevel = (level - 1) * 15;
            }
        }
        else
        {
            // XP required to level up increases by 15 every level
            if (xpAmount >= xpToNextLevel)
            {
                level++;
                xpAmount -= xpToNextLevel;
                if (this.gameObject.GetComponent<Player>())
                {
                    this.gameObject.GetComponent<Player>().waitingForLevels++;
                }
                xpToNextLevel = (level - 1) * 15;
            }
        }
    }

    // Deals damage here
    public virtual void TakeDamage(DamageInfo info)
    {
        info.receiver = this;
        info.CalculateAll();

        // Sword Upgrade - if stunned take more damage
        if (info.attacker.HasItem(ResourceManager.ItemIndex.SwordWeapon)
            && info.attacker.GetItem(ResourceManager.ItemIndex.SwordWeapon).HasTakenPath("Stun")
            && info.attacker.GetItem(ResourceManager.ItemIndex.SwordWeapon).Level >= 6
            && isStunned)
        {
            info.damage *= 1.15f;
            if (info.attacker.GetItem(ResourceManager.ItemIndex.SwordWeapon).Level >= 8)
            {
                foreach (Weapon weapon in info.attacker.weapons)
                {
                    if (weapon.index != WeaponIndex.SwordSlash)
                    {
                        weapon.ReduceCooldown(0.01f);
                    }
                }
                if (info.attacker.GetType() == typeof(Player))
                {
                    Player p = (Player)info.attacker;
                    p.dashCooldownTimer += 0.01f * p.DashCooldown;
                }
            }
        }

        if (Armor > 0)
        {
            if (info.attacker.HasItem(ItemIndex.CritChance)) // If receiver has armor, check if attacker has armor piercer
            {
                Item i = info.attacker.GetItem(ItemIndex.CritChance);

                if (info.isCrit && i.HasTakenPath("Weak Spots"))
                {
                    //Calculate new armor value if attacker had armor pen
                    if (i.Level == 4)
                    {
                        armorMult *= 0.8f;
                    }
                    else if (i.Level == 5)
                    {
                        armorMult *= 0.6f;
                    }
                    else if (i.Level == 6)
                    {
                        armorMult *= 0.25f;
                    }

                    float damageReduction = (-1f / ((0.1f * Mathf.Sqrt(Armor)) + 1f)) + 1f;

                    info.damage *= 1f - damageReduction;
                }

            }
            else // dont have item, do normal calc
            {
                float damageReduction = (-1f / ((0.1f * Mathf.Sqrt(Armor)) + 1f)) + 1f;

                info.damage *= 1f - damageReduction;
            }
        }

        // Armor Item Capstones
        if (this.HasItem(ItemIndex.Armor))
        {
            Item i = GetItem(ItemIndex.Armor);

            if (i.HasTakenPath("Spiked Armor")) //Damage reflection path
            {
                DamageInfo reflectInfo = new DamageInfo();
                reflectInfo.attacker = info.receiver;
                reflectInfo.receiver = info.attacker;
                if (i.Level == 4)
                {
                    reflectInfo.damage = (this.Armor / 6);
                }
                else if (i.Level == 5)
                {
                    reflectInfo.damage = (this.Armor / 5);
                }
                else if (i.Level == 6)
                {
                    reflectInfo.damage = (this.Armor / 4);
                }

                info.attacker.TakeDamage(reflectInfo);
            }

            if (i.HasTakenPath("Mythril Coating")) // Damage reduction path
            {
                if (i.Level == 4)
                {
                    info.damage *= 0.9f;
                }
                else if (i.Level == 5)
                {
                    info.damage *= 0.8f;
                }
                else if (i.Level == 6)
                {
                    info.damage *= 0.7f;
                }
            }
        }

        // Boxing Gloves Upgrades
        if (info.attacker.HasItem(ItemIndex.LiftingBelt))
        {
            Item i = info.attacker.GetItem(ItemIndex.LiftingBelt);

            if (i.HasTakenPath("Boxing Lessons"))
            {
                if (i.Level == 4)
                {
                    info.damage += info.attacker.CurrentHP * 0.02f;
                }
                if (i.Level == 5)
                {
                    info.damage += info.attacker.CurrentHP * 0.05f;
                }
                if (i.Level == 6)
                {
                    info.damage += info.attacker.CurrentHP * 0.05f;
                    info.damage += info.attacker.MaxHp * 0.01f;
                }
            }
        }

        // Vampire Fang Upgrades
        if (info.attacker.HasItem(ItemIndex.Vampire))
        {
            Item i = info.attacker.GetItem(ItemIndex.Vampire);
            if (i.Level == 1)
            {
                if (info.receiver.currentHP > info.damage) info.attacker.Heal(info.damage * 0.005f);
                else info.attacker.Heal(info.receiver.currentHP * 0.005f);
            }
            else if (i.Level == 2)
            {
                if (info.receiver.currentHP > info.damage) info.attacker.Heal(info.damage * 0.01f);
                else info.attacker.Heal(info.receiver.currentHP * 0.01f);
            }
            else if (i.Level > -3)
            {
                if (info.receiver.currentHP > info.damage) info.attacker.Heal(info.damage * 0.02f);
                else info.attacker.Heal(info.receiver.currentHP * 0.02f);
            }

            if (i.HasTakenPath("See Red")) // bonus damage and healing when low hp
            {
                if (i.Level == 4 && info.attacker.currentHP < (info.attacker.MaxHp / 4))
                {
                    info.damage *= 1.2f;
                    if (info.receiver.currentHP > info.damage) info.attacker.Heal(info.damage * 0.02f);
                    else info.attacker.Heal(info.receiver.currentHP * 0.02f);
                }
                else if (i.Level == 6 && info.attacker.currentHP < (info.attacker.MaxHp / 4))
                {
                    info.damage *= 1.3f;
                    if (info.receiver.currentHP > info.damage) info.attacker.Heal(info.damage * 0.03f);
                    else info.attacker.Heal(info.receiver.currentHP * 0.03f);
                }
                else if (i.Level == 6 && info.attacker.currentHP < (info.attacker.MaxHp / 4))
                {
                    info.damage *= 1.5f;
                    if (info.receiver.currentHP > info.damage) info.attacker.Heal(info.damage * 0.05f);
                    else info.attacker.Heal(info.receiver.currentHP * 0.05f);
                }
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

        if (info.attacker.HasItem(ItemIndex.CritChance))
        {
            Item i = info.attacker.GetItem(ItemIndex.CritChance);
            if (info.isCrit && i.HasTakenPath("Vicious Wounds") && i.Level >= 4)
            {
                Debug.Log("Bleed should be applied");
                //info.debuffs.Add(BuffIndex.Bleeding); Not working commenting out for now
            }
        }

        // Applying buffs/debuffs
        foreach (BuffInfo i in info.buffs)
        {
            // DoT effects
            if (i.GetType() == typeof(DotInfo))
            {
                DotInfo dot = (DotInfo)i;
                DotHolder dotHolder = new DotHolder(dot.index, dot.duration, dot.tickRate, dot.damagePerTick, info.attacker, this);
                buffs.Add(dotHolder);
            }
            else
            {
                BuffHolder buff = new BuffHolder(i.index, i.duration, i.isDebuff, info.attacker);
                buffs.Add(buff);
            }
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
            OnDeath(info);

            if (deathEffect)
            {
                DeathEffect gameObject = GameObject.Instantiate<DeathEffect>(deathEffect);
                if (sr.flipX) gameObject.GetComponent<SpriteRenderer>().flipX = true;
                gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0f);
                gameObject.GetComponent<SpriteRenderer>().sprite = this.gameObject.GetComponent<SpriteRenderer>().sprite;
            }
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
        SetLevelAndXP(i, 0);
    }

    public float GetXpToNextLevel()
    {
        return xpToNextLevel;
    }

    public float GetPercentToNextLevel()
    {
        return xpAmount / xpToNextLevel;
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
    public void AddItem(Item item)
    {
        if (item.Level == 0) { item.Level++; }
        inventory.Add(item);
        if (item.itemDef.GetType() == typeof(WeaponDef))
        {
            AddWeapon(((WeaponDef)item.itemDef).weaponPrefab);
        }
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
        foreach (BuffHolder b in buffs)
        {
            if (b.index == index)
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
        /* Item has been replaced for now
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
        */

        //Agility Boots Item
        if (HasItem(ItemIndex.AgilityBoots))
        {
            Item i = GetItem(ItemIndex.AgilityBoots);
            // Level 1
            if (i.Level >= 1)
            {
                speedAdd += 0.25f;
                attackSpeedMult *= 1.1f;
            }
            // Level 2
            if (i.Level >= 2)
            {
                speedAdd += 0.25f;
                attackSpeedMult *= 1.1f;
            }
            // Level 3
            if (i.Level >= 3)
            {
                speedAdd += 0.25f;
                attackSpeedMult *= 1.1f;
            }
            // Path 1
            if (i.HasTakenPath("Swift Strider"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    speedAdd += 0.25f;
                    attackSpeedMult *= 1.1f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    speedAdd += 0.25f;
                    attackSpeedMult *= 1.1f;
                }
                // Level 6
                if (i.Level >= 6)
                {
                    speedAdd += 0.25f;
                    attackSpeedMult *= 1.1f;
                }


            }

            // Path 2
            if (i.HasTakenPath("Berzerker's Boots"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    speedAdd += 0.1f;
                    attackSpeedAdd += 0.3f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    speedAdd += 0.1f;
                    attackSpeedAdd += 0.3f;
                }
                if (i.Level >= 6)
                {
                    attackSpeedMult *= 2f;
                    hpMult *= 0.666f;
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
                critChanceAdd += 0.1f;
            }
            // Level 3
            if (i.Level >= 3)
            {
                critDamageAdd += 0.4f;
            }
            // Path 1
            if (i.HasTakenPath("Weak Spots"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    critChanceAdd += 0.1f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    critChanceAdd += 0.1f;
                }
                if (i.Level >= 6)
                {
                    critChanceAdd += 0.1f;
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
                // Level 5
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
            if (i.HasTakenPath("Spiked Armor"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    armorAdd += 1.5f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    armorAdd += 1.5f;
                }
                if (i.Level >= 6)
                {
                    armorAdd += 1.5f;
                }

            }
            //path 1 capstone

            // Path 2
            if (i.HasTakenPath("Mythril Coating"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    armorAdd += 0.5f;
                    hpAdd += 10f;
                    speedAdd += 0.25f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    armorAdd += 0.5f;
                    hpAdd += 10f;
                    speedAdd += 0.25f;
                }
                if (i.Level >= 6)
                {
                    armorAdd += 0.5f;
                    hpAdd += 10f;
                    speedAdd += 0.25f;
                }

            }


        }

        //Lifting Belt Item
        if (HasItem(ItemIndex.LiftingBelt))
        {
            Item i = GetItem(ItemIndex.LiftingBelt);
            // Level 1
            if (i.Level >= 1)
            {
                hpAdd += 10f;
                damageAdd += 1f;
            }
            // Level 2
            if (i.Level >= 2)
            {
                hpAdd += 10f;
                damageAdd += 1f;
            }
            // Level 3
            if (i.Level >= 3)
            {
                hpAdd += 10f;
                damageAdd += 1f;
            }
            // Path 1
            if (i.HasTakenPath("Beefcake"))
            {
                // Level 4
                if (i.Level == 4)
                {
                    hpAdd += 30f;
                    hpMult *= 1.1f;

                }
                // Level 5
                if (i.Level == 5)
                {
                    hpAdd += 30f;
                    hpMult *= 1.2f;
                }
                // Level 6
                if (i.Level >= 6)
                {
                    hpAdd += 40f;
                    hpMult *= 1.4f;
                }

            }

            // Path 2
            if (i.HasTakenPath("Boxing Lessons"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    hpAdd += 15f;

                }
                // Level 5
                if (i.Level >= 5)
                {
                    hpAdd += 15f;

                }
                if (i.Level >= 6)
                {
                    hpAdd += 20f;

                }

            }

        }

        // Meat Item
        if (HasItem(ItemIndex.Meat))
        {
            Item i = GetItem(ItemIndex.Meat);
            // Level 1
            if (i.Level >= 1)
            {
                hpAdd += 10f;
                regenAdd += 0.5f;
            }
            // Level 2
            if (i.Level >= 2)
            {
                hpAdd += 10f;
                regenAdd += 0.5f;
            }
            // Level 3
            if (i.Level >= 3)
            {
                hpAdd += 10f;
                regenAdd += 0.5f;
            }
            // Path 1
            if (i.HasTakenPath("Mutated Cows"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    hpAdd += 10f;
                    regenAdd += 1f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    hpAdd += 10f;
                    regenAdd += 1f;
                }
                // Level 6
                if (i.Level >= 6)
                {
                    hpAdd += 10f;
                    regenAdd += 1f;
                }
            }

            // Path 2
            if (i.HasTakenPath("Bigger Stomach"))
            {
                // Level 4
                if (i.Level >= 4)
                {
                    hpAdd += 20f;
                    regenAdd += 0.25f;
                }
                // Level 5
                if (i.Level >= 5)
                {
                    hpAdd += 20f;
                    regenAdd += 0.25f;
                }
                // Level 6
                if (i.Level >= 5)
                {
                    hpAdd += 25f;
                    regenAdd += 0.25f;
                }

            }

        }

        // Golden Duck Item
        if (HasItem(ItemIndex.Duck))
        {
            Item i = GetItem(ItemIndex.Duck);
            // Level 1
            if (i.Level >= 1)
            {
                hpAdd += 10f;
                speedAdd += 0.2f;
                damageAdd += 0.5f;
                attackSpeedAdd += 0.1f;
                armorAdd += 0.5f;
                regenAdd += 1f;
                critChanceAdd += 0.05f;
                critDamageAdd += 0.2f;
            }
            // Level 2
            if (i.Level >= 2)
            {
                hpAdd += 10f;
                speedAdd += 0.2f;
                damageAdd += 0.5f;
                attackSpeedAdd += 0.1f;
                armorAdd += 0.5f;
                regenAdd += 1f;
                critChanceAdd += 0.05f;
                critDamageAdd += 0.2f;
            }
            // Level 3
            if (i.Level >= 3)
            {
                hpAdd += 10f;
                speedAdd += 0.2f;
                damageAdd += 0.5f;
                attackSpeedAdd += 0.1f;
                armorAdd += 0.5f;
                regenAdd += 1f;
                critChanceAdd += 0.05f;
                critDamageAdd += 0.2f;
            }
            // Level 4
            if (i.Level >= 4)
            {
                hpAdd += 10f;
                speedAdd += 0.2f;
                damageAdd += 0.5f;
                attackSpeedAdd += 0.1f;
                armorAdd += 0.5f;
                regenAdd += 1f;
                critChanceAdd += 0.05f;
                critDamageAdd += 0.2f;
            }
            // Level 5
            if (i.Level >= 5)
            {
                hpAdd += 10f;
                speedAdd += 0.2f;
                damageAdd += 0.5f;
                attackSpeedAdd += 0.1f;
                armorAdd += 0.5f;
                regenAdd += 1f;
                critChanceAdd += 0.05f;
                critDamageAdd += 0.2f;
            }
            // Level 6
            if (i.Level >= 6)
            {
                hpAdd += 10f;
                speedAdd += 0.2f;
                damageAdd += 0.5f;
                attackSpeedAdd += 0.1f;
                armorAdd += 0.5f;
                regenAdd += 1f;
                critChanceAdd += 0.05f;
                critDamageAdd += 0.2f;
            }

        }

        if (HasItem(ItemIndex.Vampire))
        {
            Item i = GetItem(ItemIndex.Vampire);

            if (i.Level >= 1)
            {
                damageMult *= 1.2f;
            }
            if (i.Level >= 2)
            {
                damageMult *= 1.2f;
            }
            if (i.Level >= 3)
            {
                damageMult *= 1.2f;
            }

            if (i.HasTakenPath("See Red"))
            {
                if (i.Level >= 4)
                {
                    damageMult *= 1.2f;
                }
                if (i.Level >= 5)
                {
                    damageMult *= 1.2f;
                }
                if (i.Level >= 6)
                {
                    damageMult *= 1.2f;
                }
            }

            if (i.HasTakenPath("Blood Transfusions"))
            {
                if (i.Level >= 4)
                {
                    damageMult *= 1.1f;
                    hpAdd += 10f;
                }
                if (i.Level >= 5)
                {
                    damageMult *= 1.1f;
                    hpAdd += 10f;
                }
                if (i.Level >= 6)
                {
                    damageMult *= 1.1f;
                    hpAdd += 10f;
                    hpAdd += vampKills / 20;
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

    public void SetLevelAndXP(int level, float xp)
    {
        this.level = level;
        xpAmount = xp;
        if (level == 1)
        {
            xpToNextLevel = 5;
        }
        else
        {
            xpToNextLevel = (level - 1) * 15;
        }
    }

    protected class BuffHolder
    {
        public BuffIndex index;
        public bool isDebuff;
        public StatsComponent inflictor;
        public float duration;

        protected bool isActive;
        protected float timer = 0f;

        public bool IsActive
        {
            get { return isActive; }
        }

        public float TimeActive
        {
            get { return timer; }
        }

        public BuffHolder(BuffIndex index = 0, float duration = 1f, bool isDebuff = false, StatsComponent inflictor = null)
        {
            isActive = true;
            this.index = index;
            this.duration = duration;
            this.isDebuff = isDebuff;
            this.inflictor = inflictor;
        }

        /// <summary>
        /// Updates the buffs timer and checks if it is still active
        /// </summary>
        /// <param name="delta"></param>
        public virtual void Update(float delta)
        {
            if (isActive)
            {
                timer += delta;
                if (timer >= duration)
                {
                    isActive = false;
                    timer = duration;
                }
            }
        }
    }

    protected class DotHolder : BuffHolder
    {
        public StatsComponent inflicted;

        private float tickRate;
        private float damagePerTick;
        private float tickTimer = 0f;

        public DotHolder(BuffIndex index = 0, float duration = 1f, float tickRate = 0.25f, float damagePerTick = 0f, StatsComponent inflictor = null, StatsComponent inflicted = null)
        {
            isActive = true;
            this.index = index;
            this.duration = duration;
            isDebuff = true;
            this.inflictor = inflictor;
            this.inflicted = inflicted;
            this.tickRate = tickRate;
            this.damagePerTick = damagePerTick;
        }

        /// <summary>
        /// Updates the buffs timer and checks if it is still active
        /// </summary>
        /// <param name="delta"></param>
        public override void Update(float delta)
        {
            if (isActive)
            {
                timer += delta;
                tickTimer += delta;
                if (tickTimer >= tickRate)
                {
                    tickTimer = 0f;
                    DamageInfo info = new DamageInfo();
                    info.damage = damagePerTick;
                    info.attacker = inflictor;
                    info.neverCrit = true;
                    info.knockback = 0;
                    inflicted.TakeDamage(info);
                }
                if (timer >= duration)
                {
                    isActive = false;
                    timer = duration;
                }
            }
        }
    }
}
