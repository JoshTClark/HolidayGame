using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : DropBase
{
    public override void HandleCollision(Collider2D other)
    {
        //if colliding with player
        if (other.gameObject.GetComponent<Player>())
        {
            //access player and add hp to it
            StatsComponent receiver = other.gameObject.GetComponent<StatsComponent>();
            receiver.Heal(receiver.MaxHp / 4);
            //remove the health drop
            pool.Release(this);
        }
    }
}
