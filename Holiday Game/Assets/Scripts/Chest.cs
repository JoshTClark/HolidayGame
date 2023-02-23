using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool onlyWeapon = false;
    public bool onlyItems = false;
    public int spawnChance;

    private void Start()
    {
        int spawn = Random.Range(0, 100);
  
        if(spawn > spawnChance)
        {
            this.gameObject.SetActive(false); 
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            GameManager.instance.ChestPickup(this);
            this.gameObject.SetActive(false);
        }
    }
}
