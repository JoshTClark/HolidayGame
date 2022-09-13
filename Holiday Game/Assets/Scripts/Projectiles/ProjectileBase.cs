using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    private float timeAlive = 0.0f;
    private float usedPierce = 0f;
    private List<Enemy> hitEnemies = new List<Enemy>();

    [SerializeField]
    private float baseSpeed, baseDamage, baseLifetime, basePierce;

    [SerializeField]
    private Team projectileTeam;

    public float Speed { get { return baseSpeed; } }
    public float Damage { get { return baseDamage; } }
    public float Lifetime { get { return baseLifetime; } }
    public float Pierce { get { return basePierce; } }


    // Decides what the projectile can damage
    public enum Team
    {
        Player,
        Enemy,
        None
    }


    private void Update()
    {
        float delta = Time.deltaTime;

        // Timer for the projectile expiring
        timeAlive += delta;
        if (timeAlive >= Lifetime)
        {
            DestroyProjectile();
        }

        // Moving the projectile
        Move();
    }

    // Called when a projectile collides with anything
    private void OnTriggerStay2D(Collider2D other)
    {
        HandleCollision(other);
    }

    // Called when the projectile should be destroyed
    private void DestroyProjectile()
    {
        OnDeath();
        Destroy(gameObject);
    }

    // Handles the projectiles collision
    private void HandleCollision(Collider2D other)
    {
        OnCollision();

        // Basic collision detection for bullets
        if (projectileTeam == Team.Player)
        {
            // Player bullet so only collide with enemies
            if (other.gameObject.GetComponent<Enemy>())
            {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                if (!hitEnemies.Contains(enemy))
                {
                    enemy.DealDamage(Damage);
                    hitEnemies.Add(enemy);

                    if (Pierce > 0 && usedPierce < Pierce)
                    {
                        usedPierce++;
                    }
                    else
                    {
                        DestroyProjectile();
                    }
                }
            }
        }
        else if (projectileTeam == Team.Enemy)
        {
            // Enemy bullet so only collide with player
            if (other.gameObject.GetComponent<Player>())
            {
                other.gameObject.GetComponent<Player>().DealDamage(Damage);
                DestroyProjectile();
            }
        }

        // Destroys the projectile if it hits a wall
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            DestroyProjectile();
        }
    }

    // Called when the projectile hits something
    private void Hit()
    {

    }

    /// <summary>
    /// How the projectile moves goes here
    /// </summary>
    public abstract void Move();

    /// <summary>
    /// Any special behavior for the projectile can go here
    /// </summary>
    public abstract void OnUpdate();

    /// <summary>
    /// Any special behavior for when the projectile dies goes here
    /// </summary>
    public abstract void OnDeath();

    /// <summary>
    /// Any special behavior for when the projectile hits something with health goes here
    /// </summary>
    public abstract void OnHit();

    /// <summary>
    /// Any special behavior for when the projectile collides with something goes here
    /// </summary>
    public abstract void OnCollision();
}
