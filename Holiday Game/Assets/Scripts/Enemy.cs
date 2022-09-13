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

    private float speed = 5;
    void Update()
    {
        // Get the players position, seek that point, apply forces, and move
        velocity = seekPlayer(player) * speed;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }
    Vector2 seekPlayer(Player player)
    {
        // Get the players position
        Vector2 desiredVelocity = (Vector2)player.transform.position - position;

        return desiredVelocity.normalized;
    }
}
