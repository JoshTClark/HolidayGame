using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public Player player;

    [SerializeField]
    Vector2 position = Vector2.zero;

    private Vector2 velocity = Vector2.zero;

    private float speed = 4;
    [SerializeField]
    private float health;

    void Update()
    {
        // Get the players position, seek that point, apply forces, and move
        velocity = seekPlayer(player) * speed;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    // Deals damage to the enemy
    public void DealDamage(float damage)
    {
        health -= damage;

    }
    Vector2 seekPlayer(Player player)
    {
        // Get the players position
        Vector2 desiredVelocity = (Vector2)player.transform.position - (Vector2)transform.position;

        return desiredVelocity.normalized;
    }
}
