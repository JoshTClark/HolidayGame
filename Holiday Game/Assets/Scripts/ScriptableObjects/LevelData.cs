using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Level Data", menuName = Constants.ASSET_MENU_PATH + "Level Data")]
public class LevelData : ScriptableObject
{
    public string sceneName = "TestScene";
    public List<Wave> waves = new List<Wave>();

    public Wave GetWaveByTime(float time)
    {
        float timesPast = 0.0f;
        for (int i = 0; i < waves.Count; i++)
        {
            timesPast += waves[i].waveLength;
            if (timesPast > time)
            {
                return waves[i];
            }
        }
        return waves[waves.Count - 1];
    }

    [System.Serializable]
    public class Wave
    {
        public float waveLength;
        public List<SpawnInfo> enemies = new List<SpawnInfo>();
    }

    [System.Serializable]
    public class SpawnInfo
    {
        public ResourceManager.EnemyIndex enemyIndex;
        public int amountToSpawn;
        public bool respawn;
        public float healthMultiplier;
        public float damageMultiplier;
        public float speedMultiplier;
    }
}

