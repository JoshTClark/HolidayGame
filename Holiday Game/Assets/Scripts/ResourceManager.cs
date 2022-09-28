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
    public static List<GameObject> weaponPrefabs = new List<GameObject>();
    public static List<GameObject> upgradeDefinitions = new List<GameObject>();
    public static List<GameObject> phaseDefinitions = new List<GameObject>();
    public static List<GameObject> pickupPrefabs = new List<GameObject>();

    /// <summary>
    /// Should be called to load everything that needs to be loaded
    /// </summary>
    public static void Init()
    {
        LoadEnemies();
    }

    public static void LoadEnemies()
    {
        Enemy[] arr = Resources.LoadAll<Enemy>("");
        foreach (Enemy i in arr)
        {
            enemyPrefabs.Add(i);
        }
    }

    /*
    public static void LoadWeapons()
    {
        Weapon[] arr = (Weapon[])Resources.LoadAll("", typeof(Weapon));
        foreach (Weapon i in arr)
        {
            weaponPrefabs.Add(i);
        }
    }

    public static void LoadUpgrades()
    {
        Enemy[] arr = (Enemy[])Resources.LoadAll("", typeof(Enemy));
        foreach (Enemy e in arr)
        {
            enemyPrefabs.Add(e);
        }
    }

    public static void LoadPhases()
    {
        Enemy[] arr = (Enemy[])Resources.LoadAll("", typeof(Enemy));
        foreach (Enemy e in arr)
        {
            enemyPrefabs.Add(e);
        }
    }

    public static void LoadPickups()
    {
        Enemy[] arr = (Enemy[])Resources.LoadAll("", typeof(Enemy));
        foreach (Enemy e in arr)
        {
            enemyPrefabs.Add(e);
        }
    }
    */
}
