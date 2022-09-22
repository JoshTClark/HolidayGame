using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XP : MonoBehaviour
{
    [SerializeField]
    private float XPAmount;

    [SerializeField]
    public EnemyManager.XPIndex index;

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
}
