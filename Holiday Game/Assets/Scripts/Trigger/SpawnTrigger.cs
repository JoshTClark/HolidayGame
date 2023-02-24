using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : CollisionTriggerBase
{
    [SerializeField]
    private List<EnemySpawn> spawns = new List<EnemySpawn>();

    public override void OnTrigger()
    {
        foreach (EnemySpawn i in spawns) 
        {
            EnemyManager.instance.SpawnEnemy(i.enemy, i.count);
        }
    }

    /// <summary>
    /// Holds data about enemy spawns
    /// </summary>
    [System.Serializable]
    public class EnemySpawn 
    {
        public ResourceManager.EnemyIndex enemy;
        public int count;
    }
}
