using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : StatsComponent
{
    public Player player;

    public GameManager.EnemyIndex index;

    protected Vector2 velocity = Vector2.zero;

    // Moves the enemy towards the player
    protected void seekPlayer()
    {
        // Get the players position
        Vector2 desiredVelocity = (Vector2)player.transform.position - (Vector2)transform.position;

        // Get the players position, seek that point, apply forces, and move
        velocity = desiredVelocity * Speed;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }
}
