using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : StatsComponent
{
    public Player player;

    public ResourceManager.EnemyIndex index;

    protected Vector2 velocity = Vector2.zero;


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

    /// <summary>
    /// Moves the Enemy towards the player
    /// </summary>
    protected void SeekPlayer()
    {
        // Get the players position
        Vector2 desiredVelocity = (Vector2)player.transform.position - (Vector2)transform.position;

        // Get the players position, seek that point, apply forces, and move
        // Add Desired Velocity to velocity
        velocity += desiredVelocity;
    }

    /// <summary>
    /// Moves the Enemy away from the player
    /// </summary>
    protected void FleePlayer()
    {
        // Get Player Position
        Vector2 desiredVelocity = (Vector2)transform.position - (Vector2)player.transform.position;

        // Take the Player's position, flee from it and move
        velocity += desiredVelocity;
    }

    /// <summary>
    /// Stays within a certain distance of the player, either fleeing or seeking
    /// </summary>
    protected void ShooterMove()
    {
        // Calculate the distance & compare it to the values
        if (PlayerDistance() >= maxPlayerDist)
        {
            // Player is too far away, move closer
            SeekPlayer();
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
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    protected void Move()
    {
        // Take Velocity normalize it so it's a heading vector
        // scale that by speed, then apply movement
        GetComponent<Rigidbody2D>().velocity = velocity.normalized*Speed;
        velocity = Vector2.zero;
    }

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
        //Drops XP
        Instantiate<XP>(EnemyManager.instance.GetXPFromIndex(XPType), transform.position, Quaternion.identity);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleCollision(collision);
    }

    /// <summary>
    /// NEEDS WORK, CAN BE BETTER:
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
            Debug.Log("Hurt");
            // We hit the player, so they take damage
            GameManager.instance.Player.DealDamage(Damage);
        }
        else if (collision.gameObject.GetComponent<Enemy>())
        {
            Debug.Log("Other Enemy");
        }
    }
}
