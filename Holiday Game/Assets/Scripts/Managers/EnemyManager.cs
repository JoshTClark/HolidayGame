using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Vector2 minSpawnDistance, maxSpawnDistance;

    private List<Enemy> enemyPrefabs;
    private List<SpawnPhase> phases;
    private List<DropBase> pickupPrefabs;

    private float spawnTimer = 0;

    private SpawnPhase currentPhase;

    private List<Enemy> currentEnemies = new List<Enemy>();

    private List<BurstSpawn> spawnedBursts = new List<BurstSpawn>();

    public static EnemyManager instance;

    public List<Enemy> CurrentEnemies
    {
        get { return currentEnemies; }
    }

    public void Start()
    {
        instance = this;
        enemyPrefabs = ResourceManager.enemyPrefabs;
        phases = ResourceManager.phaseDefinitions;
        pickupPrefabs = ResourceManager.pickupPrefabs;
        FindCurrentPhase();
    }

    private void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.Normal)
        {
            float delta = Time.deltaTime;
            spawnTimer += delta;
            if (spawnTimer >= currentPhase.spawnInterval)
            {
                SpawnEnemiesByPhase();
                spawnTimer = 0;
            }

            CheckBurstSpawns();

            RemoveDeadEnemies();

            FindCurrentPhase();
        }
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
    public void SpawnEnemy(ResourceManager.EnemyIndex index)
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
        ResourceManager.EnemyIndex[] indices = currentPhase.GetSpawnWave();
        for (int i = 0; i < indices.Length; i++)
        {
            SpawnEnemy(indices[i]);
        }
    }

    // Gets an enemy prefab from the list using the index
    public Enemy GetEnemyFromIndex(ResourceManager.EnemyIndex index)
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

    public DropBase GetDropFromIndex(ResourceManager.PickupIndex index)
    {
        foreach (DropBase i in pickupPrefabs)
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
        SpawnPhase phase = phases[0];
        for (int i = 0; i < phases.Count; i++)
        {
            if (phases[i].startTime <= GameManager.instance.GameTime && phases[i].startTime >= phase.startTime)
            {
                phase = phases[i];
            }
        }
        currentPhase = phase;
    }

    /// <summary>
    /// Removes all enemies
    /// </summary>
    public void Reset()
    {
        for (int i = currentEnemies.Count - 1; i >= 0; i--)
        {
            Destroy(currentEnemies[i].gameObject);
        }
        currentEnemies.Clear();
        phases = ResourceManager.phaseDefinitions;
        spawnedBursts.Clear();
    }

    public void CheckBurstSpawns()
    {
        foreach (BurstSpawn i in ResourceManager.spawnDefinitions)
        {
            if (GameManager.instance.GameTime >= i.startTime && !spawnedBursts.Contains(i))
            {
                spawnedBursts.Add(i);
                foreach (BurstSpawn.SpawnData s in i.enemies)
                {
                    for (int num = 0; num < s.count; num++)
                    {
                        SpawnEnemy(s.index);
                    }
                }
            }
        }
    }
}
