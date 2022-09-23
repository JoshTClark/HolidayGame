using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public enum EnemyIndex
    {
        None,
        Test,
        Test2,
        End
    }

    public enum XPIndex
    {
        XP1
    }

    [SerializeField]
    private List<Enemy> enemyPrefabs = new List<Enemy>();

    [SerializeField]
    private List<XP> xpPrefabs = new List<XP>();

    [SerializeField]
    private Vector2 minSpawnDistance, maxSpawnDistance;

    [SerializeField]
    private float spawnInterval;

    private float spawnTimer = 0;

    private List<Enemy> currentEnemies = new List<Enemy>();

    public static EnemyManager instance;

    public List<Enemy> CurrentEnemies
    {
        get { return currentEnemies; }
    }

    public void Start()
    {
        instance = this;
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0;
        }

        RemoveDeadEnemies();
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
        Vector2 playerPos = GameManager.instance.player.transform.position;
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
        spawned.player = GameManager.instance.player;
        spawned.SetLevel((int)GameManager.instance.CurrentDifficulty);
        currentEnemies.Add(spawned);
    }

    // Spawns a random enemy
    public void SpawnEnemy()
    {
        int index = (int)Random.Range(1, (int)EnemyIndex.End);
        SpawnEnemy((EnemyIndex)index);
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
}
