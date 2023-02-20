using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static ResourceManager;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Vector2 minSpawnDistance, maxSpawnDistance;

    private List<Enemy> enemyPrefabs;
    private List<DropBase> pickupPrefabs;

    private static List<Enemy> allEnemies = new List<Enemy>();

    private static List<Enemy> currentWaveEnemies = new List<Enemy>();

    public static Dictionary<EnemyIndex, ObjectPool<Enemy>> pools = new Dictionary<EnemyIndex, ObjectPool<Enemy>>();

    public static EnemyManager instance;

    private LevelData.Wave previousWave;

    public GameObject deathEffect;

    public Enemy boss;

    public List<Enemy> AllEnemies
    {
        get { return allEnemies; }
    }

    public void Start()
    {
        instance = this;
        enemyPrefabs = ResourceManager.enemyPrefabs;
        pickupPrefabs = ResourceManager.pickupPrefabs;
    }

    private void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.Normal)
        {
            float delta = Time.deltaTime;

            RemoveDeadEnemies();

            LevelData.Wave wave = GameManager.instance.levelData.GetWaveByTime(GameManager.instance.GameTime);
            if (wave != null)
            {
                if (previousWave == null || wave != previousWave)
                {
                    currentWaveEnemies.Clear();
                    foreach (LevelData.SpawnInfo info in wave.enemies)
                    {
                        if (!info.respawn)
                        {
                            for (int i = 0; i < info.amountToSpawn; i++)
                            {
                                SpawnEnemy(info);
                            }
                        }
                    }
                    previousWave = wave;
                }

                foreach (LevelData.SpawnInfo info in wave.enemies)
                {
                    if (info.respawn)
                    {
                        int amount = 0;
                        foreach (Enemy e in currentWaveEnemies)
                        {
                            if (e.index == info.enemyIndex)
                            {
                                amount++;
                            }
                        }

                        for (int i = amount; i < info.amountToSpawn; i++)
                        {
                            SpawnEnemy(info);
                        }
                    }
                }
            }

            Vector2 playerPos = GameManager.instance.player.transform.position;
            foreach (Enemy e in allEnemies)
            {
                Vector2 enemyPos = e.gameObject.transform.position;
                if (enemyPos.x > playerPos.x + maxSpawnDistance.x || enemyPos.x < playerPos.x - maxSpawnDistance.x || enemyPos.y > playerPos.y + maxSpawnDistance.y || enemyPos.y < playerPos.y - maxSpawnDistance.y)
                {
                    Vector2 spawnPos = GetRandomPosition();
                    e.transform.position = spawnPos;
                }
            }
        }
    }

    // Removes dead enemies from the game
    public void RemoveDeadEnemies()
    {
        for (int i = allEnemies.Count - 1; i >= 0; i--)
        {
            Enemy e = allEnemies[i];
            if (e.IsDead)
            {
                allEnemies.RemoveAt(i);
                if (e.pool != null)
                {
                    e.pool.Release(e);
                }
            }
        }

        for (int i = currentWaveEnemies.Count - 1; i >= 0; i--)
        {
            Enemy e = currentWaveEnemies[i];
            if (e.IsDead)
            {
                currentWaveEnemies.RemoveAt(i);
            }
        }
    }

    // Spawns enemies
    public void SpawnEnemy(ResourceManager.EnemyIndex index)
    {
        Enemy enemy = GetEnemy(index);
        Vector2 spawnPos = GetRandomPosition();
        enemy.gameObject.transform.position = spawnPos;
        enemy.damageMultConst = 1f;
        enemy.hpMultConst = 1f;
        enemy.speedMultConst = 1f;
        enemy.player = GameManager.instance.Player;
        allEnemies.Add(enemy);
    }

    public Enemy SpawnEnemy(ResourceManager.EnemyIndex index, Vector2 pos)
    {
        Enemy enemy = GetEnemy(index);
        enemy.gameObject.transform.position = pos;
        enemy.player = GameManager.instance.Player;
        enemy.damageMultConst = 1f;
        enemy.hpMultConst = 1f;
        enemy.speedMultConst = 1f;
        allEnemies.Add(enemy);
        return enemy;
    }

    public void SpawnEnemy(LevelData.SpawnInfo info)
    {
        Enemy enemy = GetEnemy(info.enemyIndex);
        enemy.gameObject.transform.position = GetRandomPosition();
        enemy.player = GameManager.instance.Player;
        enemy.damageMultConst = info.damageMultiplier;
        enemy.hpMultConst = info.healthMultiplier;
        enemy.speedMultConst = info.speedMultiplier;

        allEnemies.Add(enemy);
        currentWaveEnemies.Add(enemy);
    }

    private Vector2 GetRandomPosition()
    {
        Vector2 pos = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        pos.x = pos.x * Random.Range(minSpawnDistance.x, maxSpawnDistance.x) + GameManager.instance.player.transform.position.x;
        pos.y = pos.y * Random.Range(minSpawnDistance.y, maxSpawnDistance.y) + GameManager.instance.player.transform.position.y;

        return pos;
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

    private static void CreateNewPool(EnemyIndex index)
    {
        ObjectPool<Enemy> pool = new ObjectPool<Enemy>(createFunc: () => GameObject.Instantiate<Enemy>(ResourceManager.GetEnemyFromIndex(index)), actionOnGet: (obj) => obj.Clean(GetPool(index)), actionOnRelease: (obj) => obj.gameObject.SetActive(false), actionOnDestroy: (obj) => Destroy(obj.gameObject), collectionCheck: false, defaultCapacity: 50);
        pools.Add(index, pool);
    }

    public static ObjectPool<Enemy> GetPool(EnemyIndex index)
    {
        ObjectPool<Enemy> pool;
        pools.TryGetValue(index, out pool);
        return pool;
    }

    public static Enemy GetEnemy(EnemyIndex index)
    {
        ObjectPool<Enemy> pool;
        if (pools.TryGetValue(index, out pool))
        {
            Enemy p = pool.Get();
            return p;
        }
        else
        {
            CreateNewPool(index);
            pools.TryGetValue(index, out pool);
            Enemy p = pool.Get();
            return p;
        }
    }

    public static void Clean()
    {
        foreach (Enemy p in allEnemies)
        {
            p.pool.Release(p);
        }
        allEnemies.Clear();
        pools.Clear();
    }

    public void KillAllAndStopSpawns()
    {
        foreach (Enemy e in allEnemies)
        {
            DamageInfo info = new DamageInfo();
            info.damage = e.MaxHp * 10;
            e.TakeDamage(info);
        }
    }

    /// <summary>
    /// Draws spawn distances from player in yellow (min) and red (max) wire cubes
    /// </summary>
    void OnDrawGizmos()
    {
        if (GameManager.instance && GameManager.instance.player)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(GameManager.instance.Player.transform.position, new Vector3(minSpawnDistance.x*2, minSpawnDistance.y * 2, 1));
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(GameManager.instance.Player.transform.position, new Vector3(maxSpawnDistance.x * 2, maxSpawnDistance.y * 2, 1));
        }
    }
}
