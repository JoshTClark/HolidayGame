using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float timeAlive = 0.0f;

    [SerializeField]
    private float speed, damage, lifetime;

    [SerializeField]
    private int team;


    void Update()
    {
        float delta = Time.deltaTime;

        // Timer for the projectile expiring
        timeAlive += delta;
        if (timeAlive >= lifetime)
        {
            Destroy(gameObject);
        }

        // Moving the projectile
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Collision detection for bullets
        Debug.Log("test");
        if (team == 0)
        {
            // Player bullet so only collide with enemies
            if (other.gameObject.GetComponent<Enemy>())
            {
                other.gameObject.GetComponent<Enemy>().DealDamage(damage);
                Destroy(gameObject);
            }
        }
        else if (team == 1)
        {
            // Enemy bullet so only collide with player
            if (other.gameObject.GetComponent<Player>())
            {
                other.gameObject.GetComponent<Player>().DealDamage(damage);
                Destroy(gameObject);
            }
        }

        // Destroys the projectile if it hits a wall
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) 
        {
            Destroy(gameObject);
        }
    }
}
