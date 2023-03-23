using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    private float delay;

    public WeaponDef weaponDef;
    public int level;
    public ResourceManager.WeaponIndex index;

    public Sprite icon;

    public float timer = 0.0f;

    public StatsComponent owner;

    public bool canFire = false;

    [SerializeField]
    private float baseDamageMultiplier = 1, baseSizeMultiplier = 1, attackSpeedMultiplier = 1, pierce = 0;

    [SerializeField]
    private List<WeaponStat> extraStats = new List<WeaponStat>();

    protected Dictionary<string, float> baseStats = new Dictionary<string, float>();

    protected Dictionary<string, float> trueStats = new Dictionary<string, float>();


    // Audio
    [SerializeField]
    protected List<AudioClip> soundEffects;


    public float Delay
    {
        get { return (delay * (1 / owner.AttackSpeed)) / GetStat("Attack Speed"); }
    }

    private void Start()
    {
        attackSpeedMultiplier = 1f;

        baseStats.Add("Damage", baseDamageMultiplier);
        baseStats.Add("Attack Speed", attackSpeedMultiplier);
        baseStats.Add("Size", baseSizeMultiplier);
        baseStats.Add("Pierce", pierce);

        foreach (WeaponStat stat in extraStats)
        {
            string key = stat.statName;
            float val = stat.value;
            baseStats.Add(key, val);
        }

        foreach (string key in baseStats.Keys)
        {
            float val;
            baseStats.TryGetValue(key, out val);
            trueStats.Add(key, val);
        }
    }

    void Update()
    {
        CalcStats();
        if (GameManager.instance.State == GameManager.GameState.MainGame)
        {
            float delta = Time.deltaTime;
            timer += delta;
            if (timer >= Delay)
            {
                canFire = true;
            }
            OnUpdate();
        }
    }

    /// <summary>
    /// Finds the closest enemy
    /// </summary>
    /// <returns>The closest enemy to the player or null if none are within the player's range</returns>
    protected Enemy GetClosestEnemy()
    {
        List<Enemy> enemies = EnemyManager.instance.AllEnemies;
        if (enemies.Count > 0)
        {
            Enemy closest = null;
            float distance = GameManager.instance.Player.attackActivationRange;
            foreach (Enemy e in enemies)
            {
                float newDistance = e.PlayerDistance();
                if (newDistance < distance)
                {
                    closest = e;
                    distance = newDistance;
                }
            }
            return closest;
        }
        else
        {
            return null;
        }
    }

    protected List<Enemy> GetClosestEnemies(int num)
    {
        List<Enemy> enemies = EnemyManager.instance.AllEnemies;
        List<Enemy> targetList = new List<Enemy>();

        float closestDist = enemies[0].PlayerDistance();
        if (enemies.Count > 0)
        {
            float distance = GameManager.instance.Player.attackActivationRange;
            Enemy closest = null;
            foreach (Enemy e in enemies)
            {
                float newDistance = e.PlayerDistance();
                if (newDistance < distance)
                {
                    closest = e;
                    distance = newDistance;
                    closestDist = newDistance;
                }
            }
            targetList.Add(closest);
            if (enemies.Count < num)
            {
                num = enemies.Count;
            }
            for (int i = 1; i < num; i++)
            {
                distance = GameManager.instance.Player.attackActivationRange;
                foreach (Enemy e in enemies)
                {
                    float newDistance = e.PlayerDistance();
                    if (newDistance < distance && newDistance > closestDist)
                    {
                        closest = e;
                        distance = newDistance;
                    }
                }
                targetList.Add(closest);
                closestDist = targetList[i].PlayerDistance();
            }
            return targetList;
        }
        else
        {
            return null;
        }
    }

    protected Enemy GetRandomEnemy()
    {
        List<Enemy> enemies = EnemyManager.instance.AllEnemies;
        if (enemies.Count > 0)
        {
            int rngEnemy = Random.Range(0, EnemyManager.instance.AllEnemies.Count - 1);
            Enemy closest = enemies[rngEnemy];
            return closest;
        }
        else
        {
            return null;
        }
    }

    protected Enemy GetRandomEnemyInRange(float range)
    {
        List<Enemy> enemies = EnemyManager.instance.AllEnemies;
        List<Enemy> filtered = new List<Enemy>();
        if (enemies.Count > 0)
        {
            Vector2 pos = owner.gameObject.transform.position;
            foreach (Enemy e in enemies)
            {
                if (Vector2.Distance(pos, e.transform.position) <= range)
                {
                    filtered.Add(e);
                }
            }
            int rngEnemy = Random.Range(0, filtered.Count - 1);
            Enemy enemy = enemies[rngEnemy];
            return enemy;
        }
        else
        {
            return null;
        }
    }

    public void ResetTimer()
    {
        timer = 0.0f;
        canFire = false;
    }

    public float PercentTimeLeft()
    {
        return timer / Delay;
    }

    public Vector3 MousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0.0f;
        return mousePosition;
    }

    public float GetStat(string statName)
    {
        float val;
        if (trueStats.TryGetValue(statName, out val))
        {
            return val;
        }
        throw new System.Exception("Error in weapon " + this.name + ": cannot find stat " + statName);
    }

    public float GetBaseStat(string statName)
    {
        float val;
        if (baseStats.TryGetValue(statName, out val))
        {
            return val;
        }
        throw new System.Exception("Error in weapon " + this.name + ": cannot find stat " + statName);
    }

    public string[] GetAllStats()
    {
        string[] ar = new string[trueStats.Keys.Count];
        trueStats.Keys.CopyTo(ar, 0);
        return ar;
    }

    public abstract void OnUpdate();

    public abstract void CalcStats();

    [System.Serializable]
    private class WeaponStat
    {
        public string statName;
        public float value;
    }
}
