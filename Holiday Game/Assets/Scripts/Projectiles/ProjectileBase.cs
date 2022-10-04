using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [SerializeField]
    private float baseSpeed, baseDamage, baseLifetime, basePierce;

    [SerializeField]
    private Team projectileTeam;

    [SerializeField]
    private float timeAlive = 0.0f;
    private float usedPierce = 0f;
    private List<StatsComponent> hitTargets = new List<StatsComponent>();
    private Vector2 direction = new Vector2();

    public Vector2 Direction { get { return direction; } set { direction = value; } }
    public float Speed { get { return baseSpeed; } }
    public float Damage { get { return baseDamage; } }
    public float Lifetime { get { return baseLifetime; } }
    public float Pierce { get { return basePierce; } }
    public float TimeAlive { get { return timeAlive; } }

    [SerializeField]
    public StatsComponent shooter;


    // Decides what the projectile can damage
    public enum Team
    {
        Player,
        Enemy,
        None
    }

    private void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.Normal)
        {
            float delta = Time.deltaTime;

            // Timer for the projectile expiring
            timeAlive += delta;
            if (timeAlive >= Lifetime)
            {
                DestroyProjectile();
            }

            // Update stuff
            OnUpdate();

            // Moving the projectile
            Move();
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2();
        }
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
        // Basic collision detection for bullets
        if (projectileTeam == Team.Player)
        {
            // Player bullet so only collide with enemies
            if (other.gameObject.GetComponent<Enemy>())
            {
                StatsComponent receiver = other.gameObject.GetComponent<StatsComponent>();
                if (!hitTargets.Contains(receiver))
                {
                    Hit(receiver);
                    hitTargets.Add(receiver);

                    if (Pierce > 0 && usedPierce < Pierce)
                    {
                        usedPierce++;
                    }
                    else
                    {
                        DestroyProjectile();
                    }
                    OnCollision();
                }
            }
        }
        else if (projectileTeam == Team.Enemy)
        {
            // Enemy bullet so only collide with player
            if (other.gameObject.GetComponent<Player>())
            {
                StatsComponent receiver = other.gameObject.GetComponent<StatsComponent>();
                Hit(receiver);
                DestroyProjectile();
                OnCollision();
            }
        }

        // Destroys the projectile if it hits a wall
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            DestroyProjectile();
            OnCollision();
        }
    }

    // Called when the projectile hits something
    private void Hit(StatsComponent receiver)
    {
        OnHit(receiver);
        receiver.DealDamage(Damage + GameManager.instance.Player.Damage);
    }

    // Rotates the direction the projectile is moving by a certain degree
    public void RotateDirection(float degrees)
    {
        direction = Quaternion.Euler(0f, 0f, degrees) * direction;
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
    public abstract void OnHit(StatsComponent receiver);

    /// <summary>
    /// Any special behavior for when the projectile collides with something goes here
    /// </summary>
    public abstract void OnCollision();
}
