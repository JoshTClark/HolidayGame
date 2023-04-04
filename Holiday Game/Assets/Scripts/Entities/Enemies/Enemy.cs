using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class Enemy : StatsComponent
{
    public Player player;
    public GameObject attractor;
    public ObjectPool<Enemy> pool;
    public ResourceManager.EnemyIndex index;
    public bool isBoss;
    public EnemyDeathEffect deathEffect;

    protected List<Vector2> movements = new List<Vector2>();
    protected List<Vector2> knockback = new List<Vector2>();

    /// <summary>
    /// Minimum distance between enemy & player
    /// </summary>
    protected float minPlayerDist;

    /// <summary>
    /// Maximum distance between enemy & player
    /// </summary>
    protected float maxPlayerDist;

    // Drops
    [SerializeField]
    private List<DropInfo> drops;

    // SoundEffects
    [SerializeField]
    protected AudioSource onHitSound;

    protected Vector2 Velocity { get { return GetComponent<Rigidbody2D>().velocity; } set { GetComponent<Rigidbody2D>().velocity = value; } }

    /// <summary>
    /// Moves the Enemy towards the player
    /// </summary>
    protected Vector2 Seek()
    {
        Vector2 desiredVelocity = new Vector2();
        if (attractor) 
        {
            // If another object is attracting the enemy seek it
            desiredVelocity = (Vector2)attractor.transform.position - (Vector2)transform.position;
        }
        else
        {
            // Seek the player if there is no object
            desiredVelocity = (Vector2)player.transform.position - (Vector2)transform.position;
        }
        return desiredVelocity;
    }

    /// <summary>
    /// Moves the Enemy away from the player
    /// </summary>
    protected Vector2 Flee()
    {
        // Get Player Position
        Vector2 desiredVelocity = (Vector2)transform.position - (Vector2)player.transform.position;

        // Take the Player's position, flee from it and move
        return desiredVelocity;
    }

    /// <summary>
    /// Stays within a certain distance of the player, either fleeing or seeking
    /// </summary>
    protected Vector2 ShooterMove()
    {
        // Calculate the distance & compare it to the values
        if (PlayerDistance() >= maxPlayerDist)
        {
            // Player is too far away, move closer
            return Seek();
        }
        /*
        else if (PlayerDistance() <= minPlayerDist)
        {
            //player is too close, move away
            FleePlayer();
        }
        */
        else
        {
            return Vector2.zero;
        }
    }

    protected void Move()
    {
        if (!isStunned)
        {
            CalcMoves();
            // Then take Velocity normalize it so it's a heading vector
            // scale that by speed, then apply movement
            Vector2 velocity = Vector2.zero;
            if (movements.Count > 0)
            {
                foreach (Vector2 v in movements)
                {
                    velocity += v;
                }
            }

            if (velocity.normalized.x >= 0)
            {
                sr.flipX = true;
            }
            else
            {
                sr.flipX = false;
            }

            GetComponent<Rigidbody2D>().velocity = (velocity.normalized * Speed);
        }
        else
        {
            GetComponent<Rigidbody2D>().Sleep();
        }

        Vector2 knockbackAmount = Vector2.zero;
        if (knockback.Count > 0)
        {
            foreach (Vector2 v in knockback)
            {
                knockbackAmount += v;
            }
        }
        GetComponent<Rigidbody2D>().position = GetComponent<Rigidbody2D>().position + (knockbackAmount);
        movements.Clear();
        knockback.Clear();
    }

    /// <summary>
    /// Put the actual movement logic in here
    /// Do this by adding the vectors to the lists,
    /// REMEMBER: scale Separation down, and Scale up Seek
    /// </summary>
    abstract protected void CalcMoves();

    /// <summary>
    /// Calculates the distance between player & enemy
    /// </summary>
    /// <returns>Distance between</returns>
    public float PlayerDistance()
    {
        return Vector2.Distance((Vector2)player.transform.position, (Vector2)transform.position);
    }

    public override void OnDeath(DamageInfo dmgInfo)
    {
        // Stop the hit sound
        if (onHitSound.isPlaying)
        {
            onHitSound.Stop();
        }

        if (dmgInfo.attacker && dmgInfo.attacker.GetType() == typeof(Player))
        {
            GameManager.instance.enemiesDefeated++;

            if (dmgInfo.attacker.HasItem(ResourceManager.ItemIndex.Vampire)) // Tracks kills after player hits path 2 vamp upgrade, and does random heal on kill 
            {
                Item i = dmgInfo.attacker.GetItem(ResourceManager.ItemIndex.Vampire);

                if (i.HasTakenPath("Blood Transfusions"))
                {
                    float r = Random.Range(0, 100);
                    if (i.Level == 4 && r < 10)
                    {
                        dmgInfo.attacker.Heal(dmgInfo.attacker.MaxHp * 0.03f);
                    }
                    if (i.Level == 4 && r < 15)
                    {
                        dmgInfo.attacker.Heal(dmgInfo.attacker.MaxHp * 0.03f);
                    }
                    if (i.Level == 6 && r < 20)
                    {
                        dmgInfo.attacker.Heal(dmgInfo.attacker.MaxHp * 0.03f);
                    }

                    if (i.Level == 6) dmgInfo.attacker.vampKills++;
                }
            }
        }

        if (dmgInfo.attacker && dmgInfo.attacker.HasItem(ResourceManager.ItemIndex.MagicMissile))
        {
            Item i = dmgInfo.attacker.GetItem(ResourceManager.ItemIndex.MagicMissile);

            if (i.HasTakenPath("Burning"))
            {
                if (i.Level >= 8)
                {
                    ProjectileBase projectile = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.Explosion);
                    projectile.gameObject.transform.position = this.gameObject.transform.position;
                    DamageInfo info = new DamageInfo();
                    info.radialKnockback = true;
                    info.damagePos = this.transform.position;
                    info.attacker = dmgInfo.attacker;
                    info.damage = dmgInfo.attacker.Damage * 0.5f;
                    info.knockback = 0.25f;
                    // Burn Effect
                    DotInfo burn = new DotInfo();
                    burn.duration = 0.75f;
                    burn.index = ResourceManager.BuffIndex.Burning;
                    burn.damagePerTick = dmgInfo.attacker.Damage * 0.1f;
                    burn.tickRate = 0.5f;
                    // Level 2 burn
                    if (i.Level >= 2)
                    {
                        burn.damagePerTick = dmgInfo.attacker.Damage * 0.15f;
                    }

                    if (i.HasTakenPath("Burning"))
                    {
                        // Level 5 duration multiplier
                        if (i.Level >= 5)
                        {
                            burn.duration *= 2;
                        }

                        // Level 7 burn speed
                        if (i.Level >= 5)
                        {
                            burn.tickRate /= 1.25f;
                            burn.damagePerTick = dmgInfo.attacker.Damage * 0.4f;
                        }
                    }
                    burn.chance = 0.25f;
                    burn.isDebuff = true;
                    info.buffs.Add(burn);
                    projectile.SetDamageInfo(info);
                    projectile.DamageMultiplier = 1f;
                    projectile.Lifetime = 0.25f;
                    projectile.Pierce = 999f;
                    projectile.Size = 1.5f;
                    projectile.SizeMultiplier = 1f;
                    projectile.projectileTeam = ProjectileBase.Team.Player;
                    GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Explosion"), transform.position, Quaternion.identity);
                    explosion.GetComponent<SpriteRenderer>().size = new Vector3(1.5f, 1.5f);
                    explosion.gameObject.transform.localScale = new Vector3(projectile.Size, projectile.Size);
                }
            }
        }

        foreach (DropInfo info in drops)
        {
            Vector2 dropPosition = new Vector2(transform.position.x + Random.Range(-sr.sprite.rect.size.x / sr.sprite.pixelsPerUnit, sr.sprite.rect.size.x / sr.sprite.pixelsPerUnit) * transform.localScale.x * (2f / 3f), transform.position.y + Random.Range(-sr.sprite.rect.size.y / sr.sprite.pixelsPerUnit, sr.sprite.rect.size.y / sr.sprite.pixelsPerUnit) * transform.localScale.y * (2f / 3f));
            float ran = Random.value;
            if (ran <= info.chance)
            {
                DropBase b = DropManager.GetPickup(info.index);
                b.gameObject.transform.position = dropPosition;
            }
        }

        if (deathEffect)
        {
            EnemyDeathEffect gameObject = GameObject.Instantiate<EnemyDeathEffect>(deathEffect);
            if (sr.flipX) gameObject.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0f);
            gameObject.GetComponent<SpriteRenderer>().sprite = this.gameObject.GetComponent<SpriteRenderer>().sprite;
        }
    }

    /// <summary>
    /// Handles the logic for colliding with a player
    /// </summary>
    /// <param name="collision"></param>
    public void HandleCollision(Collider2D collision)
    {
        // Getting Component from the collider doesn't work properly for some reason
        // The code inside of the if statements will never be called for some reason

        if (collision.gameObject.GetComponent<Player>())
        {
            //Debug.Log("Hurt");
            // We hit the player, so they take damage
            if (!isStunned)
            {
                DamageInfo info = new DamageInfo();
                info.damage = Damage;
                info.attacker = this;
                GameManager.instance.Player.TakeDamage(info);
            }
        }
    }

    public void AddKnockback(Vector2 vec)
    {
        knockback.Add(vec);
    }
    public override void TakeDamage(DamageInfo info)
    {
        base.TakeDamage(info);
        /*
        if (!onHitSound.isPlaying)
        {
            // Play Audio
            onHitSound.Play();
        }
        */
    }
    public virtual void Clean(ObjectPool<Enemy> pool)
    {
        movements.Clear();
        knockback.Clear();
        buffs.Clear();
        currentHP = MaxHp;
        player = null;
        inventory.Clear();
        this.pool = pool;
        this.gameObject.SetActive(true);
        isDead = false;
    }

    [System.Serializable]
    private class DropInfo
    {
        public ResourceManager.PickupIndex index;
        [Range(0, 1)]
        public float chance;
    }
}
