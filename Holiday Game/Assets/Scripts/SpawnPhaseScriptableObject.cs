using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Phase Config", menuName = "ScriptableObject/Spawn Phase Config")]
public class SpawnPhaseScriptableObject : ScriptableObject
{
    public int numberOfEnemiesPerSpawn;
    public float spawnInterval;
    public float startTime;
    public List<SpawnConfig> enemies;
    private float[] weights;

    public float GetWeight()
    {
        return 0;
    }

    /// <summary>
    /// Finds a random enemy index from the phase based on how likely it is to spawn
    /// </summary>
    /// <returns></returns>
    public ResourceManager.EnemyIndex GetRandomEnemy()
    {
        float value = Random.value;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (value < weights[i])
            {
                return enemies[i].index;
            }
            value -= weights[i];
        }

        return ResourceManager.EnemyIndex.None;
    }

    /// <summary>
    /// Returns an array of enemy indices
    /// </summary>
    /// <returns></returns>
    public ResourceManager.EnemyIndex[] GetSpawnWave()
    {
        ResetSpawnWeights();
        ResourceManager.EnemyIndex[] indices = new ResourceManager.EnemyIndex[numberOfEnemiesPerSpawn];

        for (int i = 0; i < numberOfEnemiesPerSpawn; i++)
        {
            indices[i] = GetRandomEnemy();
        }

        return indices;
    }

    // Normalizes the spawn chances of enemies
    private void ResetSpawnWeights()
    {
        float totalWeight = 0;
        weights = new float[enemies.Count];

        for (int i = 0; i < enemies.Count; i++)
        {
            weights[i] = enemies[i].GetWeight();
            totalWeight += weights[i];
        }

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = weights[i] / totalWeight;
        }
    }
}

[System.Serializable]
public class SpawnConfig
{
    public ResourceManager.EnemyIndex index;

    [Range(0, 1)]
    public float weight;

    public float GetWeight()
    {
        return weight;
    }
}
