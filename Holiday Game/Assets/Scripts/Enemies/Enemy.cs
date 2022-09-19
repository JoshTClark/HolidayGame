using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : StatsComponent
{
    public Player player;

    public GameManager.EnemyIndex index;

    protected Vector2 velocity = Vector2.zero;

    //Which XP prefab this enemy will drop
    [SerializeField]
    public XP XPType;

    // Moves the enemy towards the player
    protected void seekPlayer()
    {
        // Get the players position
        Vector2 desiredVelocity = (Vector2)player.transform.position - (Vector2)transform.position;

        // Get the players position, seek that point, apply forces, and move
        velocity = desiredVelocity * Speed;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    public override void OnDeath()
    {
        //Drops XP
        Instantiate<XP>(XPType, transform.position, Quaternion.identity);
    }
}
