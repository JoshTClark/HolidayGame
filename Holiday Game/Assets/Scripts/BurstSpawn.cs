using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Burst Spawn Config", menuName = "ScriptableObject/Burst Spawn Config")]
public class BurstSpawn : ScriptableObject
{
    public float startTime;
    public List<SpawnData> enemies;

    [System.Serializable]
    public class SpawnData
    {
        public ResourceManager.EnemyIndex index;
        public int count;
    }
}
