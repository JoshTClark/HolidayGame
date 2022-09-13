using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player now derives from StatsComponent. This helps it keep track of all of its stats and helps organize things a lot
public class Enemy : StatsComponent
{
    [SerializeField]
    public Player player;

    private Vector2 velocity = Vector2.zero;

    void Update()
    {
        // Get the players position, seek that point, apply forces, and move
        velocity = seekPlayer(player) * Speed;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    Vector2 seekPlayer(Player player)
    {
        // Get the players position
        Vector2 desiredVelocity = (Vector2)player.transform.position - (Vector2)transform.position;

        return desiredVelocity.normalized;
    }
}
