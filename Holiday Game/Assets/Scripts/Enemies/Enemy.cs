using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : StatsComponent
{
    public Player player;

    public ResourceManager.EnemyIndex index;
    public bool isBoss;

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

    //Which XP prefab this enemy will drop
    [SerializeField]
    public ResourceManager.PickupIndex XPType;
    public ResourceManager.PickupIndex Drop;

    // SoundEffects
    [SerializeField]
    protected AudioSource onHitSound;

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

    public override void OnDeath()
    {
        // Stop the hit sound
        if (onHitSound.isPlaying)
        {
            onHitSound.Stop();
        }

        //Drops XP
        if (index != ResourceManager.EnemyIndex.Boss)
        {
            DropBase xp = DropManager.GetPickup(XPType);
            xp.gameObject.transform.position = transform.position;
        }

        //Health Drop
        float healthRandomizer = Random.value; //Randomize to get chance
        // If the player has the Cupid Health upgrade enemies have a higher chance of dropping health pickups
        if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.CupidArrowHealth))
        {
            // Enemies have + 5% chance of getting a health drop
            if (index == ResourceManager.EnemyIndex.Test)
            {
                // Base enemies have 6% chance to drop
                if (healthRandomizer <= 0.06)
                {
                    DropBase b = DropManager.GetPickup(ResourceManager.PickupIndex.HealthDrop1);
                    b.gameObject.transform.position = transform.position;
                }
            }
            else if (index == ResourceManager.EnemyIndex.Test2)
            {
                // Shooter enemies have 10% chance to drop
                if (healthRandomizer <= 0.10)
                {
                    DropBase b = DropManager.GetPickup(ResourceManager.PickupIndex.HealthDrop1);
                    b.gameObject.transform.position = transform.position;
                }
            }
            else if (index == ResourceManager.EnemyIndex.BigBoy)
            {
                // Large enemies have 20% chance to drop
                if (healthRandomizer <= 0.20)
                {
                    DropBase b = DropManager.GetPickup(ResourceManager.PickupIndex.HealthDrop1);
                    b.gameObject.transform.position = transform.position;
                }
            }
        }
        else if(!GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.CupidArrowHealth)) 
        {
            if (index == ResourceManager.EnemyIndex.Test)
            {
                // Base enemies have 1% chance to drop
                if (healthRandomizer <= 0.01)
                {
                    DropBase b = DropManager.GetPickup(ResourceManager.PickupIndex.HealthDrop1);
                    b.gameObject.transform.position = transform.position;
                }
            }
            else if (index == ResourceManager.EnemyIndex.Test2)
            {
                // Shooter enemies have 5% chance to drop
                if (healthRandomizer <= 0.05)
                {
                    DropBase b = DropManager.GetPickup(ResourceManager.PickupIndex.HealthDrop1);
                    b.gameObject.transform.position = transform.position;
                }
            }
            else if (index == ResourceManager.EnemyIndex.BigBoy)
            {
                // Large enemies have 15% chance to drop
                if (healthRandomizer <= 0.15)
                {
                    DropBase b = DropManager.GetPickup(ResourceManager.PickupIndex.HealthDrop1);
                    b.gameObject.transform.position = transform.position;
                }
            }
        }
        
        else if (index == ResourceManager.EnemyIndex.Boss)
        {
            // Boss enemies have 100% chance to drop
            DropBase b1 = DropManager.GetPickup(ResourceManager.PickupIndex.HealthDrop1);
            b1.gameObject.transform.position = new Vector2(transform.position.x + Random.Range(-2.5f, 2.5f), transform.position.y + Random.Range(-2.5f, 2.5f));
            DropBase b2 = DropManager.GetPickup(ResourceManager.PickupIndex.HealthDrop1);
            b2.gameObject.transform.position = new Vector2(transform.position.x + Random.Range(-2.5f, 2.5f), transform.position.y + Random.Range(-2.5f, 2.5f));
            DropBase b3 = DropManager.GetPickup(ResourceManager.PickupIndex.HealthDrop1);
            b3.gameObject.transform.position = new Vector2(transform.position.x + Random.Range(-2.5f, 2.5f), transform.position.y + Random.Range(-2.5f, 2.5f));

            // Boss xp drops
            DropBase xp1 = DropManager.GetPickup(ResourceManager.PickupIndex.XP2);
            xp1.gameObject.transform.position = new Vector2(transform.position.x + Random.Range(-2.5f, 2.5f), transform.position.y + Random.Range(-2.5f, 2.5f));
            DropBase xp2 = DropManager.GetPickup(ResourceManager.PickupIndex.XP2);
            xp2.gameObject.transform.position = new Vector2(transform.position.x + Random.Range(-2.5f, 2.5f), transform.position.y + Random.Range(-2.5f, 2.5f));
            DropBase xp3 = DropManager.GetPickup(ResourceManager.PickupIndex.XP1);
            xp3.gameObject.transform.position = new Vector2(transform.position.x + Random.Range(-2.5f, 2.5f), transform.position.y + Random.Range(-2.5f, 2.5f));
            DropBase xp4 = DropManager.GetPickup(ResourceManager.PickupIndex.XP1);
            xp4.gameObject.transform.position = new Vector2(transform.position.x + Random.Range(-2.5f, 2.5f), transform.position.y + Random.Range(-2.5f, 2.5f));
            DropBase xp5 = DropManager.GetPickup(ResourceManager.PickupIndex.XP1);
            xp5.gameObject.transform.position = new Vector2(transform.position.x + Random.Range(-2.5f, 2.5f), transform.position.y + Random.Range(-2.5f, 2.5f));
            DropBase xp6 = DropManager.GetPickup(ResourceManager.PickupIndex.XP1);
            xp6.gameObject.transform.position = new Vector2(transform.position.x + Random.Range(-2.5f, 2.5f), transform.position.y + Random.Range(-2.5f, 2.5f));

            DropBase drop = DropManager.GetPickup(ResourceManager.PickupIndex.BossDrop);
            drop.gameObject.transform.position = new Vector2(transform.position.x, transform.position.y);
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

        // Loop through all enemies
        foreach (Enemy e in EnemyManager.instance.CurrentEnemies)
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
        return desiredVelocity.normalized;
    }

    public void AddKnockback(Vector2 vec)
    {
        knockback.Add(vec);
    }
    public override void TakeDamage(DamageInfo info)
    {
        base.TakeDamage(info);
        if (!onHitSound.isPlaying)
        {
            // Play Audio
            onHitSound.Play();
        }
    }
}
