using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class DropBase : MonoBehaviour
{
    [SerializeField]
    public ResourceManager.PickupIndex index;

    public ObjectPool<DropBase> pool;

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

    public abstract void HandleCollision(Collider2D other);
}
