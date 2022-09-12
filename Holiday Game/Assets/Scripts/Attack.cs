using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private float timer;

    [SerializeField]
    private float delay;

    [SerializeField]
    private Projectile projectile;

    void Update()
    {
        float delta = Time.deltaTime;

        timer += delta;

        Enemy e = GetClosestEnemy();
        Vector2 direction = e.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (timer >= delay)
        {
            Instantiate<Projectile>(projectile, transform.position, transform.rotation);
            timer = 0f;
        }
    }

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
