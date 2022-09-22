using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : StatsComponent
{
    public Player player;

    public GameManager.EnemyIndex index;

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
    public XP XPType;

    /// <summary>
    /// Moves the Enemy towards the player
    /// </summary>
    protected void seekPlayer()
    {
        // Get the players position
        Vector2 desiredVelocity = (Vector2)player.transform.position - (Vector2)transform.position;

        // Get the players position, seek that point, apply forces, and move
        velocity = desiredVelocity * Speed;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    /// <summary>
    /// Moves the Enemy away from the player
    /// </summary>
    protected void fleePlayer()
    {
        // Get Player Position
        Vector2 desiredVelocity = (Vector2)transform.position - (Vector2)player.transform.position;

        // Take the Player's position, flee from it and move
        velocity = desiredVelocity * Speed;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    /// <summary>
    /// Stays within a certain distance of the player, either fleeing or seeking
    /// </summary>
    protected void shooterMove()
    {
        // Calculate the distance & compare it to the values
        if(playerDistance() >= maxPlayerDist)
        {
            // Player is too far away, move closer
            seekPlayer();
        }
        else if(playerDistance() <= minPlayerDist)
        {
            //player is too close, move away
            fleePlayer();
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Calculates the distance between player & enemy
    /// </summary>
    /// <returns>Distance between</returns>
    protected float playerDistance()
    {
        return Vector2.Distance((Vector2)player.transform.position, (Vector2)transform.position);
    }

    public override void OnDeath()
    {
        //Drops XP
        Instantiate<XP>(XPType, transform.position, Quaternion.identity);
    }
}
