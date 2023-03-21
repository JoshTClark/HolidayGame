using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGem : MonoBehaviour
{
    // Called when a collision occurs
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>()) 
        {
            GameManager.instance.CollectBossGem(this);
            this.GetComponent<Collider2D>().enabled = false;
        }
    }
}
