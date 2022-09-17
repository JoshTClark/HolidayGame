using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : StatsComponent
{
    public Player player;

    public GameManager.EnemyIndex index;

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
