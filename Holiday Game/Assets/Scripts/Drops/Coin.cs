using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : DropBase
{
    public override void HandleCollision(Collider2D other)
    {
        //if colliding with player
        if (other.gameObject.GetComponent<Player>())
        {
            //add 5 gold to total
            GameManager.instance.pickupGems(1);
            //remove the health drop
            pool.Release(this);
        }
    }
}
