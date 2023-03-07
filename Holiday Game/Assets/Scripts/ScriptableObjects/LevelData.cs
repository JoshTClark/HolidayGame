using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Level Data", menuName = Constants.ASSET_MENU_PATH + "Level Data")]
public class LevelData : ScriptableObject
{
    public string scene;
    public int difficulty = 1;
    public int startHour = 12;
    public int enemiesToDefeat = 0;
    public int daysToSurvive = 0;
    public float levelHealthMultiplier = 1f;
    public float levelDamageMultiplier = 1f;
    public float levelSpeedMultiplier = 1f;
    public bool isBossLevel = false;
    public List<Wave> waves = new List<Wave>();


    public Wave GetWaveByTime(float time)
    {

        if (waves.Count > 0)
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
        return null;
    }

    public void Init()
    {
        foreach (Wave w in waves)
        {
            foreach (SpawnInfo i in w.enemies) 
            {
                i.levelDamageMultiplier = levelDamageMultiplier;
                i.levelSpeedMultiplier = levelSpeedMultiplier;
                i.levelHealthMultiplier = levelHealthMultiplier;
            }
        }
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
        public bool isBoss = false;
        public ResourceManager.EnemyIndex enemyIndex;
        public int amountToSpawn;
        public bool respawn;
        public float healthMultiplier = 1f;
        public float damageMultiplier = 1f;
        public float speedMultiplier = 1f;

        [HideInInspector]
        public float levelHealthMultiplier = 1f, levelDamageMultiplier = 1f, levelSpeedMultiplier = 1f;
    }
}

