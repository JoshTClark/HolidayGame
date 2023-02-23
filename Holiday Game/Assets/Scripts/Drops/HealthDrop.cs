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
            
            
            if (receiver.HasItem(ResourceManager.ItemIndex.Meat))
            {
                Item i = receiver.GetItem(ResourceManager.ItemIndex.Meat);
                if(i.HasTakenPath("Insatiable Appetite"))
                {
                    receiver.Heal(receiver.MaxHp / 3);
                }
                else receiver.Heal(receiver.MaxHp / 4);
                
            }
            else receiver.Heal(receiver.MaxHp / 4);
            //remove the health drop
            pool.Release(this);
        }
    }
}
