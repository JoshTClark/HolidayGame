using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public enum EnemyIndex
    {
        None,
        Test,
        Test2,
        BigBoy
    }
    public enum PickupIndex
    {
        XP1,
        XP2
    }
    public enum WeaponIndex
    {
        Snowball,
        PumpkinBomb,
        Test,
        Fireworks,
        Count
    }
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
        CritChance4
    }

    public enum UpgradePoolIndex
    {
        Basic,
        Weapons,
        Snowball,
        Pumkin,
        Fireworks
    }

    public static List<Enemy> enemyPrefabs = new List<Enemy>();
    public static List<Weapon> weaponPrefabs = new List<Weapon>();
    public static List<Upgrade> upgradeDefinitions = new List<Upgrade>();
    public static List<SpawnPhase> phaseDefinitions = new List<SpawnPhase>();
    public static List<XP> pickupPrefabs = new List<XP>();
    public static List<UpgradePool> upgradePools = new List<UpgradePool>();
    public static Player playerPrefab;

    public static bool isLoaded = false;

    /// <summary>
    /// Should be called to load everything that needs to be loaded
    /// </summary>
    public static void Init()
    {
        LoadEnemies();
        LoadWeapons();
        LoadUpgrades();
        LoadPhases();
        LoadPickups();
        LoadUpgradePools();
        LoadPlayableCharacters();
        isLoaded = true;
    }

    public static void LoadEnemies()
    {
        Enemy[] arr = Resources.LoadAll<Enemy>("");
        foreach (Enemy i in arr)
        {
            enemyPrefabs.Add(i);
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
        }
    }

    public static void LoadPhases()
    {
        SpawnPhase[] arr = Resources.LoadAll<SpawnPhase>("");
        foreach (SpawnPhase i in arr)
        {
            phaseDefinitions.Add(i);
        }
    }

    public static void LoadPickups()
    {
        XP[] arr = Resources.LoadAll<XP>("");
        foreach (XP i in arr)
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

    public static void LoadPlayableCharacters()
    {
        Player[] arr = Resources.LoadAll<Player>("");
        playerPrefab = arr[0];
    }

    public static Upgrade GetUpgrade(ResourceManager.UpgradeIndex index)
    {
        foreach (Upgrade i in upgradeDefinitions)
        {
            if (i.index == index)
            {
                return i;
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
                return i;
            }
        }
        return null;
    }
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
}
