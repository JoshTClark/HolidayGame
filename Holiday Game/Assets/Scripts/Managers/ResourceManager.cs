using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public enum EnemyIndex
    {
        None,
        Bat,
        Scarecrow,
        BigBoy,
        Boss,
        Ghost,
        Crow
    }
    public enum ProjectileIndex
    {
        Snowball = 0,
        PumpkinBomb = 1,
        Firework = 2,
        EnemyProjectile = 3,
        Explosion = 4,
        CupidArrow = 5,
        MagicMissile = 6,
        IceShard = 7,
        SwordSlash = 8
    }
    public enum FollowingEffectIndex
    {
        None,
        Burning,
        SnowballEffect
    }
    public enum PickupIndex
    {
        XP1,
        XP2,
        HealthDrop1,
        BossDrop,
        Coin
    }
    public enum WeaponIndex
    {
        Snowball = 0,
        PumpkinBomb = 1,
        Crows = 2,
        Fireworks = 3,
        Null = 4,
        CupidArrow = 5,
        BossAttack = 6,
        BossAttack2 = 7,
        CandyCornRifle = 8,
        SwordSlash = 9
    }

    public enum ItemIndex
    {
        AttackSpeed = 0,
        Damage = 1,
        MoveSpeed = 2,
        SwordWeapon = 3,
        CritChance = 4,
        Fireball = 5,
        Armor = 6,
        AgilityBoots = 7,
        LiftingBelt = 8,
        Meat = 9,
        Duck = 10

    }

    /*
    public enum UpgradeIndex
    {
        Health1,
        Health2,
        Health3,
        Damage1,
        Damage2,
        Damage3,
        Speed1,
        Speed2,
        Speed3,
        AttackSpeed1,
        AttackSpeed2,
        AttackSpeed3,
        SnowballWeaponUpgrade,
        SnowballDamage1,
        SnowballDamage2,
        SnowballDamage3,
        SnowballSize1,
        SnowballSize2,
        SnowballSize3,
        SnowballSpeed1,
        PumpkinBombWeaponUpgrade,
        FireworkWeaponUpgrade,
        PumkinRadius1,
        PumkinRadius2,
        PumkinRadius3,
        FireworkCount,
        GlassCannon1,
        GlassCannon2,
        CritDamage1,
        CritDamage2,
        CritDamage3,
        CritDamage4,
        CritDamage5,
        CritChance1,
        CritChance2,
        CritChance3,
        CritChance4,
        PumpkinDamage1,
        PumpkinDamage2,
        PumpkinDamage3,
        PumpkinLauncher,
        ClusterPumkins,
        XP1,
        XP2,
        XP3,
        CupidArrowWeaponUpgrade,
        CupidArrowSwiftFlight,
        ArrowPierce1,
        ArrowPierce2,
        ArrowBounceDamage,
        Snowballing,
        StunningFireworks,
        CandyCornWeaponUpgrade,
        CandyCornSpray,
        MoreCorn,
        Reflector,
        Patience,
        LowHPDamage,
        CupidArrowHealth,
        SharpShadow,
        ExtraDash,
        SwordSlashWeaponUpgrade
    }
    */

    /*
    public enum UpgradePoolIndex
    {
        Basic,
        Weapons,
        Snowball,
        Pumkin,
        Fireworks,
        CupidArrow,
        CandyCorn,
        SpecialUpgrades
    }
    */

    public enum LevelPoolIndex
    {
        Test
    }

    public enum BuffIndex
    {
        Burning,
        Stunned,
        Bleeding
    }

    public enum OrbitalIndex
    {
        IceShield
    }

    public static List<Enemy> enemyPrefabs = new List<Enemy>();
    public static List<ProjectileBase> projectilePrefabs = new List<ProjectileBase>();
    public static List<Weapon> weaponPrefabs = new List<Weapon>();
    public static List<Upgrade> upgradeDefinitions = new List<Upgrade>();
    public static List<DropBase> pickupPrefabs = new List<DropBase>();
    public static List<UpgradePool> upgradePools = new List<UpgradePool>();
    public static List<BuffDef> buffs = new List<BuffDef>();
    public static List<FollowingEffect> effects = new List<FollowingEffect>();
    public static List<OrbitalParent> orbitals = new List<OrbitalParent>();
    public static List<LevelData> levelDatas = new List<LevelData>();
    public static List<LevelPool> levelPools = new List<LevelPool>();
    public static List<PlayableCharacterData> characters = new List<PlayableCharacterData>();

    public static List<ItemDef> itemDefs = new List<ItemDef>();

    public static bool isLoaded = false;

    /// <summary>
    /// Should be called to load everything that needs to be loaded
    /// </summary>
    public static void Init()
    {
        LoadProjectiles();
        LoadEnemies();
        LoadWeapons();
        LoadUpgrades();
        LoadPickups();
        LoadUpgradePools();
        LoadBuffs();
        LoadEffects();
        LoadPlayableCharacters();
        LoadOrbitals();
        LoadLevels();

        LoadItems();
        isLoaded = true;
    }
    public static void LoadItems()
    {
        ItemDef[] arr = Resources.LoadAll<ItemDef>("");
        foreach (ItemDef i in arr)
        {
            itemDefs.Add(i);
        }
    }

    public static void LoadEnemies()
    {
        Enemy[] arr = Resources.LoadAll<Enemy>("");
        foreach (Enemy i in arr)
        {
            enemyPrefabs.Add(i);
        }
    }

    public static void LoadOrbitals()
    {
        OrbitalParent[] arr = Resources.LoadAll<OrbitalParent>("");
        foreach (OrbitalParent i in arr)
        {
            orbitals.Add(i);
        }
    }

    public static void LoadProjectiles()
    {
        ProjectileBase[] arr = Resources.LoadAll<ProjectileBase>("");
        foreach (ProjectileBase i in arr)
        {
            projectilePrefabs.Add(i);
        }
    }

    public static void LoadWeapons()
    {
        Weapon[] arr = Resources.LoadAll<Weapon>("");
        foreach (Weapon i in arr)
        {
            weaponPrefabs.Add(i);
        }
    }

    public static void LoadUpgrades()
    {
        Upgrade[] arr = Resources.LoadAll<Upgrade>("");
        foreach (Upgrade i in arr)
        {
            upgradeDefinitions.Add(i);
            i.CurrentLevel = 1;
        }
    }

    public static void LoadEffects()
    {
        FollowingEffect[] arr = Resources.LoadAll<FollowingEffect>("");
        foreach (FollowingEffect i in arr)
        {
            effects.Add(i);
        }
    }

    public static void LoadPickups()
    {
        DropBase[] arr = Resources.LoadAll<DropBase>("");

        foreach (DropBase i in arr)
        {
            pickupPrefabs.Add(i);
        }
    }

    public static void LoadUpgradePools()
    {
        UpgradePool[] arr = Resources.LoadAll<UpgradePool>("");
        foreach (UpgradePool i in arr)
        {
            upgradePools.Add(i);
        }
    }

    public static void LoadLevels()
    {
        LevelData[] arr = Resources.LoadAll<LevelData>("");
        foreach (LevelData i in arr)
        {
            levelDatas.Add(i);
        }

        LevelPool[] pools = Resources.LoadAll<LevelPool>("");

        foreach (LevelPool i in pools)
        {
            levelPools.Add(i);
        }
    }

    public static void LoadBuffs()
    {
        BuffDef[] arr = Resources.LoadAll<BuffDef>("");
        foreach (BuffDef i in arr)
        {
            buffs.Add(i);
        }
    }

    public static void LoadPlayableCharacters()
    {
        PlayableCharacterData[] arr = Resources.LoadAll<PlayableCharacterData>("");
        foreach (PlayableCharacterData i in arr)
        {
            characters.Add(i);
        }
    }

    /*
    public static Upgrade GetUpgrade(ResourceManager.UpgradeIndex index)
    {
        foreach (Upgrade i in upgradeDefinitions)
        {
            if (i.index == index)
            {
                Upgrade b = GameObject.Instantiate<Upgrade>(i);
                return b;
            }
        }
        return null;
    }

    public static UpgradePool GetUpgradePool(ResourceManager.UpgradePoolIndex index)
    {
        foreach (UpgradePool i in upgradePools)
        {
            if (i.index == index)
            {
                UpgradePool b = GameObject.Instantiate<UpgradePool>(i);
                return b;
            }
        }
        return null;
    }
    */

    public static Weapon GetWeapon(ResourceManager.WeaponIndex index)
    {
        foreach (Weapon i in weaponPrefabs)
        {
            if (i.index == index)
            {
                return i;
            }
        }
        return null;
    }

    public static BuffDef GetBuffDef(ResourceManager.BuffIndex index)
    {
        foreach (BuffDef i in buffs)
        {
            if (i.index == index)
            {
                BuffDef b = GameObject.Instantiate<BuffDef>(i);
                return b;
            }
        }
        return null;
    }

    public static ProjectileBase GetProjectile(ResourceManager.ProjectileIndex index)
    {
        foreach (ProjectileBase i in projectilePrefabs)
        {
            if (i.index == index)
            {
                ProjectileBase b = GameObject.Instantiate<ProjectileBase>(i);
                return b;
            }
        }
        return null;
    }

    public static FollowingEffect GetEffect(ResourceManager.FollowingEffectIndex index)
    {
        foreach (FollowingEffect i in effects)
        {
            if (i.index == index)
            {
                FollowingEffect b = GameObject.Instantiate<FollowingEffect>(i);
                return b;
            }
        }
        return null;
    }

    public static DropBase GetPickup(ResourceManager.PickupIndex index)
    {
        foreach (DropBase i in pickupPrefabs)
        {
            if (i.index == index)
            {
                DropBase b = GameObject.Instantiate<DropBase>(i);
                return b;
            }
        }
        return null;
    }

    public static OrbitalParent GetOrbital(ResourceManager.OrbitalIndex index)
    {
        foreach (OrbitalParent i in orbitals)
        {
            if (i.index == index)
            {
                return i;
            }
        }
        return null;
    }

    /*
    public static UpgradeIndex GetUpgradeFromWeapon(WeaponIndex index)
    {
        return GetWeapon(index).upgradeIndex;
    }


    public static UpgradeIndex UpgradeIndexFromName(string name)
    {
        UpgradeIndex index = UpgradeIndex.AttackSpeed1;

        foreach (Upgrade i in upgradeDefinitions)
        {
            if (i.upgradeName == name)
            {
                index = i.index;
            }
        }

        return index;
    }
    */
    public static Enemy EnemyFromName(string name)
    {
        Enemy e = null;

        foreach (Enemy i in enemyPrefabs)
        {
            if (i.name == name)
            {
                e = i;
            }
        }

        return e;
    }

    public static DropBase DropFromName(string name)
    {
        DropBase drop = null;

        foreach (DropBase i in pickupPrefabs)
        {
            if (i.name == name)
            {
                drop = i;
            }
        }

        return drop;
    }

    public static Enemy GetEnemyFromIndex(EnemyIndex index)
    {
        foreach (Enemy i in enemyPrefabs)
        {
            if (i.index == index)
            {
                return i;
            }
        }
        return null;
    }

    public static LevelData GetLevelFromSceneName(string scene)
    {
        foreach (LevelData data in levelDatas)
        {
            if (data.scene == scene)
            {
                return data;
            }
        }
        return null;
    }
}
