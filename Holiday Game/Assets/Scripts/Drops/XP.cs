using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XP : DropBase
{
    [SerializeField]
    private float XPAmount;
    [SerializeField] private AudioClip pickUp;

    public override void HandleCollision(Collider2D other)
    {
        //if colliding with player
        if (other.gameObject.GetComponent<Player>())
        {
            AudioManager.instance.PlaySound(pickUp, .5f);
            //access player and add xp to it
            StatsComponent receiver = other.gameObject.GetComponent<StatsComponent>();
            receiver.AddXP(XPAmount);
            //remove the xp gem
            pool.Release(this);
        }
    }
}
