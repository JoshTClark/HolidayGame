using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    protected float delay;

    [SerializeField]
    protected ProjectileBase projectile;

    public GameManager.WeaponIndex index;

    protected float timer = 0.0f;

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
}
