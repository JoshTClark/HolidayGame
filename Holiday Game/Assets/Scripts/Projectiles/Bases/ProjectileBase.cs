using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [SerializeField]
    private float baseSpeed, baseLifetime, basePierce, baseSize;

    private float sizeMultiplier = 1f;
    private float speedMultiplier = 1f;
    private float damageMultiplier = 1f;

    [SerializeField]
    public Team projectileTeam;

    private float timeAlive = 0.0f;
    private float usedPierce = 0f;
    private List<StatsComponent> hitTargets = new List<StatsComponent>();
    private Vector2 direction = new Vector2();

    public Vector2 Direction { get { return direction; } set { direction = value; } }
    public float Speed { get { return baseSpeed * speedMultiplier; } set { baseSpeed = value; } }
    public float Lifetime { get { return baseLifetime; } set { baseLifetime = value; } }
    public float Pierce { get { return basePierce; } set { basePierce = value; } }
    public float TimeAlive { get { return timeAlive; } set { timeAlive = value; } }
    public float DamageMultiplier { get { return damageMultiplier; } set { damageMultiplier = value; } }
    public float Size { get { return baseSize * sizeMultiplier; } set { baseSize = value; } }
    public float SizeMultiplier { get { return sizeMultiplier; } set { sizeMultiplier = value; } }
    public float SpeedMultiplier { get { return speedMultiplier; } set { speedMultiplier = value; } }

    protected DamageInfo damageInfo;


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

            this.gameObject.transform.localScale = new Vector3(Size, Size);

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
        Destroy(this.gameObject);
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
                    if (this.GetType() == typeof(BombProjectileBase))
                    {
                        if (((BombProjectileBase)this).explodeOnContact)
                        {
                            DestroyProjectile();
                        }
                    }
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
    protected void Hit(StatsComponent receiver)
    {
        OnHit(receiver);
        damageInfo.damage *= damageMultiplier;
        receiver.DealDamage(damageInfo);
    }

    // Rotates the direction the projectile is moving by a certain degree
    public void RotateDirection(float degrees)
    {
        direction = Quaternion.Euler(0f, 0f, degrees) * direction;
    }

    public void SetDamageInfo(DamageInfo info)
    {
        damageInfo = info;
    }

    public static GameObject CreateEmptyProjectile()
    {
        GameObject projectile = new GameObject("Explosion");

        CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
        collider.radius = 1f;
        collider.isTrigger = true;

        Rigidbody2D body = projectile.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;

        projectile.AddComponent<EmptyProjectile>();

        return projectile;
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
