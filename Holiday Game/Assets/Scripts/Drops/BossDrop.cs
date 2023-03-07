using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDrop : DropBase
{
    public override void HandleCollision(Collider2D other)
    {
        //if colliding with player
        if (other.gameObject.GetComponent<Player>())
        {
            /*
            GameManager.instance.PlayerPickupBossDrop(1);
            pool.Release(this);
            */
        }
    }
}
