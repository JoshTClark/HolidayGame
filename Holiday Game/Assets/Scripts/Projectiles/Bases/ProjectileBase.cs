using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class ProjectileBase : MonoBehaviour
{
    public ResourceManager.ProjectileIndex index;

    [SerializeField]
    private float baseSpeed, baseLifetime, baseSize;

    private float sizeMultiplier = 1f;
    private float speedMultiplier = 1f;
    private float damageMultiplier = 1f;
    private float knockbackMultiplier = 1f;
    private float lifetimeMultiplier = 1f;
    private float pierce = 0f;

    [SerializeField]
    public Team projectileTeam;

    private float timeAlive = 0.0f;
    private float usedPierce = 0f;
    private Vector2 direction = new Vector2();
    protected List<StatsComponent> hitTargets = new List<StatsComponent>();

    public Vector2 Direction { get { return direction; } set { direction = value; } }
    public float Speed { get { return baseSpeed * speedMultiplier; } set { baseSpeed = value; } }
    public float Lifetime { get { return baseLifetime * LifetimeMultiplier; } set { baseLifetime = value; } }
    public float Pierce { get { return pierce; } set { pierce = value; } }
    public float TimeAlive { get { return timeAlive; } set { timeAlive = value; } }
    public float DamageMultiplier { get { return damageMultiplier; } set { damageMultiplier = value; } }
    public float Size { get { return baseSize * sizeMultiplier; } set { baseSize = value; } }
    public float SizeMultiplier { get { return sizeMultiplier; } set { sizeMultiplier = value; } }
    public float SpeedMultiplier { get { return speedMultiplier; } set { speedMultiplier = value; } }
    public float LifetimeMultiplier { get { return lifetimeMultiplier; } set { lifetimeMultiplier = value; } }

    protected DamageInfo damageInfo;

    public ObjectPool<ProjectileBase> pool;


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
        pool.Release(this);
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
                OnCollision(other);
                if (!hitTargets.Contains(receiver))
                {
                    if (this.GetType() == typeof(BombProjectileBase))
                    {
                        if (((BombProjectileBase)this).explodeOnContact)
                        {
                            DestroyProjectile();
                        }
                    }
                    this.damageInfo.damagePos = this.gameObject.transform.position;
                    this.damageInfo.receiver = receiver;
                    this.damageInfo.knockback *= knockbackMultiplier;
                    hitTargets.Add(receiver);
                    Hit(receiver);
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
                StatsComponent receiver = other.gameObject.GetComponent<StatsComponent>();
                this.damageInfo.receiver = receiver;
                this.damageInfo.knockback *= knockbackMultiplier;
                Hit(receiver);
                DestroyProjectile();
                OnCollision(other);
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Orbital"))
        {
            if (projectileTeam == Team.Enemy)
            {
                if (other.gameObject.GetComponent<IceShield>())
                {
                    other.gameObject.GetComponent<IceShield>().Shoot(damageInfo);
                    DestroyProjectile();
                }
            }
        }

        // Destroys the projectile if it hits a wall
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            DestroyProjectile();
            OnCollision(other);
        }
    }

    // Called when the projectile hits something
    protected void Hit(StatsComponent receiver)
    {
        OnHit(receiver);
        DamageInfo passedInfo = damageInfo.CreateCopy();
        passedInfo.damage *= damageMultiplier;
        receiver.TakeDamage(passedInfo);
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

    public void Clean(ObjectPool<ProjectileBase> pool)
    {
        this.gameObject.SetActive(true);
        //this.transform.rotation = Quaternion.identity;
        gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        damageInfo = new DamageInfo();
        timeAlive = 0.0f;
        usedPierce = 0f;
        hitTargets.Clear();
        direction = Vector3.forward;
        sizeMultiplier = 1f;
        speedMultiplier = 1f;
        damageMultiplier = 1f;
        knockbackMultiplier = 1f;
        lifetimeMultiplier = 1f;
        this.pool = pool;
        OnClean();
    }

    /// <summary>
    /// Finds the closest enemy
    /// </summary>
    /// <returns>The closest enemy to the player or null if none are within the player's range</returns>
    protected Enemy GetClosestEnemy()
    {
        List<Enemy> enemies = EnemyManager.instance.AllEnemies;
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
    public abstract void OnCollision(Collider2D other);

    public abstract void OnClean();

}
