using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool onlyWeapon = false;
    public bool onlyItems = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            GameManager.instance.ChestPickup(this);
            this.gameObject.SetActive(false);
        }
    }
}
