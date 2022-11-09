using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class DropBase : MonoBehaviour
{
    [SerializeField]
    public ResourceManager.PickupIndex index;
    public float pickupRangeMultiplier = 1f;

    public ObjectPool<DropBase> pool;
    private void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.Normal)
        {
            if (PlayerDistance() <= GameManager.instance.Player.PickupRange * pickupRangeMultiplier)
            {
                SeekPlayer();
            }
        }
    }

    // Called when a collision occurs
    private void OnTriggerStay2D(Collider2D other)
    {
        HandleCollision(other);
    }

    // Resets the pickup for use again in the pool
    public void Clean(ObjectPool<DropBase> p)
    {
        this.gameObject.SetActive(true);
        pool = p;
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

    public abstract void HandleCollision(Collider2D other);
}
