using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum EnemyIndex
    {
        None,
        Test,
        Test2
    }

    public enum WeaponIndex
    {
        None,
        Snowball,
        Test
    }

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private List<Enemy> enemyPrefabs = new List<Enemy>();

    [SerializeField]
    private List<Weapon> weaponPrefabs = new List<Weapon>();

    private List<Enemy> currentEnemies = new List<Enemy>();

    private float time = 0.0f;

    public float GameTime
    {
        get { return time; }
    }

    public List<Enemy> CurrentEnemies
    {
        get
        {
            return currentEnemies;
        }
    }

    // Only a single gamemanger should ever exist so we can always get it here
    public static GameManager instance;

    public Player player;

    void Start()
    {
        instance = this;

        // Just testing adding enemies and an attack to the player
        currentEnemies.Add(Instantiate<Enemy>(GetEnemyFromIndex(EnemyIndex.Test2), new Vector2(0, 5), Quaternion.identity));
        foreach (Enemy e in currentEnemies)
        {
            e.player = player;
        }
        GivePlayerWeapon(WeaponIndex.Snowball);
    }

    void Update()
    {
        time += Time.deltaTime;

        // Moving the camera
        Vector3 camPos = new Vector3(player.transform.position.x, player.transform.position.y, cam.transform.position.z);
        cam.transform.position = camPos;

        // Enemy death
        for(int i = 0; i < currentEnemies.Count; i++)
        {
            if (currentEnemies[i].IsDead)
            {
                Enemy e = currentEnemies[i];
                currentEnemies.Remove(e);
                Destroy(e.gameObject);
                i--;
            }
        }
    }

    // Gets an enemy prefab from the list using the index
    public Enemy GetEnemyFromIndex(EnemyIndex index)
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

    // Gets a weapon prefab from the list using the index
    public Weapon GetWeaponFromIndex(WeaponIndex index)
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

    // Gives the player the desired weapon
    public void GivePlayerWeapon(WeaponIndex index)
    {
        Weapon weapon = GetWeaponFromIndex(index);
        player.AddAttack(weapon);
    }
}
