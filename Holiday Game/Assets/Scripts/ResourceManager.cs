using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public enum EnemyIndex
    {
        None,
        Test,
        Test2
    }
    public enum PickupIndex
    {
        XP1,
        XP2
    }
    public enum WeaponIndex
    {
        Snowball,
        Test,
        Count
    }
    public enum UpgradeIndex
    {
        Test
    }

    public static List<Enemy> enemyPrefabs = new List<Enemy>();
    public static List<Weapon> weaponPrefabs = new List<Weapon>();
    public static List<Upgrade> upgradeDefinitions = new List<Upgrade>();
    public static List<SpawnPhaseScriptableObject> phaseDefinitions = new List<SpawnPhaseScriptableObject>();
    public static List<XP> pickupPrefabs = new List<XP>();
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
        SpawnPhaseScriptableObject[] arr = Resources.LoadAll<SpawnPhaseScriptableObject>("");
        foreach (SpawnPhaseScriptableObject i in arr)
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

    public static void LoadPlayableCharacters()
    {
        Player[] arr = Resources.LoadAll<Player>("");
        playerPrefab = arr[0];
    }
}
