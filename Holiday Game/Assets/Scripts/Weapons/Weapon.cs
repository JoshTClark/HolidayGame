using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    private float delay;

    [SerializeField]
    protected ProjectileBase projectile;

    public ResourceManager.WeaponIndex index;

    protected float timer = 0.0f;

    public StatsComponent owner;

    public bool canFire = false;

    [SerializeField]
    public float baseDamageMultiplier, baseSizeMultiplier;

    public float Delay
    {
        get { return delay * (1 / owner.AttackSpeed); }
    }

    void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.Normal)
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

    public void ResetTimer()
    {
        timer = 0.0f;
        canFire = false;
    }

    public abstract void OnUpdate();
}
