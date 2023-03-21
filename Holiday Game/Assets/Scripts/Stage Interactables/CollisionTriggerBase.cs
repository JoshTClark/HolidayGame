using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionTriggerBase : MonoBehaviour
{
    [SerializeField]
    private bool removeAfterFirstTrigger = false;

    /// <summary>
    /// Checks if the object has a collider on start
    /// </summary>
    private void Start()
    {
        if (!this.gameObject.GetComponent<Collider2D>())
        {
            Debug.LogError($"Trigger object must have a Collider2D - Object{this.gameObject.name}");
        }
    }

    /// <summary>
    /// Called every frame when the trigger is colliding with another object
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            OnTrigger();
            if (removeAfterFirstTrigger)
            {
                Destroy(this.gameObject);
            }
        }
    }

    /// <summary>
    /// Called when the trigger is activated.
    /// Custom logic for when the trigger is activated goes here.
    /// </summary>
    public abstract void OnTrigger();

    private void OnDrawGizmos()
    {
        Collider2D c = this.gameObject.GetComponent<Collider2D>();
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawCube(c.gameObject.transform.position, c.bounds.size);
    }
}
