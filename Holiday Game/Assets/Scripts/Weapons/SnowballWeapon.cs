using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballWeapon : Weapon
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
}
