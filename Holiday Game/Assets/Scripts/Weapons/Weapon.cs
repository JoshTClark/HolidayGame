using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    private float delay;

    public ResourceManager.WeaponIndex index;

    public Sprite icon;

    public float timer = 0.0f;

    public StatsComponent owner;

    public bool canFire = false;

    [SerializeField]
    public float baseDamageMultiplier, baseSizeMultiplier;

    public float Delay
    {
        get { return delay * (1 / owner.AttackSpeed); }
    }

    private void Start()
    {
    }

    void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.Normal)
        {
            if (!owner.HasBuff(ResourceManager.BuffIndex.Stunned))
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
    }

    /// <summary>
    /// Finds the closest enemy
    /// </summary>
    /// <returns>The closest enemy to the player or null if none are within the player's range</returns>
    protected Enemy GetClosestEnemy()
    {
        List<Enemy> enemies = EnemyManager.instance.CurrentEnemies;
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
        List<Enemy> enemies = EnemyManager.instance.CurrentEnemies;
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
        List<Enemy> enemies = EnemyManager.instance.CurrentEnemies;
        if (enemies.Count > 0)
        {
            int rngEnemy = Random.Range(0, EnemyManager.instance.CurrentEnemies.Count - 1);
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
        List<Enemy> enemies = EnemyManager.instance.CurrentEnemies;
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

    public abstract void OnUpdate();
}
