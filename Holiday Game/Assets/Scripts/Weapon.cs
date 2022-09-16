using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float timer = 0.0f;

    [SerializeField]
    private float delay;

    [SerializeField]
    private ProjectileBase projectile;

    void Update()
    {
        float delta = Time.deltaTime;

        // Basic targetting for now just targets the closest enemy
        Enemy e = GetClosestEnemy();
        Vector2 direction = e.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Attack timer it will do the attack every "delay" seconds
        timer += delta;
        if (timer >= delay)
        {
            Instantiate<ProjectileBase>(projectile, transform.position, transform.rotation);
            timer = 0.0f;
        }
    }

    // Gets the closest enemy
    private Enemy GetClosestEnemy()
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
