using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected float timer = 0.0f;

    [SerializeField]
    protected float delay;

    [SerializeField]
    protected ProjectileBase projectile;

    // Gets the closest enemy
    protected Enemy GetClosestEnemy()
    {
        List<Enemy> enemies = GameManager.currentEnemies;
        if (enemies.Count > 0)
        {
            Enemy closest = enemies[0];
            float distance = 0f;
            foreach (Enemy e in enemies) 
            {
                float newDistance = Vector2.Distance(transform.position, e.transform.position);
                if (newDistance > distance) 
                {
                    closest = e;
                    distance = newDistance;
                }
            }
            return closest;
        }
        else {
            return null;
        }
    }
}
