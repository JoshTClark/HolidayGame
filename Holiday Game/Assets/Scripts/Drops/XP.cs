using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XP : DropBase
{
    [SerializeField]
    private float XPAmount;

    private void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.Normal)
        {
            if (PlayerDistance() <= GameManager.instance.Player.PickupRange)
            {
                SeekPlayer();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HandleCollision(other);
    }
    private void HandleCollision(Collider2D other)
    {
        //if colliding with player
        if (other.gameObject.GetComponent<Player>())
        {
            //access player and add xp to it
            StatsComponent receiver = other.gameObject.GetComponent<StatsComponent>();
            receiver.AddXP(XPAmount);
            //remove the xp gem
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Moves towards the player
    /// </summary>
    protected void SeekPlayer()
    {
        // Get the players position
        Vector2 desiredVelocity = ((Vector2)GameManager.instance.Player.transform.position - (Vector2)transform.position).normalized;

        // Get the players position, seek that point, apply forces, and move
        Vector2 velocity = desiredVelocity * 20;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    /// <summary>
    /// Calculates the distance between player & XP
    /// </summary>
    /// <returns>Distance between</returns>
    public float PlayerDistance()
    {
        return Vector2.Distance((Vector2)GameManager.instance.Player.transform.position, (Vector2)transform.position);
    }
}
