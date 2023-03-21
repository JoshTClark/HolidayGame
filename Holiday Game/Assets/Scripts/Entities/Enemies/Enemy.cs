using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class Enemy : StatsComponent
{
    public Player player;
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
    protected Vector2 SeekPlayer()
    {
        // Get the players position
        Vector2 desiredVelocity = (Vector2)player.transform.position - (Vector2)transform.position;

        // Get the players position, seek that point, apply forces, and move
        return desiredVelocity;
    }

    /// <summary>
    /// Moves the Enemy away from the player
    /// </summary>
    protected Vector2 FleePlayer()
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
            return SeekPlayer();
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
        if (!HasBuff(ResourceManager.BuffIndex.Stunned))
        {
            CalcMoves();
        }
        if (transform.position.x < player.transform.position.x)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
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
        Vector2 knockbackAmount = Vector2.zero;
        if (knockback.Count > 0)
        {
            foreach (Vector2 v in knockback)
            {
                knockbackAmount += v;
            }
        }

        GetComponent<Rigidbody2D>().velocity = (velocity.normalized * Speed);
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

                if(i.HasTakenPath("Blood Transfusions"))
                {
                    float r = Random.Range(0, 100);
                    if(i.Level == 4 && r < 10)
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

        foreach (DropInfo info in drops)
        {
            Vector2 dropPosition = new Vector2(transform.position.x + Random.Range(-sr.sprite.rect.size.x / sr.sprite.pixelsPerUnit, sr.sprite.rect.size.x / sr.sprite.pixelsPerUnit) * transform.localScale.x * (2f / 3f), transform.position.y + Random.Range(-sr.sprite.rect.size.y / sr.sprite.pixelsPerUnit, sr.sprite.rect.size.y / sr.sprite.pixelsPerUnit) * transform.localScale.y * (2f / 3f));
            float ran = Random.value;
            if (info.index == ResourceManager.PickupIndex.HealthDrop1)
            {
                /*
                if (player.HasUpgrade(ResourceManager.UpgradeIndex.CupidArrowHealth) && dmgInfo.weapon == ResourceManager.WeaponIndex.CupidArrow)
                {
                    ran -= (float)(0.05 * GameManager.instance.Player.GetItem(ResourceManager.UpgradeIndex.CupidArrowHealth).CurrentLevel);
                */
            }
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

    virtual protected void OnTriggerStay2D(Collider2D collision)
    {
        HandleCollision(collision);
    }

    /// <summary>
    /// Handles the logic for colliding with a player
    /// </summary>
    /// <param name="collision"></param>
    private void HandleCollision(Collider2D collision)
    {
        /*
        Debug.Log("Here");
        if (!GameManager.instance.Player.Invincible)
        {
            GameManager.instance.Player.DealDamage(Damage);
        }
        */

        // Getting Component from the collider doesn't work properly for some reason
        // The code inside of the if statements will never be called for some reason

        if (collision.gameObject.GetComponent<Player>())
        {
            //Debug.Log("Hurt");
            // We hit the player, so they take damage
            DamageInfo info = new DamageInfo();
            info.damage = Damage;
            info.attacker = this;
            GameManager.instance.Player.TakeDamage(info);
        }
        else if (collision.gameObject.GetComponent<Enemy>())
        {
            //Debug.Log("Other Enemy");
        }
    }

    /// <summary>
    /// Has each enemy flee every other enemy scaled by it's distance from each other
    /// </summary>
    protected Vector2 Separation()
    {
        // Set an empty Desired Velocity
        Vector2 desiredVelocity = Vector2.zero;
        Vector2 currentVelocity = Vector2.zero;

        // Commenting this out for right now as looping through every single enemy is very costly
        // Loop through all enemies
        /*
        foreach (Enemy e in EnemyManager.instance.AllEnemies)
        {
            currentVelocity = Vector2.zero;
            float sqrDistance = Vector2.SqrMagnitude(transform.position - e.transform.position);
            if (sqrDistance <= Mathf.Epsilon)
            {
                continue;
            }
            // Flee the Enemy
            currentVelocity = (Vector2)transform.position - (Vector2)e.transform.position;

            // Scale it by how close it is & apply it to the desired velocity
            desiredVelocity = currentVelocity * (1f / sqrDistance);
        }
        */
        return desiredVelocity.normalized;
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
