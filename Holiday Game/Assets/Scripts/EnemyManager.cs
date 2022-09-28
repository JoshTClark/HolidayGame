using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public enum EnemyIndex
    {
        None,
        Test,
        Test2
    }

    public enum XPIndex
    {
        XP1,
        XP2
    }

    [SerializeField]
    private List<Enemy> enemyPrefabs = new List<Enemy>();

    [SerializeField]
    private List<SpawnPhaseScriptableObject> phases = new List<SpawnPhaseScriptableObject>();

    [SerializeField]
    private List<XP> xpPrefabs = new List<XP>();

    [SerializeField]
    private Vector2 minSpawnDistance, maxSpawnDistance;

    private float spawnTimer = 0;

    private SpawnPhaseScriptableObject currentPhase;

    private List<Enemy> currentEnemies = new List<Enemy>();

    public static EnemyManager instance;

    public List<Enemy> CurrentEnemies
    {
        get { return currentEnemies; }
    }

    public void Start()
    {
        instance = this;
        FindCurrentPhase();
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        spawnTimer += delta;
        if (spawnTimer >= currentPhase.spawnInterval)
        {
            SpawnEnemiesByPhase();
            spawnTimer = 0;
        }

        RemoveDeadEnemies();

        FindCurrentPhase();
    }

    // Removes dead enemies from the game
    public void RemoveDeadEnemies()
    {
        for (int i = currentEnemies.Count - 1; i >= 0; i--)
        {
            Enemy e = currentEnemies[i];
            if (e.IsDead)
            {
                currentEnemies.RemoveAt(i);
                Destroy(e.gameObject);
            }
        }
    }

    // Spawns enemies
    public void SpawnEnemy(EnemyIndex index)
    {
        Enemy prefab = GetEnemyFromIndex(index);
        Vector2 spawnPos = new Vector2();
        float pX = Random.Range(minSpawnDistance.x, maxSpawnDistance.x);
        float pY = Random.Range(minSpawnDistance.y, maxSpawnDistance.y);
        Vector2 playerPos = GameManager.instance.Player.transform.position;
        if (GameManager.RollCheck(0.5f))
        {
            pX *= -1;
        }
        if (GameManager.RollCheck(0.5f))
        {
            pY *= -1;
        }

        spawnPos.x = playerPos.x + pX;
        spawnPos.y = playerPos.y + pY;

        Enemy spawned = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawned.player = GameManager.instance.Player;
        spawned.SetLevel((int)GameManager.instance.CurrentDifficulty);
        currentEnemies.Add(spawned);
    }

    private void SpawnEnemiesByPhase()
    {
        EnemyIndex[] indices = currentPhase.GetSpawnWave();
        for (int i = 0; i < indices.Length; i++)
        {
            SpawnEnemy(indices[i]);
        }
    }

    // Gets an enemy prefab from the list using the index
    public Enemy GetEnemyFromIndex(EnemyManager.EnemyIndex index)
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

    public XP GetXPFromIndex(EnemyManager.XPIndex index)
    {
        foreach (XP i in xpPrefabs)
        {
            if (i.index == index)
            {
                return i;
            }
        }
        return null;
    }

    /// <summary>
    /// Finds which phase it should be on
    /// </summary>
    private void FindCurrentPhase()
    {
        for (int i = 0; i < phases.Count; i++)
        {
            if (phases[i].startTime <= GameManager.instance.GameTime)
            {
                currentPhase = phases[i];
                phases.RemoveAt(i);
                break;
            }
        }
    }
}
