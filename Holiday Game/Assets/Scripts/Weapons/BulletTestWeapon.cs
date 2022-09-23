using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTestWeapon : Weapon
{
    void Update()
    {
        float delta = Time.deltaTime;

        Player player = GameManager.instance.Player;
        Vector2 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Attack timer it will do the attack every "delay" seconds
        timer += delta;
        if (timer >= delay)
        {
            ProjectileBase p1 = Instantiate<ProjectileBase>(projectile, transform.position, Quaternion.identity);
            p1.Direction = transform.right;
            p1.RotateDirection(-30);

            ProjectileBase p2 = Instantiate<ProjectileBase>(projectile, transform.position, Quaternion.identity);
            p2.Direction = transform.right;
            p2.RotateDirection(-15);

            ProjectileBase p3 = Instantiate<ProjectileBase>(projectile, transform.position, Quaternion.identity);
            p3.Direction = transform.right;
            p3.RotateDirection(15);

            ProjectileBase p4 = Instantiate<ProjectileBase>(projectile, transform.position, Quaternion.identity);
            p4.Direction = transform.right;
            p4.RotateDirection(30);

            ProjectileBase p5 = Instantiate<ProjectileBase>(projectile, transform.position, Quaternion.identity);
            p5.Direction = transform.right;
            p5.RotateDirection(0);

            timer = 0.0f;
        }
    }
}
